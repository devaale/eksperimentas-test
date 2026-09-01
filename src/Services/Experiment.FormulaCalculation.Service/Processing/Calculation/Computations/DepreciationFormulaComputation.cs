using System;

namespace Experiment.FormulaCalculation.Service
{
	internal sealed class DepreciationFormulaComputation : IFormulaComputation, IPostWriteFormulaComputation
	{
		private const int FORMULA_ID = 1030;

		public bool CanHandle(int formulaId) => formulaId == FORMULA_ID;

		public decimal Compute(FormulaCalculationContext context)
		{
			decimal calculationResult = 0;
			Decimal.TryParse(context.Db.CalculateDepreciation(context.Datapoint.DeviceId), out calculationResult);
			return calculationResult;
		}

		public void AfterWrite(FormulaCalculationContext context)
		{
			context.Db.UpdateDeprA(
				context.Datapoint.DeviceId,
				((int)context.Datapoint.IntervalDatepart).ToString());
		}
	}
}
