using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class DatapointFormula : IDatapointFormula
	{
		public int Id { get; set; }
		/// <summary>
		/// Ml Alias
		/// </summary>
		public string Alias { get; set; }
		public int NumDatapoints { get; set; }
		public bool Aggregated { get; set; }

		/// <summary>
		/// Formula name
		/// </summary>
		public virtual string Name { get; set;  }
	}
}
