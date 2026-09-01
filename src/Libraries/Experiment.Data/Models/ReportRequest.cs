using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class ReportRequest : IReportRequest
	{
		public Guid Id { get; set; }

		public string UserId { get; set; }

		public ReportRequestType Type { get; set; }

		public string Params { get; set; }
	}
}
