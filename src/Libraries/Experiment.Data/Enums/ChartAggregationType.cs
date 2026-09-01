using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Enums{
	/// <summary>
	/// Aggregation types
	/// 
	/// @WARNING: Be careful with renaming as they are as well defined in T-SQL procedures
	/// </summary>
	public enum ChartAggregationType : byte
	{
		/// <summary>
		/// Real value without aggregation
		/// </summary>
		RealValue = 1,
		/// <summary>
		/// Aggregated minimal value
		/// </summary>
		MinimalValue = 2,
		/// <summary>
		/// Aggregated maximum value
		/// </summary>
		MaximumValue = 3,
		/// <summary>
		/// Aggregated average value
		/// </summary>
		AverageValue = 4,
		/// <summary>
		/// Sum value
		/// </summary>
		SumValue = 5,
	}
}
