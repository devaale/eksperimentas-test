using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Core.Enums;
using Experiment.Data.Enums;

namespace Experiment.Data.Metadata{
	public interface IChartSearchParams
	{
		DateTime DateFrom { get; set; }
		DateTime DateTo { get; set; }
		List<int> DatapointIds { get; set; }
		DatePartOrInterval MeasureUnit { get; set; }
		ChartAggregationType AggregationType { get; set; }
		ChartValueType ValueType { get; set; }
		ChartType ChartType { get; set; }
	}
}
