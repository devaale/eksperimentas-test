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
	[Table("tblReportRequest")]
	public class ReportRequest : IReportRequest
	{
		[Key]
		public Guid Id { get; set; }

		[Required]
		[StringLength(128)]
		public string UserId { get; set; }

		// How need to declare it but then it starting to annoy with plenty of incorrect structures of Asp.Net tables, so better to comment it.
		[ForeignKey(nameof(UserId))]
		public virtual ApplicationUser User { get; set; }

		[Required]
		[DefaultValue(ReportRequestType.None)]
		public ReportRequestType Type { get; set; }

		[Required]
		public string Params { get; set; }
	}
}
