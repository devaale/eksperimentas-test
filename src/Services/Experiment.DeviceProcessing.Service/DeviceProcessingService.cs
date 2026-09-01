using System;
using System.Diagnostics;
using System.ServiceProcess;

using Experiment.Core;
using Experiment.Core.IO;
using Experiment.Core.Metadata;
using Experiment.DeviceProcessing.Service.Runtime;

namespace Experiment.DeviceProcessing.Service{
	public partial class DeviceProcessingService : ServiceBase
	{
		private const string TYPE_NAME = nameof(DeviceProcessingService);
		public const int DEFAULT_LOG_LEVEL = 5;

		private EventLog _eventLog;
		private ILogger _logger;
		private DeviceProcessingEngine _engine;

		public DeviceProcessingService()
		{
			InitializeComponent();
			ServiceName = Program.ServiceName;

			_logger = CreateLogger();
			_engine = new DeviceProcessingEngine(_logger);

			InitEventLog();
		}

		protected override void OnStart(string[] args)
		{
			var vLoc = $"{TYPE_NAME}::{nameof(OnStart)}..";
			_logger.WriteLine(3, vLoc);
			WriteToEventLog(vLoc);

			_engine.Start();
		}

		protected override void OnStop()
		{
			var msg = $"{TYPE_NAME}::{nameof(OnStop)}";
			_logger.WriteLine(3, msg);
			WriteToEventLog(msg);

			_engine.StopAsync().GetAwaiter().GetResult();
		}

		internal void StartInteractive(string[] args)
		{
			OnStart(args ?? Array.Empty<string>());
		}

		internal void StopInteractive()
		{
			OnStop();
		}

		private ILogger CreateLogger()
		{
			if (Environment.UserInteractive)
			{
				return new ConsoleLogger(
					DEFAULT_LOG_LEVEL,
					TYPE_NAME);
			}

			return new FileLogger(
				DEFAULT_LOG_LEVEL,
				Defaults.DEFAULT_LOG_FOLDER,
				TYPE_NAME);
		}

		private void InitEventLog()
		{
			try
			{
				_eventLog = new EventLog
				{
					Source = TYPE_NAME
				};
			}
			catch (Exception ex)
			{
				_logger.WriteLine(0, $"{TYPE_NAME}: Initializing of event log went wrong: {ex.Message}");
			}
		}

		private void WriteToEventLog(string msg)
		{
			if (_eventLog != null)
			{
				_eventLog.WriteEntry(msg);
			}
		}
	}
}
