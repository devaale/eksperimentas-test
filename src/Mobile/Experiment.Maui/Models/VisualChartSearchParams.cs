using Experiment.Core;
using Experiment.Core.Enums;
using Experiment.Core.Metadata;
using Experiment.Data.Enums;
using Experiment.Data.Models;
using Experiment.Maui.ViewModels.Devices;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using D = Experiment.Core.Data;

namespace Experiment.Maui.Models{
	public class VisualChartSearchParams : ChartSearchParams
	{
		#region Attributes
		DateRange? _CurrentDateRange;

		private List<DatapointViewModel> _SelectedDatapoints;
		private Dictionary<int, DatapointViewModel> _SelectedDatapointsDic = new Dictionary<int, DatapointViewModel>();

		#endregion

		#region Properties

		public override DatePartOrInterval MeasureUnit
		{
			get => base.MeasureUnit;
			set
			{
				base.MeasureUnit = value;

				if (value == DatePartOrInterval.Minute)
				{
					if (AggregationType != ChartAggregationType.RealValue)
					{
						AggregationType = ChartAggregationType.RealValue;
					}
				}
				else
				{
					if(AggregationType == ChartAggregationType.RealValue)
					{
						AggregationType = ChartAggregationType.AverageValue;
					}
				}
			}
		}

		public override ChartAggregationType AggregationType
		{
			get => base.AggregationType;
			set
			{
				base.AggregationType = value;

				if(value == ChartAggregationType.RealValue)
				{
					if(MeasureUnit != DatePartOrInterval.Minute)
					{
						MeasureUnit = DatePartOrInterval.Minute;
					}
				}
				else
				{
					if(MeasureUnit == DatePartOrInterval.Minute)
					{
						MeasureUnit = DatePartOrInterval.Hour;
					}
				}
			}
		}

		public override ChartValueType ValueType
		{
			get => base.ValueType;
			set
			{
				base.ValueType = value;
			}
		}

		/// <summary>
		/// Anywhere used?
		/// </summary>
		[JsonIgnore]
		public DateRange? CurrentDateRange
		{
			get => _CurrentDateRange;
			set
			{
				SetProperty(ref _CurrentDateRange, value);

				// Assign dateFrom and dateTo according to ChartSelectionDateRange Enum value
				if (_CurrentDateRange.HasValue)
				{
					IDateRangeData range;

					switch (_CurrentDateRange.Value)
					{
						default:
						case DateRange.Today:
							range = D.DateRangeData.Today;
                            MeasureUnit = DatePartOrInterval.Hour;
							break;

						case DateRange.ThisWeek:
							range = D.DateRangeData.ThisWeek;
                            MeasureUnit = DatePartOrInterval.Day;
							break;

						case DateRange.ThisMonth:
							range = D.DateRangeData.ThisMonth;
                            MeasureUnit = DatePartOrInterval.Week;
							break;

						case DateRange.ThisQuarter:
							range = D.DateRangeData.ThisQuarter;
                            MeasureUnit = DatePartOrInterval.Week;
							break;

						case DateRange.ThisYear:
							range = D.DateRangeData.ThisYear;
                            MeasureUnit = DatePartOrInterval.Month;
							break;

						case DateRange.Last24Hours:
							range = D.DateRangeData.Last24Hours;
                            AggregationType = ChartAggregationType.RealValue;
							break;

						case DateRange.Last7Days:
							range = D.DateRangeData.Last7Days;
                            MeasureUnit = DatePartOrInterval.Day;
							break;

						case DateRange.Last12Months:
							range = D.DateRangeData.Last12Months;
                            MeasureUnit = DatePartOrInterval.Month;
							break;
					}

					if (range != null)
					{
						DateFrom = range.From;
						DateTo = range.To;
					}
				}
			}
		}

		/// <summary>
		/// Graph Datapoint Selection dialogue selected datapoints
		/// </summary>
		[JsonIgnore]
		public List<DatapointViewModel> SelectedDatapoints
		{
			get => _SelectedDatapoints;
			set
			{
				SetProperty(ref _SelectedDatapoints, value);

				// Assigning Ids
				DatapointIds = (from dp in SelectedDatapoints select dp.Id).ToList();

				OnPropertyChanged(nameof(Title));
			}
		}

		[JsonIgnore]
		public IEnumerable<DatapointValue> DatapointValues { get; set; }

		/// <summary>
		/// Datapoints for main chart, where can be comparison of the same datapoint by different years and so on.
		/// </summary>
		[JsonIgnore]
		public IEnumerable<Datapoint> PopulatedDatapoints { get; set; }

		#endregion

		#region Ctor
		public VisualChartSearchParams()
			 : base()
		{
			DatapointIds = new List<int>();
			MeasureUnit = DatePartOrInterval.Minute;
			AggregationType = ChartAggregationType.RealValue;
			ValueType = ChartValueType.Value;
			ChartType = ChartType.Line;

            // Assigned as last, as can redefine some properties
            CurrentDateRange = DateRange.Today;
		}

		#endregion
	}
}
