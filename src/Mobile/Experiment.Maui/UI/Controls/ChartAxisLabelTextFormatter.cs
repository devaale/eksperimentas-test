using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using DevExpress.Maui.Charts;

using Experiment.Core;

namespace Experiment.Maui.UI.Controls{
	public class ChartAxisLabelTextFormatter : IAxisLabelTextFormatter
	{
		public string Format(object axisValue)
		{
			string retVal = string.Empty;
			if(axisValue is DateTime)
			{
				var value = (DateTime)axisValue;
				Debug.WriteLine("ChartAxisLabelTextFormatter: " + value.ToString(Defaults.DEFAULT_DATETIME_FORMAT));
				return value.ToString(Defaults.DEFAULT_DATETIME_FORMAT);
				//return ((value.Day >= 1) && (value.Day <= 7)) ? $"{value.ToString("MMM, d")}" : $"{value.Day}";
			}
			return retVal;
		}
	}
}

