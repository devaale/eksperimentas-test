using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Core.Metadata{
	public interface ILogger
	{
		/// <summary>
		/// ILogger::LogLevel Log leve which defined message type and how it serious
		/// </summary>
		int LogLevel { get; set; }

		/// <summary>
		/// ILogger::WriteLine main logging output method
		/// </summary>
		/// <param name="logLevel"></param>
		/// <param name="msg"></param>
		void WriteLine(int logLevel, string msg);
    }
}
