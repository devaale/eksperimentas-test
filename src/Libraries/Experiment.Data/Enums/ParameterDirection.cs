using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Enums{
	/// <summary>
	/// Directin of AI devices parameters/datapoints
	/// </summary>
	public enum ParameterDirection : byte
	{
		/// <summary>
		/// All bits 0
		/// </summary>
		None = 0,

		/// <summary>
		/// Bitwise, lowest bit
		/// </summary>
		In = 1,

		/// <summary>
		/// Bitwise, the second bit
		/// </summary>
		Out = 2,

		/// <summary>
		/// Both bits - In and Out is on.
		/// </summary>
		Both = 3
	}
}
