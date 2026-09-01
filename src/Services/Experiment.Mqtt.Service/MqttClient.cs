#define MQTT_SUBSCRIBE_ALL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using MQTTnet.Protocol;
//using MQTTnet.Samples.Helpers;

using Experiment.Core;
using Experiment.Core.Metadata;
using Experiment.Core.IO;
using Experiment.Core.BL.Data;

using Experiment.Mqtt.Service.Models;

namespace Experiment.Mqtt.Service{
	internal class MqttClient
	{
		#region Const
		internal static readonly string[] SUBSCRIBE_ALL = new string[] { "#" };

		#endregion

		#region Attributes
		/// <summary>
		/// Service logger
		/// </summary>
		ILogger _Logger;

		/// <summary>
		/// Topics loggers
		/// </summary>
		Dictionary<string, ILogger> _Loggers;

		MqttFactory _Factory;
		ExpSql _Db;

		#endregion

		#region Properties

		internal BrokerMqtt Broker { get; set; }
		internal IMqttClient Client { get; set; }
		internal MqttClientOptions Options { get; set; }
		internal MqttFactory Factory
		{
			get
			{
				if (_Factory == null)
				{
					_Factory = new MqttFactory();
				}

				return _Factory;
			}
		}
		internal bool Connected
		{
			get
			{
				if (Client != null)
				{
					return Client.IsConnected;
				}

				return false;
			}
		}

		#endregion

		#region Static
		internal static string TopicsLoggingFolder
		{
			get
			{
				return MqttService.LoggingFolder + @"Topics\";
			}
		}

		#endregion

		#region Ctor
		public MqttClient(ILogger logger)
		{
			_Logger = logger;
			_Loggers = new Dictionary<string, ILogger>();
			_Db = ExpSql.GenerateFromDefaults(logger);
		}

		#endregion

		#region Helpers
		internal MqttClientOptions CreateOptions(BrokerMqtt broker)
		{
			Validation.RequireValid(broker, nameof(broker));
			Validation.RequireValidString(broker.Host, $"{nameof(broker)}.{nameof(BrokerMqtt.Host)}");

			var b = new MqttClientOptionsBuilder()
				.WithTcpServer(broker.Host, broker.Port)
				.WithClientId(broker.ClientId);
			//.WithKeepAlivePeriod(new TimeSpan(0, 1, 0));

			// The used public broker sometimes has invalid certificates. This sample accepts all
			// certificates. This should not be used in live environments.

			// This doesn't work well, caused exception
			//.WithTlsOptions(o => o.WithCertificateValidationHandler(_ => true))

			if (!string.IsNullOrEmpty(broker.Username) &&
				!string.IsNullOrEmpty(broker.Password))
			{
				b.WithCredentials(broker.Username, broker.Password);
			}

			var options = b.Build();
			return options;
		}

		internal async Task<bool> ConnectAsync()
		{
			try
			{
				_Logger.WriteLine(5, $"{nameof(Connected)}={Connected}");
				if (!Connected)
				{

					Client = Factory.CreateMqttClient();
					Client.ApplicationMessageReceivedAsync += e =>
					{
						// MQTT not designed to return publisher's clientId
						// This clientid is this own client's
						//var clientId = e.ClientId;
						var topic = e.ApplicationMessage.Topic;
						var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment.ToArray());
						ReceivedData(topic, payload);

						return Task.CompletedTask;
					};
					Options = CreateOptions(Broker);
					var result = await Client.ConnectAsync(Options, CancellationToken.None);
				}
			}
			catch (Exception ex)
			{
				_Logger.WriteLine(0, ex.Message);
			}

			_Logger.WriteLine(5, $"{nameof(Connected)}={Connected}");
			return Connected;
		}

		protected async Task SubscribeAsync()
		{
#if MQTT_SUBSCRIBE_ALL
			await SubscribeAsync(SUBSCRIBE_ALL);
#else
			await SubscribeAsync(Broker.Topics.Distinct());
#endif
		}

