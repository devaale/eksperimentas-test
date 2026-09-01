using System.Threading.Tasks;

namespace Experiment.DeviceProcessing.Service.Processors{
	internal interface IDeviceProcessor
	{
		Task StartAsync();
	}
}
