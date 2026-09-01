using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Core.Models{
	public class MessageMqtt
	{
		public int Id { get; set; }
		public int DeviceId { get; set; }
		public int DeviceTopicId { get; set; }
		public string Topic { get; set; }
		public string Payload { get; set; }
	}
}
