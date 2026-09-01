using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class CheckedItem<T> : NamedDbItem<T>, IHasChecked
	{
		public bool Checked { get; set; }
	}
}
