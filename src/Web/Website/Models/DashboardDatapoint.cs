using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;

namespace Website.Models
{
	[Table("tblDashboardDatapoint")]
	public class DashboardDatapoint : IDashboardDatapoint
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(128)]
		public string UserId { get; set; }

		public int? ObjectId { get; set; }

		[ForeignKey(nameof(UserId))]
		public virtual ApplicationUser User { get; set; }

		[Required]
		public byte GraphId { get; set; }

		[Required]
		public int DatapointId { get; set; }

		[ForeignKey(nameof(DatapointId))]
		public virtual Datapoint Datapoint { get; set; }

	}
}
