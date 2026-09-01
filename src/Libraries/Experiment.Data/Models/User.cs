using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	/// <summary>
	/// User's information, how much of it needs front-end.
	/// 
	/// We don't provide emails and similar info. 
	/// </summary>
	public class User : IUser
	{
		/// <summary>
		/// User's Id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// User's Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// User's password
		/// </summary>
		public string Language { get; set; }

		/// <summary>
		/// Tokens ballance
		/// </summary>
		public int Tokens { get; set; }

		/// <summary>
		/// Blockchain Address of an user
		/// </summary>
		public string Address { get; set; }
	}
}
