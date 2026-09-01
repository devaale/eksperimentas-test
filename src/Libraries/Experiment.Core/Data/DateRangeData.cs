using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using Experiment.Core;
using Experiment.Core.Metadata;

namespace Experiment.Core.Data{
	/// <summary>
	/// Class which specifies range of date 
	/// </summary>
	public class DateRangeData : IDateRangeData
	{
		#region Constants
		public const int NUM_OF_QUARTER_MONTHS = 3;

		#endregion

		#region Properties
		public DateTime From { get; set; }
		public DateTime To { get; set; }

		#region DateRange Manipulations
		/// <summary>
		/// Returns Today IDateRange
		/// </summary>
		/// <returns></returns>
		public static IDateRangeData Today { get => ToDayDateRange(DateTime.Now); }

		/// <summary>
		/// Yesterday IDateRange
		/// </summary>
		public static IDateRangeData Yesterday { get => ToDayDateRange(DateTime.Now.AddDays(-1)); }

		/// <summary>
		/// Tomorrow IDateRange
		/// </summary>
		public static IDateRangeData Tomorrow { get => ToDayDateRange(DateTime.Now.AddDays(1)); }

		/// <summary>
		/// This Week IDateRange
		/// </summary>
		public static IDateRangeData ThisWeek { get => ToWeekDateRange(DateTime.Now); }

		/// <summary>
		/// Previous Week IDateRange
		/// </summary>
		public static IDateRangeData PreviousWeek { get => ToWeekDateRange(DateTime.Now.AddDays(-7)); }

		/// <summary>
		/// Next Week IDateRange
		/// </summary>
		public static IDateRangeData NextWeek { get => ToWeekDateRange(DateTime.Now.AddDays(7)); }

		/// <summary>
		/// This Month IDateRange
		/// </summary>
		public static IDateRangeData ThisMonth { get => ToMonthDateRange(DateTime.Now); }

		/// <summary>
		/// Previous Month IDateRange
		/// </summary>
		public static IDateRangeData PreviousMonth { get => ToMonthDateRange(DateTime.Now.AddMonths(-1)); }

		/// <summary>
		/// Next Month IDateRange
		/// </summary>
		public static IDateRangeData NextMonth { get => ToMonthDateRange(DateTime.Now.AddMonths(1)); }

		/// <summary>
		/// This Quarter IDateRange
		/// </summary>
		public static IDateRangeData ThisQuarter { get => ToQuarterDateRange(DateTime.Now); }

		/// <summary>
		/// Previous Quarter IDateRange
		/// </summary>
		public static IDateRangeData PreviousQuarter { get => ToQuarterDateRange(DateTime.Now.AddMonths(-NUM_OF_QUARTER_MONTHS)); }

		/// <summary>
		/// Next Quarter IDateRange
		/// </summary>
		public static IDateRangeData NextQuarter { get => ToQuarterDateRange(DateTime.Now.AddMonths(NUM_OF_QUARTER_MONTHS)); }

		/// <summary>
		/// This Year IDateRange
		/// </summary>
		public static IDateRangeData ThisYear { get => ToYearDateRange(DateTime.Now); }

		/// <summary>
		/// Previous Year IDateRange
		/// </summary>
		public static IDateRangeData PreviousYear { get => ToYearDateRange(DateTime.Now.AddYears(-1)); }

		/// <summary>
		/// Next Year IDateRange
		/// </summary>
		public static IDateRangeData NextYear { get => ToYearDateRange(DateTime.Now.AddYears(1)); }

		/// <summary>
		/// Last 24 hours IDateRange
		/// </summary>
		public static IDateRangeData Last24Hours {  get => new DateRangeData() { From = DateTime.Now.AddDays(-1), To = DateTime.Now}; }

		/// <summary>
		/// Last 7 days IDataRange
		/// </summary>
		public static IDateRangeData Last7Days { get => new DateRangeData() { From = DateTime.Now.AddDays(-7), To = DateTime.Now }; }

		/// <summary>
		/// Last 12 Months IDataRange
		/// </summary>
		public static IDateRangeData Last12Months { get => new DateRangeData() { From = DateTime.Now.AddMonths(-12), To = DateTime.Now }; }


		#endregion // DateRange Manipulations

		#endregion // Properties

		#region Static

		/// <summary>
		/// Returns given date day IDateRange
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static IDateRangeData ToDayDateRange(DateTime dt)
		{
			return new DateRangeData()
			{
				From = Tools.ToDateFrom(dt),
				To = Tools.ToDateTo(dt),
			};
		}

