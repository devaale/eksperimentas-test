using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IGroupDatapoint
	{
		int Id { get; set; }
		int GroupId { get; set; }
		int DatapointId { get; set; }
	}
}