		/// <summary>
		/// Update received values
		/// </summary>
		/// <param name="topic"></param>
		/// <param name="payload"></param>
		protected void ReceivedData(string topic, string payload)
		{
			//_Logger.WriteLine(5, $"Topic: {topic}, payload: {payload}");

			// Report received
			Received(topic, payload);
			// Further processing will happen only if specific topic we have to save
			// As we subscribing all topics for statistics

			// All devices which contain specific topic / distinct
			var devices = Broker.Devices.Where(d => d.Topics.Contains(topic)).Distinct();
			// If found any
			if (devices.Count() > 0)
			{
				// Parse payload
				var values = PayloadMqtt.Parse(_Logger, payload);
				_Logger.WriteLine(5, $"Parsed {values.Count} values.");

				// For all affected devices
				foreach (var device in devices)
				{
					// With each received value
					foreach (var kvp in values)
					{
						// Check it exists?
						var datapoints = device.Datapoints.Where(dp =>
							dp.Topic.Equals(topic, StringComparison.OrdinalIgnoreCase) &&
							dp.Path.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase)
						).ToList();

						// if datapoint(s) not exists
						if (datapoints.Count < 1)
						{
							// Creating new one
							var datapoint = new DatapointMqtt()
							{
								Id = 0,
								DeviceId = device.Id,
								Name = kvp.Key,
								Topic = topic,
								Path = kvp.Key,
								Value = kvp.Value,
							};
							device.Datapoints.Add(datapoint);
							_Logger.WriteLine(5, $"DEV: {device.Id}!{device.Name}, NEW DTP:{datapoint.Id} {datapoint.Topic} => {datapoint.Path} = {datapoint.Value}");
						}
						else
						{
							// As exists, assigning value
							foreach (var datapoint in datapoints)
							{
								datapoint.Value = kvp.Value;
								//_Logger.WriteLine(5, $"DEV: {device.Id}!{device.Name}, UPD DTP:{datapoint.Id} {datapoint.Topic} => {datapoint.Path} = {datapoint.Value}");
							}
						} // else: if (datapoints.Count < 1)

					} // foreach (var kvp in values)

				} // foreach (var device in devices)

			} // if(devices.Count() > 0)
			else
			{
				//_Logger.WriteLine(5, "No further processing as topic not found");
			}

		}

		/// <summary>
		/// Report received data to log file and database
		/// </summary>
		/// <param name="url"></param>
		/// <param name="topic"></param>
		/// <param name="payload"></param>
		internal void Received(string topic, string payload)
		{
			// Make Url for logging
			var url = $"{Broker.Host}:{Broker.Port}";
			string loggingHandle = string.Empty;

			try
			{
				// Report to log
				// Create specific topic logging handle
				loggingHandle = Utils.ConvertToFileName(topic);
				///_Logger.WriteLine(5, $"Logging: {loggingHandle}");
				if (!_Loggers.ContainsKey(loggingHandle))
				{
					_Loggers.Add(loggingHandle,
						new FileLogger(
							_Logger.LogLevel,
							TopicsLoggingFolder,
							loggingHandle));
				}
				var topicLogger = _Loggers[loggingHandle];
				if (topicLogger != null)
				{
					topicLogger.WriteLine(5, $"{payload}");
				}

				// Report to the database
				_Db.MqttMessageReceived(url, topic, payload);
			}
			catch (Exception ex)
			{
				_Logger.WriteLine(0, $"{ex.Message}\n{url}|{topic}|{payload}, [{nameof(loggingHandle)}:{loggingHandle}]");
			}
		}

		/// <summary>
		/// @Deprecated, USE PayloadMqtt class instead!
		/// </summary>
		/// <param name="payload"></param>
		/// <returns></returns>
		protected Dictionary<string, decimal> ParsePayload(string payload)
		{
			var retVal = new Dictionary<string, decimal>();
			decimal value = 0;

			// if this is regular numeric/decimal text
			if (decimal.TryParse(payload, out value))
			{
				// Yes
				//_Logger.WriteLine(5, $"Regular value: {payload} => {value}");
				retVal.Add(string.Empty, value);
				return retVal;
			}


			// No, trying regexp
			var valueStr = Regex.Replace(payload, "[^0-9.]", "");
			_Logger.WriteLine(5, $"Regex {payload} => {valueStr}");
			if (decimal.TryParse(valueStr, out value))
			{
				// yes
				retVal.Add(string.Empty, value);
				return retVal;
			}

			try
			{
				var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(payload);
			}
			catch (Exception ex)
			{

			}

			_Logger.WriteLine(1, $"Regex {payload} => Failed!");
			return retVal;
		}

		#endregion

		#region Methods

		internal async Task<bool> UpdateBrokerAsync(BrokerMqtt broker)
		{
			_Logger.WriteLine(5, $"Broker: {broker.Host}:{broker.Port}");

			Broker = broker;

			return await MakeSureConnectedAndSubscribedAsync();
		}

		internal async Task<bool> MakeSureConnectedAndSubscribedAsync()
		{
			if (Broker == null)
			{
				_Logger.WriteLine(1, $"Broker Is Null!");
				return false;
			}

			_Logger.WriteLine(5, $"{nameof(Connected)}={Connected}");
			// If not connected
			if (!Connected)
			{
				// Let's connect
				await ConnectAsync();

				// If connected, let's subscribe
				if (Connected)
				{
					await SubscribeAsync();
				}
			}

			return Connected;
		}

		/// <summary>
		/// Publish payload to topic
		/// </summary>
		/// <param name="topic"></param>
		/// <param name="payload"></param>
		/// <returns></returns>
		public async Task Publish(string topic, string payload)
		{
			_Logger.WriteLine(5, $"{topic} => {payload}");

			var applicationMessage = new MqttApplicationMessageBuilder()
			   .WithTopic(topic)
			   .WithPayload(payload)
			   .Build();

			await Client.PublishAsync(applicationMessage, CancellationToken.None);
		}

		public async Task Disconnect()
		{
			_Logger.WriteLine(5, $"{nameof(Connected)}={Connected}");
			if (Connected)
			{
				await Client.DisconnectAsync(MqttClientDisconnectOptionsReason.NormalDisconnection);
			}
			_Logger.WriteLine(4, $"{nameof(Connected)}={Connected}");
		}

		public async Task SubscribeAsync(IEnumerable<string> topics)
		{
			_Logger.WriteLine(4, $"Broker: {Broker.Host}:{Broker.Port}, {nameof(BrokerMqtt.ClientId)}={Broker.ClientId}, {nameof(Connected)}={Connected}");
			if (Connected && topics != null)
			{
				var sob = Factory.CreateSubscribeOptionsBuilder();
				var included = 0;

				// We need to subscribe only once per device/broker,
				// as we don't subscribe by ClientId, but only by Topic.
				// Where we will have to identify the data anyway by ClientId, not just Topic.
				foreach (var topic in topics)
				{
					_Logger.WriteLine(4, $"Subscribing: {topic}");
					sob.WithTopicFilter(
						f =>
						{
							f.WithTopic(topic);
						});

					included++;
				}

				if (included > 0)
				{
					var so = sob.Build();

					_Logger.WriteLine(5, $"Subscribtion options built, subscribing...");
					var response = await Client.SubscribeAsync(so, CancellationToken.None);
				}
				else
				{
					_Logger.WriteLine(5, $"Nothing to subscribe!");
				}
			}
		}

		public async Task UnsubscribeAsync(IEnumerable<string> topics)
		{
			try
			{
				_Logger.WriteLine(4, $"Broker: {Broker.Host}:{Broker.Port}, {nameof(BrokerMqtt.ClientId)}={Broker.ClientId}, {nameof(Connected)}={Connected}");
				if (Connected && topics != null)
				{
					var uob = Factory.CreateUnsubscribeOptionsBuilder();
					var included = 0;

					foreach (var topic in topics)
					{
						_Logger.WriteLine(4, $"Preparing unsubscription for {topic}");
						uob.WithTopicFilter(topic);

						included++;
					}

					if (included > 0)
					{
						var uo = uob.Build();

						_Logger.WriteLine(5, $"Unsubscribe options built, unsubscribing...");
						var response = await Client.UnsubscribeAsync(uo, CancellationToken.None);
					}
					else
					{
						_Logger.WriteLine(5, $"Nothing to unsubscribe!");
					}
				}
			}
			catch (Exception ex)
			{
				_Logger.WriteLine(0, ex.Message);
			}
		}

		#endregion
	}
}
