using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class GroupedItem<T> : CheckedItem<T>, IHasGroup
	{
		public string Group { get; set; }
	}
}
