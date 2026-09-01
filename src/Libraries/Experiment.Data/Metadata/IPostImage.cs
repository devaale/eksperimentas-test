using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IPostImage
	{
		/// <summary>
		/// Primary key, Id
		/// </summary>
		Guid Id { get; set; }

		/// <summary>
		/// Post Id
		/// </summary>
		int PostId { get; set; }

		/// <summary>
		/// Image content type/META
		/// </summary>
		string ContentType { get; set; }

		/// <summary>
		/// Image file name
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Image file URL, internet accessible supposed
		/// </summary>
		string RawName { get; set; }
	}
}
