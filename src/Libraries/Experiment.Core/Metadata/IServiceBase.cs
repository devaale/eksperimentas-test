using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Transactions;

using Experiment.Core.Enums;
using Experiment.Core.BL.Data;

namespace Experiment.Core.Metadata{
	public interface IServiceBase
	{
		/// <summary>
		/// Service name
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Service state
		/// </summary>
		ServiceState State { get; }

		/// <summary>
		/// Service commmand line args
		/// </summary>
		string[] Args { get; }

		/// <summary>
		/// Specific service's logger
		/// </summary>
		ILogger Logger { get; }

		/// <summary>
		/// Specific service's thread
		/// </summary>
		Thread Thread { get; }

		/// <summary>
		/// Database
		/// </summary>
		ExpSql Db { get; }

		/// <summary>
		/// Start method
		/// </summary>
		/// <param name="args"></param>
		void Start(string[] args);

		/// <summary>
		/// Stop method
		/// </summary>
		void Stop();

		/// <summary>
		/// Heartbeat entry point
		/// </summary>
		void Heartbeat();
	}
}
