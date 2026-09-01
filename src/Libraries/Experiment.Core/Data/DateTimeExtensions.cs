using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Core.Data{
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Based on https://dotnetcodr.com/2015/10/28/various-quarter-related-datetime-functions-in-c/
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="quarters"></param>
		/// <returns></returns>
		public static DateTime AddQuarters(this DateTime instance, int quarters)
		{
			return instance.AddMonths(quarters * 3);
		}

		public static int GetQuarter(this DateTime instance)
		{
			int month = instance.Month - 1;
			int month2 = Math.Abs(month / 3) + 1;
			return month2;
		}
	}
}
