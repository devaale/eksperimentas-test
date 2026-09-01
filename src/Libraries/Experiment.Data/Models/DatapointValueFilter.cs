using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Core.Enums;
using Experiment.Core.Metadata;

using D = Experiment.Core.Data;

namespace Experiment.Data.Models{
	public class DatapointValueFilter
	{
		#region Properties
		/// <summary>
		/// Date From 
		/// </summary>
		public DateTime? DateFrom { get; set; }

		/// <summary>
		/// Date To
		/// </summary>
		public DateTime? DateTo { get; set; }

		/// <summary>
		/// Interval, from which can be filled DateFrom and DateTo
		/// </summary>
		public DateRange DateRange { get; set; }

		/// <summary>
		/// Datapoint Ids
		/// </summary>
		public IEnumerable<int> DatapointIds { get; set; }

		#endregion

		#region Ctor

		/// <summary>
		/// Constructor
		/// </summary>
		public DatapointValueFilter ()
		{
			DateRange = DateRange.None;
		}

		#endregion

		#region Methods
		public void Parse()
		{
			if (DateRange != DateRange.None)
			{
				IDateRangeData range = null;

				switch (DateRange)
				{
					case DateRange.Today:
						range = D.DateRangeData.Today;
						break;

					case DateRange.ThisWeek:
						range = D.DateRangeData.ThisWeek;
						break;

					case DateRange.ThisMonth:
						range = D.DateRangeData.ThisMonth;
						//from = to.AddMonths(-1);
						break;

					case DateRange.ThisQuarter:
						range = D.DateRangeData.ThisQuarter;
						//from = to.AddMonths(-3);
						break;

					case DateRange.ThisYear:
						range = D.DateRangeData.ThisYear;
						//from = to.AddYears(-1);
						break;
				}

				if(range != null)
				{
					DateFrom = range.From;
					DateTo = range.To;
				}
				
			}
		}

		#endregion
	}
}
