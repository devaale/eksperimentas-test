using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Experiment.Core.Metadata;

namespace Experiment.Core{
	public abstract class ErrorInfo : IErrorInfo
	{

		#region Attributes
		protected Exception _ErrorException;

		#endregion

		#region Properties

		public bool IsError { get { return _ErrorException != null; } }

		public string ErrorMsg
		{
			get
			{
				if (IsError)
				{
					return _ErrorException.Message;
				}
				else
				{
					return null;
				}
			}
		}

		public Exception ErrorException { get { return _ErrorException; } }

		#endregion

		#region CTOR
		public ErrorInfo()
		{
			Error();
		}

		#endregion

		#region Helpers

		#endregion

		#region Methods
		/// <summary>
		/// Resets the error status
		/// </summary>
		public void Error()
		{
			_ErrorException = null;
		}

		/// <summary>
		/// Sets the error message and fake exception
		/// </summary>
		/// <param name="msg"></param>
		public void Error(string msg)
		{
			_ErrorException = new Exception(msg);
		}

		/// <summary>
		/// Sets the error exception and message
		/// </summary>
		/// <param name="ex"></param>
		public void Error(Exception ex)
		{
			_ErrorException = ex;
		}

		public void DebugOut(string msg)
		{
			if(IsError)
			{
				Debug.WriteLine(
					(string.IsNullOrEmpty(msg) ? String.Empty : "["+msg+"] ") + 
					_ErrorException.Message + Environment.NewLine + 
					_ErrorException.StackTrace);
			}
		}

		/// <summary>
		/// Show Exception debug output
		/// </summary>
		public void DebugOut()
		{
			if(IsError ) { 

				Debug.WriteLine(
					ErrorException.Message + 
					Environment.NewLine + 
					Environment.NewLine + 
					ErrorException.StackTrace);
			}
			
		}
		#endregion

		#region Junk

		#endregion
	}
}
