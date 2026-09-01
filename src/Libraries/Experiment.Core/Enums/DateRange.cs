using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Core.Metadata;

namespace Experiment.Core.Enums{
	/// <summary>
	/// Chart Selection Date Range
	/// 
	/// Compatible with DatePartOrInterval.
	/// </summary>
	public enum DateRange : byte
	{
		None = (byte)DatePartOrInterval.None,

		Today = (byte)DatePartOrInterval.Day,
		Yesterday = 20 + (byte)DatePartOrInterval.Day,
		Tomorrow = 40 + (byte)DatePartOrInterval.Day,

		ThisWeek = (byte)DatePartOrInterval.Week,
		PreviousWeek = 20 + (byte)DatePartOrInterval.Week,
		NextWeek = 40 + (byte)DatePartOrInterval.Week,

		ThisMonth = (byte)DatePartOrInterval.Month,
		PreviousMonth = 20 + (byte)DatePartOrInterval.Month,
		NextMonth = 40 + (byte)DatePartOrInterval.Month,

		ThisQuarter = (byte)DatePartOrInterval.Quarter,
		PreviousQuarter = 20 + (byte)DatePartOrInterval.Quarter,
		NextQuarter = 40 + (byte)DatePartOrInterval.Quarter,

		ThisYear = (byte)DatePartOrInterval.Year,
		PreviousYear = 20 + (byte)DatePartOrInterval.Year,
		NextYear = 40 + (byte)DatePartOrInterval.Year,

		Last24Hours = 50 + (byte)DatePartOrInterval.Day,
		Last7Days = 50 + (byte)DatePartOrInterval.Week,
		Last12Months = 50 + (byte)DatePartOrInterval.Year,
	}
}
