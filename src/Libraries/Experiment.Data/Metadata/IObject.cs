using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IObject
	{
		/// <summary>
		/// PK
		/// </summary>
		int Id { get; set; }
		/// <summary>
		/// 256
		/// </summary>
		string Name { get; set; }
		/// <summary>
		/// 128
		/// </summary>
		string UserId { get; set; }
		/// <summary>
		/// NULL
		/// </summary>
		DateTime? Deleted { get; set; }
	}
}
