using System;
using System.Collections.Generic;
using System.Threading;

using Experiment.Core.Enums;
using Experiment.Core.IO;
using Experiment.Core.Metadata;
using Experiment.Data.Models;

namespace Experiment.FormulaCalculation.Service
{
	internal sealed class DatapointProcessor : IDatapointProcessor
	{
		readonly IFormulaCalculator _formulaCalculator;
		readonly ILogger _logger;

		public DatapointProcessor(IFormulaCalculator formulaCalculator, ILogger logger)
		{
			_formulaCalculator = formulaCalculator;
			_logger = logger;
		}

		public void Process(IEnumerable<Datapoint> datapoints)
		{
			var vLoc = string.Format("{0}::{1}", FormulaCalculationLogContext.TypeName, nameof(Process));
			var vStep = nameof(Process);
			_logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));

			int? currentOrder = null;

			foreach (var datapoint in datapoints)
			{
				vStep = "foreach..";
				_logger.WriteLine(5, string.Format("{0}, Id={1}, Name={2}, {3}={4}",
					vLoc, datapoint.Id, datapoint.Name, nameof(Datapoint.IntervalDatepart), datapoint.IntervalDatepart));

				DateTime lastFormulaCalcTime = datapoint.LastFormulaCalcTime;
				DateTime nextFormulaCalcTime = lastFormulaCalcTime;

				// Calculate next calc time
				switch (datapoint.IntervalDatepart)
				{
					default:
						// UI not supported
						_logger.WriteLine(3, string.Format("{0}, Interval unsupported!", vLoc));
						break;

					case DatePartOrInterval.Hour:
						nextFormulaCalcTime = nextFormulaCalcTime.AddHours(1);
						break;

					case DatePartOrInterval.Day:
						nextFormulaCalcTime = nextFormulaCalcTime.AddDays(1);
						break;

					case DatePartOrInterval.Week:
						nextFormulaCalcTime = nextFormulaCalcTime.AddDays(7);
						break;

					case DatePartOrInterval.Month:
						nextFormulaCalcTime = nextFormulaCalcTime.AddMonths(1);
						break;

					case DatePartOrInterval.Quarter:
						nextFormulaCalcTime = nextFormulaCalcTime.AddMonths(4);
						break;

					case DatePartOrInterval.Year:
						nextFormulaCalcTime = nextFormulaCalcTime.AddYears(4);
						break;

				}

				// Is this time for calculation?
				if (DateTime.Now <= nextFormulaCalcTime)
				{
					// If we already have current order?
					if (currentOrder.HasValue)
					{
						// Is this datapoint order doesn't correspond current order?
						if (!datapoint.Order.Equals(currentOrder.Value))
						{
							// Means that order number incresed and we involving order step delay
							Thread.Sleep(FormulaCalculationTiming.SleepBeforeNextOrder);
						}
					}
					// Saving current datapoint order as new current order
					currentOrder = datapoint.Order;

					// Proceeding to calculation
					_formulaCalculator.Calculate(datapoint);
				}

				// Small delay
				Thread.Sleep(FormulaCalculationTiming.SleepAfterDatapointProcessing);
			} // foreach
		}
	}
}
