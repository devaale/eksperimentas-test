using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Core.IO{
    public class ConsoleLogger : FileLogger
    {
		public ConsoleLogger(int logLevel, string logFolder, string module)
			: base(logLevel, logFolder, module)
		{
		}
		
        public ConsoleLogger(int logLevel, string module)
            : base(logLevel, Defaults.DEFAULT_LOG_FOLDER, module)
        {
        }

        protected override void WriteLine(string msg)
        {
            base.WriteLine(msg);

            Console.WriteLine(msg);
        }
    }
}
