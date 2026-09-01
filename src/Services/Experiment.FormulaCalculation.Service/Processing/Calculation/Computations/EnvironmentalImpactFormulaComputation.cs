using System;
using System.Data;

using Experiment.Core;

namespace Experiment.FormulaCalculation.Service
{
	internal sealed class EnvironmentalImpactFormulaComputation : IFormulaComputation
	{
		private const int FORMULA_ID = 1010;
		private const string EXPECTED_ELEMENT0 = "element0";
		private const string EXPECTED_ELEMENT1 = "element1";
		private const string EXPECTED_RELATED_DATAPOINT = "relatedDatapoint";

		private readonly IFormulaPresetChainResolver _presetChainResolver;

		public EnvironmentalImpactFormulaComputation(IFormulaPresetChainResolver presetChainResolver)
		{
			_presetChainResolver = presetChainResolver;
		}

		public bool CanHandle(int formulaId) => formulaId == FORMULA_ID;

		public decimal Compute(FormulaCalculationContext context)
		{
			var expectedOrderByName = _presetChainResolver.GetExpectedOrderByName(context.Db, FORMULA_ID);
			if (!_presetChainResolver.TryResolveValue(context.FormulaTable, expectedOrderByName, EXPECTED_ELEMENT0, out decimal element0) ||
				!_presetChainResolver.TryResolveValue(context.FormulaTable, expectedOrderByName, EXPECTED_ELEMENT1, out decimal element1) ||
				!_presetChainResolver.TryResolveRelatedDatapointId(context.FormulaTable, expectedOrderByName, EXPECTED_RELATED_DATAPOINT, out int relatedDatapointId))
			{
				return 0m;
			}

			decimal datapointValue = GetDatapointValue(context.Db.GetLastDatapointValue(relatedDatapointId));
			decimal calculationResult = 0;

			// SQL function
			Decimal.TryParse(context.Db.CalculateEnvironmentalImpact(element0, datapointValue, element1), out calculationResult);
			return calculationResult;
		}

		private static decimal GetDatapointValue(DataTable table)
		{
			if (table == null ||
				table.Rows.Count == 0 ||
				!table.Columns.Contains(Defaults.DB_VALUE) ||
				table.Rows[0][Defaults.DB_VALUE] == DBNull.Value)
			{
				return 0m;
			}

			Decimal.TryParse(table.Rows[0][Defaults.DB_VALUE].ToString(), out decimal datapointValue);
			return datapointValue;
		}
	}
}
