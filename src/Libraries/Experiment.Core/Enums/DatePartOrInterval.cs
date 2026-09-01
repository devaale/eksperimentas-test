using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Core.Enums{
	//
	// Summary:
	//     Lists values used to specify the detail level for date-time values.
	public enum DatePartOrInterval : byte
	{
		None = 0,
		Millisecond = 1,
		Second = 2,
		Minute = 3,
		Hour = 4,
		Day = 5,
		Week = 6,
		Month = 7,
		Quarter = 8,
		Year = 9
	}
}
