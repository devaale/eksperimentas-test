using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Models{
	public class PostImageNew : PostImage
	{
		#region Attributes
		string _FullPath;
		byte[] _Data;

		#endregion

		#region Properties

		/// <summary>
		/// File data as byte array 
		/// </summary>
		public byte[] Data
		{
			get
			{
				if(_Data == null && !string.IsNullOrEmpty(_FullPath))
				{
					_Data = System.IO.File.ReadAllBytes(_FullPath);
				}
				return _Data;
			}
			set
			{
				_Data = value;
			}
		}

		#endregion

		#region Ctor

		/// <summary>
		/// 
		/// </summary>
		public PostImageNew()
			: base()
		{

		}

		/// <summary>
		/// Full path is private member, as don't need to be exposed to internet transport or back-end.
		/// To the back-end will be given only binary file/image data array.
		/// While this data to device's memory will be loaded only on demand, not before it was needed, to save memory space.
		/// </summary>
		/// <param name="fullPath"></param>
		public PostImageNew(string fullPath)
			: this()
		{
			_FullPath = fullPath;
		}

		#endregion
	}
}
