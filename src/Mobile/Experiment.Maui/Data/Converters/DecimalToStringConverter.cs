using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Microsoft.Maui.Controls;

namespace Experiment.Maui.Data.Converters{
	public class DecimalToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return null;

			return (decimal)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return null;

			string stringValue = value as string;
			if (string.IsNullOrEmpty(stringValue))
				return null;

			decimal dcm;
			if (decimal.TryParse(stringValue, out dcm))
			{
				if (dcm == 0)
				{
					return null;
				}

				return dcm;
			}
			return null;
		}
	}
}

