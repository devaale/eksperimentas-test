using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Enums;

namespace Experiment.Data.Metadata{
	public interface IOrderDetail
	{
		int Id { get; set; }
		Guid OrderId { get; set; }
		UserLicenseType LicenseType { get; set; }
		int NumMonths { get; set; }
	}
}
