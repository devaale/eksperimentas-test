using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IDatapointMqtt
	{
		int DeviceId { get; set; }
		string Topic { get; set; }
		string Path { get; set; }
		decimal? Value { get; set; }
	}
}
