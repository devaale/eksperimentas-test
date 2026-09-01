using System;
using System.Collections.Generic;

using Experiment.Core.BL.Data;
using Experiment.Core.Metadata;
using Experiment.Data.Models;

namespace Experiment.DeviceProcessing.Service.Data
{
	internal class ThreadStateObject
	{
		internal Device Device { get; set; }
		internal ExpSql Db { get; set; }
		internal ThreadState State { get; set; }
		internal ILogger Logger { get; set; }
		internal IDictionary<int, DateTime> CurrentlyProcessingDevices { get; set; }
		internal string DebugThreadId { get; set; }
	}
}
