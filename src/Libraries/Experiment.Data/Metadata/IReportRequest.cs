using Experiment.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IReportRequest
	{
		Guid Id { get; set; }

		string UserId { get; set; }

		ReportRequestType Type { get; set; }

		string Params { get; set; }
	}
}
