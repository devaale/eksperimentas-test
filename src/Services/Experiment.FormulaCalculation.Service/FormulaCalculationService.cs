//#define FORMULA_CALC_ALWAYS_FAIL // only for DEBUG purposes
#define EVENT_LOG_REPORT_DB_ERRORS
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

using Experiment.Core;
using Experiment.Core.BL.Data;
using Experiment.Core.IO;
using Experiment.Core.Metadata;

namespace Experiment.FormulaCalculation.Service
{
	partial class FormulaCalculationService : ServiceBase
	{
		/// <summary>
		/// Default log level
		/// </summary>
		public const int DEFAULT_LOG_LEVEL = 5;

		bool _serviceStarted;

		ExpSql _db;
		ILogger _logger;
		EventLog _eventLog;

		Thread _thread;
		FormulaCalculationHeartbeat _heartbeat;

		public FormulaCalculationService()
		{
			InitializeComponent();

			_serviceStarted = false;
			if (Environment.UserInteractive)
			{
				_logger = new ConsoleLogger(
					DEFAULT_LOG_LEVEL,
					Program.ServiceName);
			}
			else
			{
				_logger = new FileLogger(
					DEFAULT_LOG_LEVEL,
					Defaults.DEFAULT_LOG_FOLDER,
					Program.ServiceName);
			}

			InitEventLog();
			_db = ExpSql.GenerateFromDefaults(_logger);

			var dbFactory = new ExpSqlFactory(_logger);
			var sysVarUpdater = new SysVarUpdater(_db, _logger);
			var datapointProvider = new DatapointProvider(dbFactory, _logger);
			var formulaCalculator = new FormulaCalculator(dbFactory, _logger);
			var datapointProcessor = new DatapointProcessor(formulaCalculator, _logger);

			_heartbeat = new FormulaCalculationHeartbeat(
				sysVarUpdater,
				datapointProvider,
				datapointProcessor,
				_logger);
		}

		void InitEventLog()
		{
			try
			{
				_eventLog = new EventLog();// "Application");
				_eventLog.Source = Program.ServiceName;
			}
			catch (Exception ex)
			{
				_logger.WriteLine(0, Program.ServiceName + ": Initializing of event log went wrong: " + ex.Message);
			}
		}

		void WriteToEventLog(string msg)
		{
			if (_eventLog != null)
			{
				_eventLog.WriteEntry(msg);
			}
		}

		protected override void OnStart(string[] args = null)
		{
			var vLoc = string.Format("{0}::{1}..", Program.ServiceName, nameof(OnStart));
			_logger.WriteLine(3, vLoc);
			WriteToEventLog(vLoc);

			_serviceStarted = true;

			_thread = new Thread(Heartbeat);
			_thread.Start();
		}

		protected override void OnStop()
		{
			var msg = string.Format("{0}::{1}\r\n\r\n\r\n", Program.ServiceName, nameof(OnStop));
			_logger.WriteLine(3, msg);
			WriteToEventLog(msg);

			_serviceStarted = false;
		}

		internal void Start()
		{
			OnStart();
		}

		internal void Heartbeat()
		{
			_heartbeat.Heartbeat(() => _serviceStarted);
		}
	}
}
