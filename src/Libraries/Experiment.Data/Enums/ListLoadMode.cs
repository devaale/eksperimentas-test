using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Enums{
	public enum ListLoadMode : byte
	{
		/// <summary>
		/// Full reload from 0 or FULL refresh.
		/// </summary>
		Full = 0,

		/// <summary>
		/// Check only for new data by firstDate, which will be added at the beginning of already loaded list
		/// </summary>
		Newest = 1,

		/// <summary>
		/// Scroll down, load of older items, which will be added at the bottom
		/// </summary>
		Older = 2,
	}
}
