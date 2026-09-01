using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class GroupDatapoint : IGroupDatapoint
	{
		public int Id { get; set; }
		public int GroupId { get; set; }
		public int DatapointId { get; set; }
	}
}
