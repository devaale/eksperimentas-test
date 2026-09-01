//#define eDebug

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;

using Experiment.Data.Enums;


namespace Experiment.Maui.Data.Converters{
	public class LicenseTypeToNameConverter : IValueConverter
	{
		const string TYPE_NAME = nameof(LicenseTypeToNameConverter);
		readonly Dictionary<UserLicenseType, string> LicenseTypes = new Dictionary<UserLicenseType, string>()
		{
			{ UserLicenseType.None, E.T("unknown") },
			{ UserLicenseType.License1, E.T("lic1desc") },
			{ UserLicenseType.License2, E.T("lic2desc") },
			{ UserLicenseType.License3, E.T("lic3desc") },
		};
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(Convert));
#if eDebug
			Debug.WriteLine(string.Format("{0}, Value is {1}", vLoc, value));
#endif

			var retVal = LicenseTypes[UserLicenseType.None];
			if (value != null && value is UserLicenseType)
			{
				retVal = LicenseTypes[(UserLicenseType)value];
			}
			return retVal;
		}

		/// <summary>
		/// This part is untested well, never probably used. 
		/// We really never needed to convert it back, so be careful with this.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(ConvertBack));
#if eDebug
			Debug.WriteLine(string.Format("{0}, Value is {1}", vLoc, value));
#endif

			var retVal = UserLicenseType.None;

			KeyValuePair<UserLicenseType, string>? kvp = LicenseTypes.First(lt => lt.Value.Equals(value));
			if(kvp.HasValue)
			{
				return kvp.Value.Key;
			}

			return retVal;
		}
	}
}

