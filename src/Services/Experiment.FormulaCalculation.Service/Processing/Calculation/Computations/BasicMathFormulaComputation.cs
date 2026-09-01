using System;
using System.Data;

using Experiment.Core.IO;
using Experiment.Core.Metadata;

namespace Experiment.FormulaCalculation.Service
{
	internal sealed class BasicMathFormulaComputation : IFormulaComputation
	{
		private readonly IFormulaOperandValueResolver _operandValueResolver;
		private readonly ILogger _logger;

		public BasicMathFormulaComputation(IFormulaOperandValueResolver operandValueResolver, ILogger logger)
		{
			_operandValueResolver = operandValueResolver;
			_logger = logger;
		}

		public bool CanHandle(int formulaId)
		{
			return formulaId == 10 ||
				formulaId == 20 ||
				formulaId == 30 ||
				formulaId == 40;
		}

		public decimal Compute(FormulaCalculationContext context)
		{
			var formulaId = context.Datapoint.DatapointFormulaId.Value;

			// Get first formula operand value
			decimal calculationResult = _operandValueResolver.Resolve(context.FormulaTable.Rows[0]);

			int count = 0;
			foreach (DataRow operand in context.FormulaTable.Rows)
			{
				if (count == 0)
				{
					// Do nothing
				}
				else
				{
					// Get next operand value
					var nextOperantValue = _operandValueResolver.Resolve(operand);

					// Avoid divide by zero errors
					if (formulaId == 20 && nextOperantValue == 0)
					{
						calculationResult = 0;
						_logger.WriteLine(5, string.Format(
							"{0}, Divide by zero error, DatapointId: {1}",
							context.CalculationLocation,
							context.Datapoint.Id));
						break;
					}

					// Calculate formula
					switch (formulaId)
					{
						case 10:
							calculationResult = calculationResult * nextOperantValue;
							Console.WriteLine("Multiplication");
							break;

						case 20:
							calculationResult = calculationResult / nextOperantValue;
							Console.WriteLine("Division");
							break;

						case 30:
							calculationResult = calculationResult + nextOperantValue;
							Console.WriteLine("Addition");
							break;

						case 40:
							calculationResult = calculationResult - nextOperantValue;
							Console.WriteLine("Substraction");
							break;

						default:
							Console.WriteLine("Unkwnown operation");
							break;
					}
				}
				count++;
			}

			return calculationResult;
		}
	}
}
