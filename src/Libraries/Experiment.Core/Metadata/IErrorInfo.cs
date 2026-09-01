using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Core.Metadata{
	public interface IErrorInfo
	{
		bool IsError { get; }
		string ErrorMsg { get; }
		Exception ErrorException { get; }

		void Error();
		void Error(string msg);
		void Error(Exception ex);

		void DebugOut();

		void DebugOut(string msg);
	}
}
