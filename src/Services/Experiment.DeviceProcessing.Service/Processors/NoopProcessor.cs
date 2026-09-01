using System.Threading.Tasks;

using Experiment.Core.Metadata;
using Experiment.DeviceProcessing.Service.Data;

namespace Experiment.DeviceProcessing.Service.Processors{
	internal sealed class NoopProcessor : IDeviceProcessor
	{
		private readonly ThreadStateObject _state;
		private readonly ILogger _logger;

		internal NoopProcessor(ThreadStateObject state, ILogger logger)
		{
			_state = state;
			_logger = logger;
		}

		public Task StartAsync()
		{
			var vLoc = $"{_state.DebugThreadId}/{nameof(NoopProcessor)}::{nameof(StartAsync)}";
			_logger?.WriteLine(3, $"{vLoc} Unsupported protocol for device {_state.Device?.Id}, skipping.");
			return Task.CompletedTask;
		}
	}
}
