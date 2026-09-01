using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

using Experiment.Data.Models;

using Experiment.Maui.Data;

namespace Experiment.Maui.Models{
	public class VisualLicenseProduct : LicenseProduct
	{
		public string VisualName { get => LicensingUx.GetLicenseName(LicenseType); }
		public string VisualDescription { get => LicensingUx.GetLicenseDescription(LicenseType); }
		public string VisualDescriptionShort { get => LicensingUx.GetLicenseDescriptionShort(VisualDescription); }
		public string VisualPrice { get => LicensingUx.FormatPicture(Price); }
		public Color Ink { get => LicensingUx.GetInk(LicenseType); }
		public Color Paper { get => LicensingUx.GetPaper(LicenseType); }
		public Style Style { get => LicensingUx.GetStyle(LicenseType); }
	}
}

