using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IDatapointFormula
	{
		/// <summary>
		/// PK
		/// </summary>
		int Id { get; set; }

		/// <summary>
		/// Multilingual alias
		/// </summary>
		string Alias { get; set; }

		/// <summary>
		/// Fixed number of datapoints, which formula needs or 0 if not limited
		/// </summary>
		int NumDatapoints { get; set; }

		/// <summary>
		/// Function deals with values aggregation if true
		/// </summary>
		bool Aggregated { get; set; }
	}
}
