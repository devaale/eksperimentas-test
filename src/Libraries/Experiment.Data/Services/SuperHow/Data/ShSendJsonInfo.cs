using Experiment.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Services.SuperHow.Data{
	public class ShSendJsonInfo
	{
		public Guid Id { get; set; }

		public string UserId { get; set; }

		public string OrderNo { get; set; }

		public decimal FullPrice { get; set; }

		public int UsedTokens { get; set; }

		public decimal Discount { get; set; }

		public decimal FinalPrice { get; set; }

		public string PaymentMethodId { get; set; }

		public DateTime Posted { get; set; }

		public UserLicenseType LicenseType { get; set; }
		public int NumMonths { get; set; }
	}
}
