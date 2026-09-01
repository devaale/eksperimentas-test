using System;

using Experiment.Core.Metadata;
using Experiment.Data.Enums;
using Experiment.DeviceProcessing.Service.Data;

namespace Experiment.DeviceProcessing.Service.Processors{
	internal sealed class DeviceProcessorFactory
	{
		private readonly ILogger _logger;

		internal DeviceProcessorFactory(ILogger logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		internal IDeviceProcessor Create(ThreadStateObject state)
		{
			switch (state.Device.Protocol)
			{
				case DeviceProtocol.Modbus:
					return new ModbusProcessor(state, _logger);
				case DeviceProtocol.API:
					return new ApiProcessor(state, _logger);
				default:
					return new NoopProcessor(state, _logger);
			}
		}
	}
}
