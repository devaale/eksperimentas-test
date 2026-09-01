using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

using Experiment.Core;
using Experiment.Data.Enums;
using Experiment.Data.Models;

using Experiment.Maui.Data;

namespace Experiment.Maui.Models{
	public class VisualLicense : License
	{
		public string VisualName { get => LicensingUx.GetLicenseName(Type); }
		public string VisualDescription { get => LicensingUx.GetLicenseDescription(Type); }
		public string VisualDescriptionShort { get => LicensingUx.GetLicenseDescriptionShort(VisualDescription); }
		public Color Ink { get => LicensingUx.GetInk(Type); }
		public Color Paper { get => LicensingUx.GetPaper(Type); }
		public Style Style { get => LicensingUx.GetStyle(Type); }

		public bool HasValidFrom { get => Type != UserLicenseType.License1; }
		public string VisualValidFrom
		{
			get
			{
				switch(Type)
				{
					case UserLicenseType.License1:
						return "";

					default:
						return E.T("validFrom") + ": " + ValidFrom.ToString(Defaults.DEFAULT_DATE_FORMAT);
				}
			}
		}
		public string VisualValidUntil
		{
			get
			{
				switch (Type)
				{
					case UserLicenseType.License1:
						return E.T("freeLicense");

					default:
						return E.T("validUntil") + ": " + ValidUntil.ToString(Defaults.DEFAULT_DATE_FORMAT);
				}
			}
		}

		internal static VisualLicense FromLicense(License license)
		{
			return new VisualLicense()
			{
				Id = license.Id,
				UserId = license.UserId,
				Type = license.Type,
				Active = license.Active,
				ValidFrom = license.ValidFrom,
				ValidUntil = license.ValidUntil,
			};
		}
	}
}

