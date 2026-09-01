using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Core.Enums{
	public enum MqttMessageState : int
	{
		New = 0,
		Sent = 1,
		Error = 3,
	}
}
