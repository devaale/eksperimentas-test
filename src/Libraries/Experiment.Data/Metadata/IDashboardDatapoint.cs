using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IDashboardDatapoint
	{
		int Id { get; set; }
		string UserId { get; set; }
		byte GraphId { get; set; }
		int DatapointId { get; set; }
	}
}
