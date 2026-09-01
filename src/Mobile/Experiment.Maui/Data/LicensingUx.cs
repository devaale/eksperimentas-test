using Experiment.Data.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

using Experiment.Core;
using Experiment.Data.Enums;

namespace Experiment.Maui.Data{
	internal class LicensingUx
	{
		#region Const
		internal const int LEN_SHORT_DESC = 48;
		internal const float DEFAULT_CORNER_RADIUS = 3;

		internal static readonly Style STYLE_NORMAL_LICENSE = (Style)Application.Current.Resources["normalLicense"];
		internal static readonly Style STYLE_ADVANCED_LICENSE = (Style)Application.Current.Resources["advancedLicense"];

		internal static readonly IDictionary<UserLicenseType, PackageColors> LicenseColors = new Dictionary<UserLicenseType, PackageColors>()
		{
			{ UserLicenseType.None, new PackageColors() { Ink = Color.FromArgb("#505F6A"), Paper = Color.FromArgb("#D5DBE2"), Style = STYLE_NORMAL_LICENSE } },
			{ UserLicenseType.License1, new PackageColors() { Ink = Color.FromArgb("#073c7c"), Paper = Color.FromArgb("#bbfff9"), Style = STYLE_NORMAL_LICENSE } },
			{ UserLicenseType.License2, new PackageColors() { Ink = Color.FromArgb("#6C9F2D"), Paper = Color.FromArgb("#cefbc9"), Style = STYLE_ADVANCED_LICENSE } },
			{ UserLicenseType.License3, new PackageColors() { Ink = Color.FromArgb("#c91f1f"), Paper = Color.FromArgb("#ffe0ae"), Style = STYLE_ADVANCED_LICENSE } },
		};

		#endregion

		internal static string GetLicenseName(UserLicenseType licenseType)
		{
			return E.T(string.Format("lic{0}name", (int)licenseType));
		}
		internal static string GetLicenseDescription(UserLicenseType licenseType)
		{
			return E.T(string.Format("lic{0}desc", (int)licenseType));
		}
		internal static string GetLicenseDescriptionShort(string longDescription)
		{
			if (!string.IsNullOrEmpty(longDescription))
			{
				if (longDescription.Length > LEN_SHORT_DESC)
				{
					return String.Format("{0}...", longDescription.Substring(0, LEN_SHORT_DESC));
				}
				return longDescription;
			}
			return string.Empty;
		}
		internal static Color GetInk(UserLicenseType licenseType)
		{
			return LicenseColors[licenseType].Ink;
		}
		internal static Color GetPaper(UserLicenseType licenseType)
		{
			return LicenseColors[licenseType].Paper;
		}

		internal static Style GetStyle(UserLicenseType licenseType)
		{
			return LicenseColors[licenseType].Style;
		}

		internal static string FormatPicture(decimal price)
		{
			return string.Format("{0:0.00}{1}", price, Defaults.CURRENCY_EUR_SIGN);
		}


	}
}

