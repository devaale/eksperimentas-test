using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IDeviceTopic
	{
		int Id { get; set; }
		int DeviceId { get; set; }
		string Topic { get; set; }
	}
}
