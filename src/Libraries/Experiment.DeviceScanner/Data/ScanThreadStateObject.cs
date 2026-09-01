using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Experiment.Core.BL.Data;
using Experiment.Core.Metadata;

namespace Experiment.DeviceScanner.Data{
	internal class ScanThreadStateObject
	{
		internal ScanDeviceInfo Device;
		internal ExpSql Db;
		internal ScanThreadState State;
		internal ILogger Logger;
		internal Dictionary<string, DateTime> CurrentlyScanningDevices;
		internal string DebugThreadId;
	}
}
