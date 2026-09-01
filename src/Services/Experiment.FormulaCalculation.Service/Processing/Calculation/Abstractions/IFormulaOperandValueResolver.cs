using System.Data;

namespace Experiment.FormulaCalculation.Service
{
	internal interface IFormulaOperandValueResolver
	{
		decimal Resolve(DataRow operand);
	}
}
