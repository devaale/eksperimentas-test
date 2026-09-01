using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Experiment.Core;
using Experiment.Data.Metadata;

namespace Experiment.Mqtt.Service.Models{
	public class DeviceMqtt : IDeviceMqtt
	{
		#region Attributes
		string _Url;

		#endregion

		#region Properties
		public int Id { get; set; }
		public string Name { get; set; }
		public string Url
		{
			get => _Url;
			set
			{
				_Url = value;

				if (!string.IsNullOrEmpty(_Url))
				{
					var port = Defaults.DEFAULT_MQTT_PORT;
					var nodes = _Url.ToLower().Split(':');

					if (nodes.Length > 0)
						Host = nodes[0];

					if (nodes.Length > 1)
					{
						if (!int.TryParse(nodes[1], out port))
						{
							port = Defaults.DEFAULT_MQTT_PORT;
						}
					}

					Port = port;
				}
			}
		}
		public string Username { get; set; }
		public string Password { get; set; }
		//public string Topic { get; set; }	// 2024-05-02-ag removed, moved to tblDeviceTopic
		public int Interval { get; set; }
		public DateTime? LastScanTime { get; set; }
		public DateTime ProjectedScanTime { get; set; }
		public List<DatapointMqtt> Datapoints { get; set; }
		public List<string> Topics { get; set; }
#if NOT_FIXED
		public List<string> Topics { get => Datapoints.Select(dp => dp.Topic).Distinct().ToList(); }
#endif
		public string Host { get; protected set; }
		public int Port { get; protected set; }
		#endregion

		#region Init
		public DeviceMqtt()
		{
			Port = Defaults.DEFAULT_MQTT_PORT;
		}

		#endregion

		#region Methods

		#endregion
	}
}
