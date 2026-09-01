using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Core{
	public class Tools
	{
		/// <summary>
		/// Returns date aligned on beginning of the day
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime ToDateFrom(DateTime date)
		{
			return new DateTime(date.Year, date.Month, date.Day);
		}

		/// <summary>
		/// Returns date aligned on end of the day (last second)
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime ToDateTo(DateTime date)
		{
			return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
		}
	}
}
