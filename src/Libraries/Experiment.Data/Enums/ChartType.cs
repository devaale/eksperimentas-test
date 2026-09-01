using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Enums{
	public enum ChartType : byte
	{
		None = 0,
		Points = 1,
		Line = 2,
		Area = 3,
		Bar = 4,
		Pie = 5,
		Donut = 6,
	}
}
