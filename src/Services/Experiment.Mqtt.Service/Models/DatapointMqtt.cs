using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Experiment.Data.Metadata;

namespace Experiment.Mqtt.Service.Models{
	public class DatapointMqtt : IDatapointMqtt
	{
		public int Id { get; set; }
		public int DeviceId { get; set; }
		public string Name { get; set; }
		public string Topic { get; set; }
		public string Path { get; set; }

		/// <summary>
		/// Value of the datapoint
		/// 
		/// Comes not from database but MQTT broker
		/// </summary>
		public decimal? Value { get; set; }
	}
}
