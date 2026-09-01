using System;

namespace Experiment.FormulaCalculation.Service
{
	internal sealed class VirtualFunctionFormulaComputation : IFormulaComputation
	{
		public bool CanHandle(int formulaId)
		{
			return formulaId == 50 ||
				formulaId == 60 ||
				formulaId == 70 ||
				formulaId == 80 ||
				formulaId == 90 ||
				formulaId == 100;
		}

		public decimal Compute(FormulaCalculationContext context)
		{
			decimal calculationResult = 0;
			Decimal.TryParse(context.Db.VdpFunctions(context.Datapoint.Id), out calculationResult);
			return calculationResult;
		}
	}
}
