using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Core{
	public class ExpException : Exception
	{
		public ExpException(string message)
			: base(message)
		{
			Debug.WriteLine("ExpException: " + this.Message);
		}

		public ExpException(string message, Exception innerException)
			: base(message, innerException)
		{
			Debug.WriteLine("ExpException: " + this.Message);
		}

		public ExpException(Exception innerException)
			: base(innerException.Message, innerException)
		{
			Debug.WriteLine("ExpException: " + this.Message);
		}
	}
}
