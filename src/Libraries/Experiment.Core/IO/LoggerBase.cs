using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Experiment.Core.Metadata;

namespace Experiment.Core.IO{
	public abstract class LoggerBase : ErrorInfo, ILogger
	{
		#region Const

		public const int DEFAULT_LOGLEVEL_FOR_DB_ERRORS = 0;

		const string DEFAULT_LOG_FORMAT = "[{0}] {1}";
		const bool SHOW_DATETIME = true;

		const string MSG_LOG_LEVEL = "Current log level is: {0}";

		#endregion

		#region Attributes
		bool _LogLevelReported = false;
		int _LogLevel;

		#endregion

		#region Properties
		public int LogLevel
		{
			get => _LogLevel;
			set
			{
				var changed = _LogLevel != value;
				_LogLevel = value;

				if (changed || !_LogLevelReported)
				{
					WriteLogLevel();
				}
			}
		}

		#endregion

		public LoggerBase (int logLevel)
		{
			this.LogLevel = logLevel;
		}

		protected abstract void WriteLine(string msg);

		public void WriteLine(int logLevel, string msg)
		{
			if(!_LogLevelReported)
			{
				WriteLogLevel();
			}

			if (logLevel <= this.LogLevel)
			{

				WriteLine(
					(SHOW_DATETIME ? DateTime.Now.ToString(Defaults.DEFAULT_DATETIME_FORMAT) : "") +"> " +
					string.Format(DEFAULT_LOG_FORMAT, logLevel, msg));
			}
		}

		public void WriteLogLevel()
		{
			_LogLevelReported = true;

			WriteLine(1, string.Format(MSG_LOG_LEVEL, LogLevel));
		}
	}
}