		/// <summary>
		/// Returns given date week IDateRange, from Monday to Sunday
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static IDateRangeData ToWeekDateRange(DateTime dt)
		{
			// Zero based week day index (Monday = 0)
			var dayOffset = DayOfWeek.Monday - dt.DayOfWeek;
			var from = Tools.ToDateFrom(dt.AddDays(dayOffset));
			var to = Tools.ToDateTo(from.AddDays(6));

			return new DateRangeData()
			{
				From = from,
				To = to,
			};
		}

		/// <summary>
		/// Returns given date Month IDateRange
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static IDateRangeData ToMonthDateRange(DateTime dt)
		{
			var from = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0);
			var to = from.AddMonths(1).AddSeconds(-1);

			return new DateRangeData()
			{
				From = from,
				To = to,
			};
		}

		public static IDateRangeData ToQuarterDateRange(DateTime dt)
		{
			// zero based year quarter index
			var numQuarter = Math.Abs((dt.Month - 1) / NUM_OF_QUARTER_MONTHS);
			var quarterMonth = 1 + (numQuarter * NUM_OF_QUARTER_MONTHS);
			var from = new DateTime(dt.Year, quarterMonth, 1, 0, 0, 0);
			var to = from.AddMonths(NUM_OF_QUARTER_MONTHS).AddSeconds(-1);

			return new DateRangeData()
			{
				From = from,
				To = to,
			};
		}

		/// <summary>
		/// Returns given date Year IDateRange
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static IDateRangeData ToYearDateRange(DateTime dt)
		{
			return new DateRangeData()
			{
				From = new DateTime(dt.Year, 1, 1, 0, 0, 0),
				To = new DateTime(dt.Year, 12, 31, 23, 59, 59),
			};
		}

		#endregion

		#region Helpers
		#endregion

		#region Static 
		public static void PrintDateRange(string msg, IDateRangeData dr)
		{
			Debug.WriteLine(string.Format("{0} to {1}, {2}",
				dr.From.ToString(Defaults.DEFAULT_DATETIME_FORMAT),
				dr.To.ToString(Defaults.DEFAULT_DATETIME_FORMAT),
				msg));
		}

		public static void Test()
		{
			//for (var i = 0; i < 12; i++) Debug.WriteLine(i + "/ " + (int)i / 3);

			// Day
			PrintDateRange(nameof(DateRangeData.Yesterday), DateRangeData.Yesterday);
			PrintDateRange(nameof(DateRangeData.Today), DateRangeData.Today);
			PrintDateRange(nameof(DateRangeData.Tomorrow), DateRangeData.Tomorrow);
			Debug.Write(Environment.NewLine);

			// Week
			PrintDateRange(nameof(DateRangeData.PreviousWeek), DateRangeData.PreviousWeek);
			PrintDateRange(nameof(DateRangeData.ThisWeek), DateRangeData.ThisWeek);
			PrintDateRange(nameof(DateRangeData.NextWeek), DateRangeData.NextWeek);
			Debug.Write(Environment.NewLine);

			// Month
			PrintDateRange(nameof(DateRangeData.PreviousMonth), DateRangeData.PreviousMonth);
			PrintDateRange(nameof(DateRangeData.ThisMonth), DateRangeData.ThisMonth);
			PrintDateRange(nameof(DateRangeData.NextMonth), DateRangeData.NextMonth);
			Debug.Write(Environment.NewLine);

			// Quarter
			PrintDateRange(nameof(DateRangeData.PreviousQuarter), DateRangeData.PreviousQuarter);
			PrintDateRange(nameof(DateRangeData.ThisQuarter), DateRangeData.ThisQuarter);
			PrintDateRange(nameof(DateRangeData.NextQuarter), DateRangeData.NextQuarter);
			Debug.Write(Environment.NewLine);

			// year
			PrintDateRange(nameof(DateRangeData.PreviousYear), DateRangeData.PreviousYear);
			PrintDateRange(nameof(DateRangeData.ThisYear), DateRangeData.ThisYear);
			PrintDateRange(nameof(DateRangeData.NextYear), DateRangeData.NextYear);
			Debug.Write(Environment.NewLine);

			// Lasts
			PrintDateRange(nameof(DateRangeData.Last24Hours), DateRangeData.Last24Hours);
			PrintDateRange(nameof(DateRangeData.Last7Days), DateRangeData.Last7Days);
			PrintDateRange(nameof(DateRangeData.Last12Months), DateRangeData.Last12Months);
			Debug.Write(Environment.NewLine);
		}


		#endregion

		#region Methods

		#endregion


	}
}
