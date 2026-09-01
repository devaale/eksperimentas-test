using System;

using Experiment.Core.IO;
using Experiment.Core.Metadata;
using Experiment.Data.Models;

namespace Experiment.FormulaCalculation.Service
{
	internal sealed class FormulaResultWriter : IFormulaResultWriter
	{
		readonly IExpSqlFactory _dbFactory;
		readonly ILogger _logger;

		public FormulaResultWriter(IExpSqlFactory dbFactory, ILogger logger)
		{
			_dbFactory = dbFactory;
			_logger = logger;
		}

		public void Write(Datapoint datapoint, int datapointFormulaId, decimal calculationResult)
		{
			var vLoc = string.Format("{0}::{1}", FormulaCalculationLogContext.TypeName, nameof(Write));

			var db = _dbFactory.Create();

			// Write datapoint formula calculation result tu database
			db.CalcFormulaValueWrite(
				datapoint.DeviceId.ToString(),
				datapoint.Id.ToString(),
				calculationResult);

			// Update last formula calc time
			db.LastFormulaCalcTimeUpdate(datapoint.Id);

			_logger.WriteLine(4, string.Format("{0}, Caclulated Successfully. DatapointId: {1}, DatapointFormulaId: {2}, Calculation Result: {3}",
				vLoc,
				datapoint.Id,
				datapointFormulaId,
				calculationResult));
		}
	}
}
