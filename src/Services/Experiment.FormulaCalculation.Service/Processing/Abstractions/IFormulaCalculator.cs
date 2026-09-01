using Experiment.Data.Models;

namespace Experiment.FormulaCalculation.Service
{
	internal interface IFormulaCalculator
	{
		void Calculate(Datapoint datapoint);
	}
}
