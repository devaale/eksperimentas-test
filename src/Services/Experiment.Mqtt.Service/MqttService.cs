//#define TESTING_DATA
#define MQTT_SUBSCRIBE_ALL

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Experiment.Core;
using Experiment.Core.Enums;
using Experiment.Core.BL.Data;
using Experiment.Core.BL.Data.SysVars;
using Experiment.Core.IO;
using Experiment.Core.Metadata;

using Experiment.Mqtt.Service.Models;

namespace Experiment.Mqtt.Service{
	/// <summary>
	/// Service using MQTTnet library, @see https://github.com/dotnet/MQTTnet
	/// Library is under MIT license.
	/// </summary>
	partial class MqttService : ServiceBase
	{
		#region Constants
		const string TYPE_NAME = nameof(MqttService);
		const bool DEBUG = true;

		/// <summary>
		/// Defaut log level
		/// </summary>
		const int DEFAULT_LOG_LEVEL = 5;     // Default log level

		/// <summary>
		/// Interval in seconds, how frequentlt we checking to reconnect devices if they got disconnected
		/// </summary>
		const int DEFAULT_RECONNECT_CHECK_SECS = 2;

		/// <summary>
		/// Interval in seconds, how often we checking for db changes 
		/// </summary>
		const int DEFAULT_DB_CHANGES_CHECK_SECS = 10;

		#endregion

		#region Attributes
		bool _ServiceStarted;

		ExpSql _DbSub;
		ExpSql _DbPub;
		ILogger _Logger;
		EventLog _EventLog;

		Thread _Thread;
		private Task _SubTaskProcessing;
		private Task _PubTaskProcessing;
		private CancellationTokenSource _CancellationTokenSource;

		List<MqttClient> _Clients;

		IDictionary<SysVarName, object> _Vars = new Dictionary<SysVarName, object>();
		#endregion

		#region Properties
		internal static bool ConsoleMode
		{
			get { return Environment.UserInteractive; }
		}

		internal static string LoggingFolder
		{
			get
			{
				return Defaults.DEFAULT_LOG_FOLDER + @"MQTT\";
			}
		}

		#endregion

		#region Ctor
		public MqttService()
		{
			InitializeComponent();

			//_VarsLoopDelay = DEFAULT_LOOP_DELAY_SEC;

			_ServiceStarted = false;
			if (ConsoleMode)
			{
				_Logger = new ConsoleLogger(
					DEFAULT_LOG_LEVEL,
					LoggingFolder,
					Program.ServiceName);
			}
			else
			{
				_Logger = new FileLogger(
					DEFAULT_LOG_LEVEL,
					LoggingFolder,
					Program.ServiceName);
			}

			InitEventLog();
			_DbSub = ExpSql.GenerateFromDefaults(_Logger);
			_DbPub = ExpSql.GenerateFromDefaults(_Logger);
			UpdateSysVars();
		}


		#endregion

		#region Helpers
		void InitEventLog()
		{
			try
			{
				_EventLog = new EventLog();// "Application");
				_EventLog.Source = Program.ServiceName;

			}
			catch (Exception ex)
			{
				_Logger.WriteLine(0, Program.ServiceName + ": Initialization of event log went wrong: " + ex.Message);
			}
		}

		void UpdateSysVars()
		{
			//if (_Logger is FileLogger logger)
			//{
			//	logger.LogFolder = _DbSub.SysVarGet(SysVarName.LOG_LOCATION);
			//}
			_Logger.LogLevel = Convert.ToInt32(_DbSub.SysVarGet(SysVarName.MQTT_LOG_LEVEL));
		}

		void WriteToEventLog(string msg)
		{
			if (_EventLog != null)
			{
				_EventLog.WriteEntry(msg);
			}
		}
		#endregion

		#region Methods
		protected override void OnStart(string[] args = null)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(OnStart));
			WriteToEventLog(vLoc);
			_Logger.WriteLine(3, nameof(OnStart));

			_ServiceStarted = true;


