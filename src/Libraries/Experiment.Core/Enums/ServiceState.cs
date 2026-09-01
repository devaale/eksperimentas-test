using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Core.Enums{
	public enum ServiceState : byte
	{
		None = 0,
		Iitialized = 1,
		Started = 2,
		StopRequested = 3,
		Stopped = 4,
	}
}
