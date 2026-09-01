using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class DeviceTopic : IDeviceTopic
	{
		public int Id { get; set; }
		public int DeviceId { get; set; }
		public string Topic { get; set; }
	}
}
