using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IMessageMqtt
	{
		int Id { get; set; }
		int DeviceId { get; set; }
		int DeviceTopicId { get; set; }
		string Topic { get; set; }
		string Payload { get; set; }
	}
}
