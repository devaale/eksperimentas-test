using System.Collections.Generic;
using System.Data;

using Experiment.Core.BL.Data;

namespace Experiment.FormulaCalculation.Service
{
	internal interface IFormulaPresetChainResolver
	{
		IReadOnlyDictionary<string, int> GetExpectedOrderByName(ExpSql db, int formulaId);

		bool TryResolveValue(
			DataTable formulaTable,
			IReadOnlyDictionary<string, int> expectedOrderByName,
			string expectedDataPointName,
			out decimal value);

		bool TryResolveRelatedDatapointId(
			DataTable formulaTable,
			IReadOnlyDictionary<string, int> expectedOrderByName,
			string expectedDataPointName,
			out int relatedDatapointId);
	}
}
