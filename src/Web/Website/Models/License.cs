using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

using Experiment.Data.Metadata;
using Experiment.Data.Enums;

namespace Website.Models
{
	[Table("tblLicense")]
	public class License : ILicense
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(128)]
		public string UserId { get; set; }

		[ForeignKey(nameof(UserId))]
		public virtual ApplicationUser User { get; set; }

		public Guid? OrderId { get; set; }

		[Required]
		public UserLicenseType Type { get; set; }

		[Required]
		public DateTime ValidFrom { get; set; }

		[Required]
		public DateTime ValidUntil { get; set; }

		[Required]
		public bool Active { get; set; }
	}
}