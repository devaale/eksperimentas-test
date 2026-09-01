using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Mqtt.Service.Models{
	public class BrokerMqtt
	{
		#region Const
		internal const string CLIENT_ID_PATTERN = "{0}_EXP_{1}";
		internal const string CLIENT_ID_WRONG = "WRONG";

		#endregion

		#region Attributes

		#endregion

		#region Properties

		public string Host { get; set; }
		public int Port { get; set; }
		public string ClientId
		{
			get
			{
				string id  = Guid.NewGuid().ToString("N");

				if (Devices.Count > 0)
				{
					id = Devices[0].Id.ToString();
				}

				return string.Format(CLIENT_ID_PATTERN, Environment.MachineName, id);
			}
		}
		public string Username { get => Devices[0].Username; }
		public string Password { get => Devices[0].Password; }
		public List<DeviceMqtt> Devices { get; set; }
		public List<string> Topics { get => Devices.SelectMany(d => d.Topics).Distinct().ToList(); }

		#endregion
	}
}
