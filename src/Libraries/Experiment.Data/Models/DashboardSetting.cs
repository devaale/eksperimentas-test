using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Core.Base;
using Experiment.Core.Enums;
using Experiment.Data.Enums;
using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class DashboardSetting : ViewModelBase, IDashboardSetting
	{
		#region Const
		public const ChartType DEFAULT_GRAPH_TYPE = ChartType.Line;
		public const DatePartOrInterval DEFAULT_GRAPH_INTERVAL = DatePartOrInterval.Hour;
		public const ChartAggregationType DEFAULT_GRAPH_AGGREGATION = ChartAggregationType.AverageValue;

		#endregion

		#region Attributes
		int _Id;
		string _UserId;
		int? _ObjectId;
		DateRange _DateRange;

		ChartType _Graph1Type;
		DatePartOrInterval _Graph1Interval;
		bool _Graph1Difference;
		ChartAggregationType _Graph1Aggregation;

		ChartType _Graph2Type;
		DatePartOrInterval _Graph2Interval;
		bool _Graph2Difference;
		ChartAggregationType _Graph2Aggregation;

		ChartType _Graph3Type;
		DatePartOrInterval _Graph3Interval;
		bool _Graph3Difference;
		ChartAggregationType _Graph3Aggregation;

		ChartType _Graph4Type;
		DatePartOrInterval _Graph4Interval;
		bool _Graph4Difference;
		ChartAggregationType _Graph4Aggregation;

		ICollection<DashboardDatapoint> _Datapoints;

		#endregion

		#region Properties

		public int Id { get => _Id; set => SetProperty(ref _Id, value); }
		public string UserId { get => _UserId; set => SetProperty(ref _UserId, value); }
		public int? ObjectId { get => _ObjectId; set => SetProperty(ref _ObjectId, value); }
		public DateRange DateRange { get => _DateRange; set => SetProperty(ref _DateRange, value); }

		/// <summary>
		/// Graph 1
		/// </summary>
		public ChartType Graph1Type { get => _Graph1Type; set => SetProperty(ref _Graph1Type, value); }
		public DatePartOrInterval Graph1Interval { get => _Graph1Interval; set => SetProperty(ref _Graph1Interval, value); }
		public bool Graph1Difference { get => _Graph1Difference; set => SetProperty(ref _Graph1Difference, value); }
		public ChartAggregationType Graph1Aggregation { get => _Graph1Aggregation; set => SetProperty(ref _Graph1Aggregation, value); }

		/// <summary>
		/// Graph 2
		/// </summary>
		public ChartType Graph2Type { get => _Graph2Type; set => SetProperty(ref _Graph2Type, value); }
		public DatePartOrInterval Graph2Interval { get => _Graph2Interval; set => SetProperty(ref _Graph2Interval, value); }
		public bool Graph2Difference { get => _Graph2Difference; set => SetProperty(ref _Graph2Difference, value); }
		public ChartAggregationType Graph2Aggregation { get => _Graph2Aggregation; set => SetProperty(ref _Graph2Aggregation, value); }

		/// <summary>
		/// Graph 3
		/// </summary>
		public ChartType Graph3Type { get => _Graph3Type; set => SetProperty(ref _Graph3Type, value); }
		public DatePartOrInterval Graph3Interval { get => _Graph3Interval; set => SetProperty(ref _Graph3Interval, value); }
		public bool Graph3Difference { get => _Graph3Difference; set => SetProperty(ref _Graph3Difference, value); }
		public ChartAggregationType Graph3Aggregation { get => _Graph3Aggregation; set => SetProperty(ref _Graph3Aggregation, value); }

		/// <summary>
		/// Graph 4
		/// </summary>
		public ChartType Graph4Type { get => _Graph4Type; set => SetProperty(ref _Graph4Type, value); }
		public DatePartOrInterval Graph4Interval { get => _Graph4Interval; set => SetProperty(ref _Graph4Interval, value); }
		public bool Graph4Difference { get => _Graph4Difference; set => SetProperty(ref _Graph4Difference, value); }
		public ChartAggregationType Graph4Aggregation { get => _Graph4Aggregation; set => SetProperty(ref _Graph4Aggregation, value); }

		public ICollection<DashboardDatapoint> Datapoints { get => _Datapoints; set => SetProperty(ref _Datapoints, value); }

		#endregion
	}
}
