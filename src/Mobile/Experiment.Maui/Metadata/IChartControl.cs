using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Enums;

using Experiment.Maui.Data;

namespace Experiment.Maui.Metadata{
	interface IChartControl<T>
	{
		/// <summary>
		/// Title of the Chart
		/// </summary>
		string ChartTitle { get; set; }

		/// <summary>
		/// Chart type
		/// </summary>
		ChartType ChartType { get; set; }

		/// <summary>
		/// ChartSeries
		/// </summary>
		T ChartSeries { get; set; }

	}
}
