using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Experiment.Core;
using Experiment.Core.IO;
using Experiment.Core.Metadata;
using Experiment.Data.Models;

namespace Experiment.FormulaCalculation.Service
{
	internal sealed class FormulaCalculator : IFormulaCalculator
	{
		readonly IExpSqlFactory _dbFactory;
		readonly ILogger _logger;
		readonly IReadOnlyList<IFormulaComputation> _computations;
		readonly IFormulaResultWriter _resultWriter;

		public FormulaCalculator(IExpSqlFactory dbFactory, ILogger logger)
			: this(
				dbFactory,
				logger,
				CreateComputations(dbFactory, logger),
				new FormulaResultWriter(dbFactory, logger))
		{
		}

		internal FormulaCalculator(
			IExpSqlFactory dbFactory,
			ILogger logger,
			IReadOnlyList<IFormulaComputation> computations,
			IFormulaResultWriter resultWriter)
		{
			_dbFactory = dbFactory;
			_logger = logger;
			_computations = computations;
			_resultWriter = resultWriter;
		}

		public void Calculate(Datapoint datapoint)
		{
			var vLoc = string.Format("{0}::{1}(Id={2}, Name={3})",
				FormulaCalculationLogContext.TypeName, nameof(Calculate), datapoint.Id, datapoint.Name);
			var vStep = nameof(Calculate);
			_logger.WriteLine(4, string.Format("{0}, {1}..", vLoc, vStep));

			//string datapointId = datapointInfo[Defaults.DB_ID].ToString();
			//string deviceId = datapointInfo[Defaults.DB_DEVICE_ID].ToString();

			var db = _dbFactory.Create();

			try
			{
				vStep = nameof(ExpSqlFactory.Create);
				DataTable virtualDatapointFormula = db.GetDatapointFormula(datapoint.Id.ToString());

				if (!db.IsError && virtualDatapointFormula.Columns.Contains(Defaults.DB_ID))
				{
					if (!datapoint.DatapointFormulaId.HasValue)
					{
						return;
					}

					var formulaId = datapoint.DatapointFormulaId.Value;
					vStep = "ResolveComputation";
					var computation = _computations.FirstOrDefault(c => c.CanHandle(formulaId));
					if (computation == null)
					{
						return;
					}

					var context = new FormulaCalculationContext(
						datapoint,
						virtualDatapointFormula,
						db,
						vLoc);

					vStep = string.Format("{0}::{1}", computation.GetType().Name, nameof(IFormulaComputation.Compute));
					var calculationResult = computation.Compute(context);

					vStep = nameof(IFormulaResultWriter.Write);
					_resultWriter.Write(datapoint, formulaId, calculationResult);

					if (computation is IPostWriteFormulaComputation postWrite)
					{
						vStep = nameof(IPostWriteFormulaComputation.AfterWrite);
						postWrite.AfterWrite(context);
					}
				}
				else
				{
					if (!virtualDatapointFormula.Columns.Contains(Defaults.DB_ID))
					{
						_logger.WriteLine(3, string.Format("{0}, Returned data sources table has no data!",
							vLoc));
					}
				}
			}
			catch (Exception ex)
			{
				var errorMsg = string.Format(
					"{0}#{1}/Datapoint, Id={2}, Name={3}/{4}",
					vLoc, vStep,
					datapoint.Id,
					datapoint.Name,
					ex.Message);
				_logger.WriteLine(0, errorMsg);
			}
		}

		private static IReadOnlyList<IFormulaComputation> CreateComputations(
			IExpSqlFactory dbFactory,
			ILogger logger)
		{
			var operandValueResolver = new FormulaOperandValueResolver(dbFactory, logger);
			var presetChainResolver = new FormulaPresetChainResolver();

			return new IFormulaComputation[]
			{
				new EnvironmentalImpactFormulaComputation(presetChainResolver),
				new ThermalComfortFormulaComputation(presetChainResolver),
				new DepreciationFormulaComputation(),
				new PowerFlowDistributionFormulaComputation(presetChainResolver),
				new BasicMathFormulaComputation(operandValueResolver, logger),
				new VirtualFunctionFormulaComputation(),
			};
		}
	}
}
