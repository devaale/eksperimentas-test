using System;
using System.Linq;
using System.Threading;

using Experiment.Core.IO;
using Experiment.Core.Metadata;

namespace Experiment.FormulaCalculation.Service
{
	internal sealed class FormulaCalculationHeartbeat
	{
		readonly ISysVarUpdater _sysVarUpdater;
		readonly IDatapointProvider _datapointProvider;
		readonly IDatapointProcessor _datapointProcessor;
		readonly ILogger _logger;

		public FormulaCalculationHeartbeat(
			ISysVarUpdater sysVarUpdater,
			IDatapointProvider datapointProvider,
			IDatapointProcessor datapointProcessor,
			ILogger logger)
		{
			_sysVarUpdater = sysVarUpdater;
			_datapointProvider = datapointProvider;
			_datapointProcessor = datapointProcessor;
			_logger = logger;
		}

		public void Heartbeat(Func<bool> isRunning)
		{
			var vLoc = string.Format("{0}::{1}", FormulaCalculationLogContext.TypeName, nameof(Heartbeat));
			var vStep = nameof(Heartbeat);

			while (isRunning())
			{
				_sysVarUpdater.UpdateSysVars();

				vStep = "while (_ServiceStarted)";
				_logger.WriteLine(4, string.Format("{0}, {1}", vLoc, vStep));
				var sleepCount = FormulaCalculationTiming.SleepTiles;

				try
				{
					// Get virtual datapoints
					vStep = nameof(IDatapointProvider.GetDatapoints);
					_logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));
					var datapoints = _datapointProvider.GetDatapoints();

                    vStep = nameof(IDatapointProcessor.Process);
					_logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));
					_datapointProcessor.Process(datapoints);
				}
				catch (Exception ex)
				{
					var errorMsg = string.Format(
						"{0}, Failed at: {1}, with: {2}",
						vLoc, vStep, ex.Message);
					_logger.WriteLine(0, errorMsg);
				}

				_logger.WriteLine(5, string.Format("{0}, Sleeping...", vLoc));
				while (--sleepCount > 0)
				{
					Thread.Sleep(FormulaCalculationTiming.SleepSingle);

					if (!isRunning())
						break;
				}
			}
		}
	}
}
