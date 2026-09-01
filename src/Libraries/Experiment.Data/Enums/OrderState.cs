using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Enums{
	public enum OrderState : byte
	{
		Invalid = 0,
		Posted = 1,
		Paid = 2,
		Rejected = 4,
	}
}
