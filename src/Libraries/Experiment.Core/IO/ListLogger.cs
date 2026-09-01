using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Core.IO{
	public class ListLogger : DebugLogger
	{
		public IList<string> List { get; protected set; }

		public ListLogger(int logLevel)
			: this(logLevel, new List<string>())
		{

		}

		public ListLogger(int logLevel, IList<string> list) 
			: base(logLevel)
		{
			this.List = list;
		}

		protected override void WriteLine(string msg)
		{
			base.WriteLine(msg);

			List.Add(msg);
		}
	}
}
