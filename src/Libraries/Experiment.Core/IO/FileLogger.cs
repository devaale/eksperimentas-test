using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Experiment.Core.Metadata;

namespace Experiment.Core.IO{
	public class FileLogger : LoggerBase
	{
		#region Attributes
		string _Module;

		#endregion

		#region Properties
		public bool UseDatesInLogFileNames { get; set; }
		public string LogFolder { get; set; }

		#endregion

		#region Init
		/// <summary>
		/// Constructor of FileLogger
		/// </summary>
		/// <param name="logLevel">get or set log level at any time</param>
		/// <param name="logFolder">get or set log folder at any time</param>
		/// <param name="module">module name, which will be used as part of log file name</param>
		public FileLogger(int logLevel, string logFolder, string module)
			: base(logLevel)
		{
			Validation.RequireValidString(logFolder, "FileLogger: Log folder is empty!");
			Validation.RequireValidString(module, "FileLogger: module parameter is empty!");

			this.LogFolder = logFolder;
			this.UseDatesInLogFileNames = true;
			_Module = module;

		}

		#endregion

		protected override void WriteLine(string msg)
		{
			try
			{
				Helpers.WriteLine(GetFullLogFileName(), msg);
			}
			catch (Exception ex)
			{
				Error(ex);
				
			}
		}

		public string GetFullLogFileName()
		{
			string retVal = string.Empty;

			DirectoryInfo logDir = new DirectoryInfo(LogFolder);
			if (!logDir.Exists)
			{
				logDir.Create();
			}

			retVal += logDir.FullName;

			if (UseDatesInLogFileNames)
			{
				retVal += DateTime.Now.ToString("yyyy-MM-dd");
			}

			retVal += "_" + _Module + ".log";

			return retVal;
		}
	}
}
