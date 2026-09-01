using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class DatapointSelection : Datapoint, ISelectable
	{
		public bool Selected { get; set; }
	}
}
