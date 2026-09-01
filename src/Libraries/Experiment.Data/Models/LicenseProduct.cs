using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Enums;

namespace Experiment.Data.Models{
	public class LicenseProduct : Product
	{
		public UserLicenseType LicenseType { get; set; }
	}
}
