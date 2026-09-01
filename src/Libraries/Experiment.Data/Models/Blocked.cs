using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class Blocked : IRelatedPerson
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public string RelatedUserId { get; set; }
		public string Name { get; set; }
	}
}
