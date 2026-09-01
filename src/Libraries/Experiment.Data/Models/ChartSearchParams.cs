using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Experiment.Core;
using Experiment.Core.Base;
using Experiment.Core.Enums;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;

using Newtonsoft.Json;

namespace Experiment.Data.Models{
	public class ChartSearchParams : ViewModelBase, IChartSearchParams
	{
		#region Attributes

		protected DateTime _DateFrom;
		protected DateTime _DateTo;
		protected List<int> _DatapointIds;
		protected DatePartOrInterval _MeasureUnit;
		protected ChartAggregationType _AggregationType;
		protected ChartValueType _ValueType;
		protected ChartType _ChartType;
		protected List<int> _ComparisonYears;

		#endregion

		#region Properties

		public virtual DateTime DateFrom { get => _DateFrom; set => SetProperty(ref _DateFrom, value); }
		public virtual DateTime DateTo { get => _DateTo; set => SetProperty(ref _DateTo, value); }
		public virtual List<int> DatapointIds { get => _DatapointIds; set => SetProperty(ref _DatapointIds, value); }
		public virtual DatePartOrInterval MeasureUnit { get => _MeasureUnit; set => SetProperty(ref _MeasureUnit, value); }
		public virtual ChartAggregationType AggregationType { get => _AggregationType; set => SetProperty(ref _AggregationType, value); }
		public virtual ChartValueType ValueType { get => _ValueType; set => SetProperty(ref _ValueType, value); }
		public virtual ChartType ChartType { get => _ChartType; set => SetProperty(ref _ChartType, value); }
		public virtual List<int> ComparisonYears { get => _ComparisonYears; set => SetProperty(ref _ComparisonYears, value); }

		[JsonIgnore]
		public virtual string SqlParamDatapointIds { get => string.Join(Defaults.FIELD_SEPARATOR.ToString(), DatapointIds); }

		[JsonIgnore]
		public virtual object SqlParamComparisonYears
		{
			get
			{
				object comparison = DBNull.Value;
				if (ComparisonYears != null)
				{
					// Converting year, eg. 2020, 2022 to current year - specific year in array
					// In result We have eg. 2023-2020 = 3, 2023-2022 = 1
					// Consider than in SQL side with such value easier to calculate
					comparison = String.Join(
						Defaults.FIELD_SEPARATOR.ToString(),
						from yearOfArray in ComparisonYears
						select DateTime.Now.Year - yearOfArray);
				}
				return comparison;
			}
		}

		#endregion
	}
}
