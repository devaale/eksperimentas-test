using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class Audience : IAudience
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}
