using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

using Experiment.Data.Models;
using Experiment.Core;
using Microsoft.Maui.Controls;

namespace Experiment.Maui.Models{
	public class VisualAlgorithm : Algorithm
	{
		static readonly Style alarmActiveStyle = (Style)Application.Current.Resources["alarmActive"];
		static readonly Style alarmInactiveStyle = (Style)Application.Current.Resources["alarmInactive"];


		[JsonIgnore]
		public Style ActiveStateStyle
		{
			get => Status == ValueOn ? alarmActiveStyle : alarmInactiveStyle;
		}
		
	}
}

