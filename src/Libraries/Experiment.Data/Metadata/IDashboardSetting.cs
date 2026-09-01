using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Core.Enums;
using Experiment.Data.Enums;

namespace Experiment.Data.Metadata{
	public interface IDashboardSetting
	{
		int Id { get; set; }
		string UserId { get; set; }
		int? ObjectId { get; set; }
		DateRange DateRange { get; set; }

		ChartType Graph1Type { get; set; }
		DatePartOrInterval Graph1Interval { get; set; }
		bool Graph1Difference { get; set; }
		ChartAggregationType Graph1Aggregation { get; set; }

		ChartType Graph2Type { get; set; }
		DatePartOrInterval Graph2Interval { get; set; }
		bool Graph2Difference { get; set; }
		ChartAggregationType Graph2Aggregation { get; set; }

		ChartType Graph3Type { get; set; }
		DatePartOrInterval Graph3Interval { get; set; }
		bool Graph3Difference { get; set; }
		ChartAggregationType Graph3Aggregation { get; set; }

		ChartType Graph4Type { get; set; }
		DatePartOrInterval Graph4Interval { get; set; }
		bool Graph4Difference { get; set; }
		ChartAggregationType Graph4Aggregation { get; set; }
	}
}
