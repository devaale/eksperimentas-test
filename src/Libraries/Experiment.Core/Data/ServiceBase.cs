using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Experiment.Core.BL.Data;
using Experiment.Core.Enums;
using Experiment.Core.IO;
using Experiment.Core.Metadata;

namespace Experiment.Core.Data{
	public abstract class ServiceBase : IServiceBase
	{
		#region Properties
		public abstract string Name { get; }
		public virtual string[] Args { get; protected set; }
		public virtual ILogger Logger { get; protected set; }
		public virtual Thread Thread { get; protected set; }
		public virtual ExpSql Db { get; protected set; }
		public virtual ServiceState State { get; protected set; } = ServiceState.None;

		#endregion

		#region Ctor
		public ServiceBase()
			: base()
		{
			if (Environment.UserInteractive)
			{
				Logger = new ConsoleLogger(
					Defaults.DEFAULT_LOG_LEVEL,
					Name);
			}
			else
			{
				Logger = new FileLogger(
					Defaults.DEFAULT_LOG_LEVEL,
					Defaults.DEFAULT_LOG_FOLDER,
					Name);
			}

			Db = ExpSql.GenerateFromDefaults(Logger);
		}

		#endregion

		#region Methods
		public virtual void Start(string[] args)
		{
			Args = args;

			// If not started yet
			if(State < ServiceState.Started)
			{
				// Setting it as Started
				State = ServiceState.Started;

				// Run it in separate thread
				// Just sending to it signals about the situation
				if(Thread == null)
				{
					Thread = new Thread(Heartbeat);
				}
				Thread?.Start();
			}
			
		}

		public virtual void Stop()
		{
			// If not stopped yet
			if(State < ServiceState.Stopped)
			{
				// Asking to stop
				State = ServiceState.StopRequested;
			}
		}

		public virtual void Heartbeat()
		{
			//while(State < ServiceState.StopRequested) { }

			// FYI: Finish everything with this state
			State = ServiceState.Stopped;
		}

		#endregion
	}
}
