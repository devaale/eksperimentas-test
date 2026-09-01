using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.DeviceScanner.Data{
	internal enum ScanThreadState
	{
		Started,
		ReadingFromDb,
		WritingToDb,
		ScanningDevice,
		Finished,
		Error,
		ConnectionError,
	}
}
