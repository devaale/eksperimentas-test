using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Experiment.Core.IO{
	public class DebugLogger : LoggerBase
	{

		public DebugLogger(int logLevel)
			: base(logLevel)
		{

		}

		protected override void WriteLine(string msg)
		{
			Debug.WriteLine(msg);
		}
	}
}
