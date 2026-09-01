using Experiment.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Models{
	public class OrderDetail
	{
		public int Id { get; set; }
		public Guid OrderId { get; set; }
		public UserLicenseType LicenseType { get; set; }
		public int NumMonths { get; set; }
	}
}
