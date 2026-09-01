using Experiment.Data.Models;

namespace Experiment.FormulaCalculation.Service
{
	internal interface IFormulaResultWriter
	{
		void Write(Datapoint datapoint, int datapointFormulaId, decimal calculationResult);
	}
}
