using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using Experiment.Core;
using Experiment.Core.IO;
using Experiment.Core.Metadata;

namespace Experiment.Service{
	partial class ExperimentService : ServiceBase
	{
		#region Const
		const string TYPE_NAME = nameof(ExperimentService);
		internal const int DEFAULT_LOG_LEVEL = 5;


		#endregion

		#region Attributes
		IEnumerable<IServiceBase> _Services;
		ILogger _Logger;
		EventLog _EventLog;
		#endregion

		#region Properties
		internal bool IsConsoleMode
		{
			get { return Environment.UserInteractive; }
		}


		#endregion

		#region Ctor
		public ExperimentService()
		{
			InitializeComponent();

			if (IsConsoleMode)
			{
				_Logger = new ConsoleLogger(
					DEFAULT_LOG_LEVEL,
					TYPE_NAME);
			}
			else
			{
				_Logger = new FileLogger(
					DEFAULT_LOG_LEVEL,
					Defaults.DEFAULT_LOG_FOLDER,
					TYPE_NAME);
			}

			InitEventLog();
		}

		public ExperimentService(IEnumerable<IServiceBase> services)
			: this()
		{
			_Services = services;
		}
		#endregion

		#region Helpers
		void InitEventLog()
		{
			try
			{
				_EventLog = new EventLog();// "Application");
				_EventLog.Source = TYPE_NAME;

			}
			catch (Exception ex)
			{
				_Logger.WriteLine(0, TYPE_NAME + ": Initializing of event log went wrong: " + ex.Message);
			}
		}
		internal void WriteToEventLog(string msg)
		{
			_EventLog?.WriteEntry(msg);
		}

		#endregion

		#region Methods
		internal void Start (string[] args)
		{
			OnStart(args);
		}

		protected override void OnStart(string[] args)
		{
			var vLoc = string.Format("{0}::{1}(string[] args)", TYPE_NAME, nameof(OnStart));
			_Logger.WriteLine(3, vLoc);
			WriteToEventLog(vLoc);

			if(_Services != null)
			{
				foreach (var service in _Services)
				{
					_Logger.WriteLine(3, string.Format("{0}, Starting {1}...", vLoc, service.Name));
					service.Start(args);
				}
			}
		}

		protected override void OnStop()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(OnStop));
			_Logger.WriteLine(3, vLoc);
			WriteToEventLog(vLoc);

			if (_Services != null)
			{
				foreach (var service in _Services)
				{
					_Logger.WriteLine(3, string.Format("{0}, Stopping {1}...", vLoc, service.Name));
					service.Stop();
				}
			}
		}

		#endregion
	}
}
