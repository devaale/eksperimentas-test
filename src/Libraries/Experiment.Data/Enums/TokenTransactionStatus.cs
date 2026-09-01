using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Enums{
	public enum TokenTransactionStatus : byte
	{
		Unvalidated = 0,
		Valid = 1,
		Rejected = 4,
	}
}
