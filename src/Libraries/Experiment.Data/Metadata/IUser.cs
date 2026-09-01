using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	/// <summary>
	/// Actual database table fields, relevant to the project.
	/// 
	/// User data is partially administered through EF.
	/// </summary>
	public interface IUser
    {
		/// <summary>
		/// Troublesome if to enable, as not all places has Id
		/// </summary>
		//string Id { get; set; }

		/// <summary>
		/// User's name
		/// </summary>
		string Name { get; set; }
		
		/// <summary>
		/// User's prefered language
		/// </summary>
		string Language { get; set; }

		/// <summary>
		/// Tokens
		/// </summary>
		int Tokens { get; set; }
	}
}
