using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;
using Experiment.Data.Models;

namespace Website.Models
{
	[Table("tblOrderDetail")]
	public class OrderDetail
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public Guid OrderId { get; set; }
		/// <summary>
		/// Subject's User
		/// </summary>
		[ForeignKey(nameof(OrderId))]
		public virtual Order Order { get; set; }

		[Required]
		public UserLicenseType LicenseType { get; set; }

		[Required]
		public int NumMonths { get; set; }
	}
}
