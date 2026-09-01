using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IGroup
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
		int ObjectId { get; set; }
		/// <summary>
		/// 256
		/// </summary>
		DateTime? Deleted { get; set; }
	}
}
