namespace Experiment.DeviceProcessing.Service.Data{
	internal enum ThreadState
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