			_Logger.WriteLine(3, $"Thread.Start() => {nameof(StartAndWait)}");
			_Thread = new Thread(StartAndWait);
			_Thread.Start();
			_Logger.WriteLine(3, $"{nameof(OnStart)} Finished!");
		}

		protected override void OnStop()
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(OnStop));
			var msg = string.Format("{0}...\n\n\n\n\n\n\n\n", vLoc);
			_Logger.WriteLine(3, msg);

			WriteToEventLog(msg);

			_ServiceStarted = false;

			_CancellationTokenSource.Cancel();

			try
			{
				_PubTaskProcessing.Wait();
				_SubTaskProcessing.Wait();
			}
			catch (Exception e)
			{
				// handle exeption
			}
		}

		internal void Start()
		{
			OnStart();
		}

		#endregion

		#region Helpers
		internal List<BrokerMqtt> LoadBrokers()
		{
			var stage = "Init";
			var brokers = new List<BrokerMqtt>();

			try
			{
				var devicesTable = 0;
				var topicsTable = 1;
				var datapointsTable = 2;

				// Should match prcMqttServiceData SQL procedure fields
				stage = nameof(ExpSql.MqttServiceData);
				var ds = _DbSub.MqttServiceData();

				// ORM devices list
				stage = "ORM devices list";
				var devices = ds.Tables[devicesTable].AsEnumerable().Select(row => new DeviceMqtt()
				{
					Id = row.Field<int>(nameof(DeviceMqtt.Id)),
					Name = row.Field<string>(nameof(DeviceMqtt.Name)),
					Url = row.Field<string>(nameof(DeviceMqtt.Url)),
					Username = DBNull.Value.Equals(row[Defaults.DB_USERNAME]) ? null : row[Defaults.DB_USERNAME].ToString(),
					Password = DBNull.Value.Equals(row[Defaults.DB_PASSWORD]) ? null : row[Defaults.DB_PASSWORD].ToString(),
					Interval = row.Field<int>(Defaults.DB_DP_INTERVAL),
					LastScanTime = DBNull.Value.Equals(row[Defaults.DB_DP_LAST_SCAN_TIME]) ? (DateTime?)null : row.Field<DateTime>(Defaults.DB_DP_LAST_SCAN_TIME), // troublesome? (AG) @see https://stackoverflow.com/a/12508003
					ProjectedScanTime = row.Field<DateTime>(Defaults.DB_DP_PROJECTED_SCAN_TIME),
					Topics = new List<string>(),

				}).ToList();

				// ORM Topics list
				stage = "ORM Topics list";
				var topics = ds.Tables[topicsTable].AsEnumerable().Select(row => new DeviceTopic()
				{
					DeviceId = row.Field<int>(Defaults.DB_DEVICE_ID),
					Topic = row.Field<string>(Defaults.DB_TOPIC),
				});

				// Assign topics to the devices
				stage = "Assign topics to the devices";
				foreach (var device in devices)
				{
					// Topics => topigs => to pigs (giggles)
					device.Topics = topics
						.Where(t => t.DeviceId == device.Id) // Specific device Id
						.Select(topic => topic.Topic).ToList(); // List of only topics
				}

				// ORM Datapoints
				stage = "ORM Datapoints";
				var datapoints = ds.Tables[datapointsTable].AsEnumerable().Select(row => new DatapointMqtt()
				{
					Id = row.Field<int>(nameof(DatapointMqtt.Id)),
					DeviceId = row.Field<int>(nameof(DatapointMqtt.DeviceId)),
					Name = row.Field<string>(nameof(DatapointMqtt.Name)),
					Topic = row.Field<string>(nameof(DatapointMqtt.Topic)),
					Path = (row.Field<string>(nameof(DatapointMqtt.Path)) == null ? string.Empty : row.Field<string>(nameof(DatapointMqtt.Path))),
					Value = null,

				});//.ToList();

				//_Logger.WriteLine(5, $"Loaded {datapoints.Count()} datapoint(s)!");
				// Assigning datapoints to their devices
				stage = "Assigning datapoints to their devices";
				foreach (var device in devices)
				{
					device.Datapoints = datapoints
						.Where(dp => dp.DeviceId == device.Id)
						.ToList();
				}

				stage = "Init brokers";
				foreach (var dev in devices)
				{
					BrokerMqtt broker = brokers.Where(b =>
						b.Host.Equals(dev.Host, StringComparison.OrdinalIgnoreCase) &&
						b.Port == dev.Port).FirstOrDefault();
					if (broker == null)
					{
						broker = new BrokerMqtt()
						{
							Host = dev.Host,
							Port = dev.Port,
							Devices = new List<DeviceMqtt>(),
						};
						brokers.Add(broker);
					}

					if (broker != null)
					{
						broker.Devices.Add(dev);
					}
				}

			}
			catch(Exception ex)
			{
				_Logger.WriteLine(0, $"{stage}: {ex.Message}");
			}
			return brokers;
		}

		/// <summary>
		/// Loads new data from database
		/// </summary>
		/// <returns></returns>
		async Task<bool> UpdateSubData()
		{
			//_Logger.WriteLine(5, $"{nameof(LoadBrokers)}()..");
			var brokers = LoadBrokers();

			// If _Clients weren't initialized (first launch)
			if (_Clients == null)
			{
				_Logger.WriteLine(5, $"Clients are NULL (first launch?)");

				if(brokers != null)
				{
					_Clients = brokers.Select(broker => new MqttClient(_Logger)
					{
						Broker = broker,
					}).ToList();
				}
				else
				{
					_Logger.WriteLine(5, $"WARNING! {nameof(brokers)} also are null!");
					return false;
				}
			}
			else
			{
#if TESTING_DATA
				// Adding fake device to remove
				_Clients.Add(new MqttClient(_Logger)
				{
					Broker = new BrokerMqtt()
					{
						Host = "bla",
						Port = BrokerMqtt.DEFAULT_MQTT_PORT,
					}
				});
#endif
				// First checking for unavailable anymore devices
				var unavailableClients = _Clients.Where(c => !brokers.Any(b =>
					b.Host.Equals(c.Broker.Host, StringComparison.OrdinalIgnoreCase) &&
					b.Port == c.Broker.Port)).ToList();

				if (unavailableClients.Count > 0)
					_Logger.WriteLine(5, $"{nameof(unavailableClients)}: {unavailableClients.Count()}");

				foreach (var unavailableClient in unavailableClients)
				{
					//_Logger.WriteLine(5, $"Disconneting & deleting device, Id={unavailableDevice.Device.Id}, Name={unavailableDevice.Device.Name}");
					// Disconnecting unavailable anymore device
					await unavailableClient.Disconnect();
					// Removing its client from collection with specific broker itself
					_Clients.Remove(unavailableClient);
				}

				// Processing new brokers
				foreach (var broker in brokers)
				{
					// Client with such broker host already added? Its index?
					var clientIndex = _Clients.FindIndex(c =>
						c.Broker.Host.Equals(broker.Host, StringComparison.OrdinalIgnoreCase) &&
						c.Broker.Port == broker.Port);
					if (clientIndex == -1)
					{
						// Not, this is new device
						_Logger.WriteLine(5, $"New broker, host: {broker.Host}:{broker.Port}!");
						// and connected devices checking algorythm @see HeartBeatAsync>reconnectCheckInterval
						// will automatically connect it after few seconds
						_Clients.Add(new MqttClient(_Logger)
						{
							Broker = broker,
						});
					}
					else
					{
						// Yes, existing device
						var client = _Clients[clientIndex];
						//_Logger.WriteLine(5, $"Existing broker, host: {client.Broker.Host}:{client.Broker.Port}!");
						// Connected?
						if (client.Connected)
						{
#if !MQTT_SUBSCRIBE_ALL
							// Getting distinct topics of old and new data
							var oldDataTopics = client.Broker.Topics;
							var newDataTopics = broker.Topics;
							var changes = 0;

							// Topics for unsubscribtion
							var unavailableTopics = oldDataTopics.Where(ot => !newDataTopics.Any(nt => nt.Equals(ot, StringComparison.OrdinalIgnoreCase)));
							if (unavailableTopics.Count() > 0)
							{
								_Logger.WriteLine(5, $"Unavailable topics: {unavailableTopics.Count()}");
								await client.UnsubscribeAsync(unavailableTopics);
								changes++;
							}

							// New topics for subscribtion
							var newTopics = newDataTopics.Where(nt => !oldDataTopics.Any(ot => ot.Equals(nt, StringComparison.OrdinalIgnoreCase)));
							if (newTopics.Count() > 0)
							{
								_Logger.WriteLine(5, $"New topics: {newTopics.Count()}");
								await client.SubscribeAsync(newTopics);
								changes++;
							}
#endif // !MQTT_SUBSCRIBE_ALL

							// For each newly loadaded devices
							foreach (var nDevice in broker.Devices)
							{
								// Find device's old version
								var oDevice = client.Broker.Devices.FirstOrDefault(d => d.Id == nDevice.Id);
								if (oDevice != null)
								{
									// For each new version of old datapoint
									foreach (var nDatapoint in nDevice.Datapoints)
									{
										// Find old version of its datapoint
										var oDatapoint = oDevice.Datapoints.FirstOrDefault(oDp =>
											// NOTE! We do not search Datapoint Id, because only topic and path matter.
											// Idea is that we need to transfer value, while value is always tied to topic/path
											oDp.Topic.Equals(nDatapoint.Topic) &&
											oDp.Path.Equals(nDatapoint.Path)
										);

										// If found old version of this datapoint
										if (oDatapoint != null)
										{
											// Assign to the new version of this datapoint old its version value
											// As new version won't have even this one
											nDatapoint.Value = oDatapoint.Value;
										}
									}

									// For every not saved yet to database datapoint
									foreach (var oDatapoint in oDevice.Datapoints.Where(dp => dp.Id == 0))
									{
										// Do we alreayd added it to new array?
										var nDatapoint = nDevice.Datapoints.FirstOrDefault(oDp =>
											// NOTE! We do not search Datapoint Id, because only topic and path matter.
											// Idea is that we need to transfer value, while value is always tied to topic/path
											oDp.Topic.Equals(oDatapoint.Topic) &&
											oDp.Path.Equals(oDatapoint.Path)
										);

										// Not yet?
										if (nDatapoint == null)
										{
											//_Logger.WriteLine(5, $"Moving not saved new datapoint to new structure: {oDatapoint.Topic} => {oDatapoint.Path} = {oDatapoint.Value}");
											// Save it in new structure
											nDevice.Datapoints.Add(oDatapoint);
										}
									}
								}
							}

							// After we assigned values replacing client Broker completelly
							// As it is only model/data class, which is unrelated to connection
							// By the design 
							client.Broker = broker;

							// Old device will be purged by GC

						} // if (client.Connected)
					} // else if (clientIndex == -1)
				} // foreach (var broker in brokers)
			} // else if (_Clients == null)

			return true;
		}

		void UpdateSubDatabase()
		{
			var now = DateTime.Now;
			//_Logger.WriteLine(5, $"{now}..");

			// For each broker/client
			foreach (var client in _Clients)
			{
				// for each its device
				//_Logger.WriteLine(5, $"Client, Id:{client.Broker.ClientId}");
				foreach (var device in client.Broker.Devices)
				{
					var updated = 0;

					// If it is time to update data
					if (device.ProjectedScanTime <= DateTime.Now)
					{
						//_Logger.WriteLine(5, $"!Req^Update {device.Id}:{device.Name}");

						foreach (var dp in device.Datapoints)
						{
							UpdateDatapoint(updated + 1, device.Id, dp, now);
							updated++;
						}

						if (updated > 0)
						{
							_Logger.WriteLine(5, $"{nameof(ExpSql.DeviceLastScanUpdate)} {device.Id}, {now}");
							_DbSub.DeviceLastScanUpdate(device.Id, now);

							device.ProjectedScanTime = now.AddSeconds(device.Interval);
						}

						if (updated > 0)
							_Logger.WriteLine(5, $"Updated: {updated} datapoints!");
					}
					//else
					//{
					//	// Save new datapoints always imediatelly, if we have
					//	foreach (var dp in device.Datapoints)
					//	{
					//		if(dp.Id == 0)
					//		{
					//			UpdateDatapoint(updated + 1, device.Id, dp, now);
					//			updated++;
					//		}
					//	}
					//}
				}
			}

			//_Logger.WriteLine(5, $"End {now}..");
		}

		void UpdateDatapoint(int num, int deviceId, DatapointMqtt dp, DateTime now)
		{
			//_Logger.WriteLine(5, $"{nameof(EseSql.MqttValueSave)}#{num} DeviceId: {deviceId}, {dp.Topic} => {dp.Path} = {dp.Value}");
			_DbSub.MqttValueSave(deviceId, dp.Topic, dp.Path, dp.Value, now);

			// Nulling already saved value
			dp.Value = null;
		}

		#endregion

		#region Service logic
		/// <summary>
		/// Starter and Waiting thread for async Heartbeat start and waiting
		/// </summary>
		internal void StartAndWait()
		{
			_Logger.WriteLine(5, $"Starting..");
			_CancellationTokenSource = new CancellationTokenSource();

			// Subscriptions processing
			_SubTaskProcessing = Task.Run(() => MqttSubHeartbeatAsync(_CancellationTokenSource.Token));

			// Publishing processing
			_PubTaskProcessing = Task.Run(() => MqttPubHeartbeatAsync(_CancellationTokenSource.Token));

			// Wait until they will be ended
			// This saved the day. If not this line, service would end momentally
			_PubTaskProcessing.Wait();
			_SubTaskProcessing.Wait();
		}

		/// <summary>
		/// Subscriptions processing
		/// </summary>
		/// <param name="cancel"></param>
		/// <returns></returns>
		internal async Task MqttSubHeartbeatAsync(CancellationToken cancel)
		{
			_Logger.WriteLine(4, $"{nameof(LoadBrokers)}()..");
			var updated = await UpdateSubData();
			if (!updated)
				return;

			var reconnectCheckInterval = DEFAULT_RECONNECT_CHECK_SECS;
			var dbCheckInterval = DEFAULT_DB_CHANGES_CHECK_SECS;

			while (_ServiceStarted)
			{
				try
				{
					// if already need to check db for changes
					if (dbCheckInterval < 1)
					{
						await UpdateSubData();

						dbCheckInterval = DEFAULT_DB_CHANGES_CHECK_SECS;
					}

					// if already need to recheck connections of the devices
					if (reconnectCheckInterval < 1)
					{
						var disconnected = _Clients.Where(c => !c.Connected);
						var disconnectedCount = disconnected.Count();
						if (disconnectedCount > 0)
						{
							_Logger.WriteLine(5, $"Found {disconnectedCount} disconnected client(s)!");
						}
						foreach (var client in disconnected)
						{
							// reconnecting and re-subscribing
							await client.MakeSureConnectedAndSubscribedAsync();
						}

						reconnectCheckInterval = DEFAULT_RECONNECT_CHECK_SECS;
					}

					// Updating database
					UpdateSubDatabase();
				}
				catch (Exception ex)
				{
					_Logger.WriteLine(0, ex.Message);
				}

				// For Tasks instead of Thread.Sleep
				await Task.Delay(1000, cancel).ConfigureAwait(false);

				reconnectCheckInterval--;
				dbCheckInterval--;
			} // while

			_Logger.WriteLine(4, $"{nameof(MqttSubHeartbeatAsync)} finished!\n\n");
		}

		/// <summary>
		/// Publishing processing
		/// </summary>
		/// <param name="cancel"></param>
		/// <returns></returns>
		internal async Task MqttPubHeartbeatAsync(CancellationToken cancel)
		{
			_Logger.WriteLine(4, $"{nameof(LoadBrokers)}()..");

			var dbCheckInterval = DEFAULT_DB_CHANGES_CHECK_SECS / 2; //give different delay than for Sub processing

			await Task.Delay(1000, cancel).ConfigureAwait(false);

			while (_ServiceStarted)
			{
				try
				{
					// if already need to check db for changes
					if (dbCheckInterval < 1)
					{
						var allMsgs = _DbPub.MqttMessagesUnprocessed();

						foreach (var client in _Clients)
						{
							var cMsgs = allMsgs.Where(m =>
								client.Broker.Devices.Any(d => d.Id == m.DeviceId));

							if (cMsgs.Count() > 0)
							{
								var connected = await client.MakeSureConnectedAndSubscribedAsync();

								foreach (var cMsg in cMsgs)
								{
									await client.Publish(cMsg.Topic, cMsg.Payload);
									_DbPub.MqttMessageProcessed(cMsg.Id, MqttMessageState.Sent);

									//await Task.Delay(20, cancel).ConfigureAwait(false);
								}
							}

						}

						dbCheckInterval = DEFAULT_DB_CHANGES_CHECK_SECS;
					}
				}
				catch (Exception ex)
				{
					_Logger.WriteLine(0, ex.Message);
				}

				// For Tasks instead of Thread.Sleep
				await Task.Delay(1000, cancel).ConfigureAwait(false);

				dbCheckInterval--;
			} // while

			_Logger.WriteLine(4, $"{nameof(MqttPubHeartbeatAsync)} finished!\n\n");
		}

		#endregion
	}
}
