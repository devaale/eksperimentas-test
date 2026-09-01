namespace Experiment.FormulaCalculation.Service
{
	internal interface IFormulaComputation
	{
		bool CanHandle(int formulaId);
		decimal Compute(FormulaCalculationContext context);
	}
}
