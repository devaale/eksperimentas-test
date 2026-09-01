using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class License : ILicense
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public Guid? OrderId { get; set; }
		public UserLicenseType Type { get; set; }
		public DateTime ValidFrom { get; set; }
		public DateTime ValidUntil { get; set; }
		public bool Active { get; set; }
	}
}
