using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Enums{
	public enum AlgorithmType : int
	{
		/// <summary>
		/// During exact date (from - to) and time (from - to) sets specified On state. 
		/// And at other times, sets specified status Off.
		/// 
		/// Does not send emails.
		/// </summary>
		TimeTrigger = 10,

		/// <summary>
		/// Periodically, not during specific date (from - to), 
		/// but during specific day of week and time (not a date) sets specified On state.
		/// And at other times, sets specified status Off.
		/// 
		/// Does not send emails.
		/// </summary>
		PeriodicTimeTrigger = 20,

		/// <summary>
		/// 
		/// </summary>
		Alarm = 30,

		/// <summary>
		/// 
		/// </summary>
		AlarmTrigger = 40,
	}
}
