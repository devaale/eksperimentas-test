using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Core.Metadata{
	public interface IDateRangeData
	{
		DateTime From { get; set; }
		DateTime To { get; set; }
	}
}
