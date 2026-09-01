using System;
using System.Collections.Generic;
using System.Data;

using Experiment.Core;
using Experiment.Core.BL.Data;
using Experiment.Data.Models;

namespace Experiment.FormulaCalculation.Service
{
	internal sealed class FormulaPresetChainResolver : IFormulaPresetChainResolver
	{
		private const string PRESET_ORDER_TABLE_NAME = "[dbo].[tblDatapointFormulaPresetChain]";

		readonly object _sync = new object();
		readonly Dictionary<int, IReadOnlyDictionary<string, int>> _expectedOrderByFormulaId =
			new Dictionary<int, IReadOnlyDictionary<string, int>>();

		public IReadOnlyDictionary<string, int> GetExpectedOrderByName(ExpSql db, int formulaId)
		{
			lock (_sync)
			{
				if (_expectedOrderByFormulaId.TryGetValue(formulaId, out IReadOnlyDictionary<string, int> cached))
				{
					return cached;
				}
			}

			var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			string sql = string.Format(
				"SELECT [Order], [ExpectedDataPointName] FROM {0} WHERE [FormulaId] = {1} ORDER BY [Order]",
				PRESET_ORDER_TABLE_NAME,
				formulaId);

			var table = db.Query(sql);
			if (table != null)
			{
				foreach (DataRow row in table.Rows)
				{
					if (row["ExpectedDataPointName"] == DBNull.Value || row["Order"] == DBNull.Value)
					{
						continue;
					}

					string expectedDataPointName = row["ExpectedDataPointName"].ToString();
					if (string.IsNullOrWhiteSpace(expectedDataPointName))
					{
						continue;
					}

					if (int.TryParse(row["Order"].ToString(), out int order))
					{
						result[expectedDataPointName] = order;
					}
				}
			}

			lock (_sync)
			{
				_expectedOrderByFormulaId[formulaId] = result;
			}

			return result;
		}

		public bool TryResolveValue(
			DataTable formulaTable,
			IReadOnlyDictionary<string, int> expectedOrderByName,
			string expectedDataPointName,
			out decimal value)
		{
			value = 0m;

			DataRow operand = FindFormulaOperand(formulaTable, expectedOrderByName, expectedDataPointName);
			if (operand == null ||
				!formulaTable.Columns.Contains(Defaults.DB_VALUE) ||
				operand[Defaults.DB_VALUE] == DBNull.Value)
			{
				return false;
			}

			return decimal.TryParse(operand[Defaults.DB_VALUE].ToString(), out value);
		}

		public bool TryResolveRelatedDatapointId(
			DataTable formulaTable,
			IReadOnlyDictionary<string, int> expectedOrderByName,
			string expectedDataPointName,
			out int relatedDatapointId)
		{
			relatedDatapointId = 0;

			DataRow operand = FindFormulaOperand(formulaTable, expectedOrderByName, expectedDataPointName);
			if (operand == null ||
				!formulaTable.Columns.Contains(Defaults.DB_RELATED_DATAPOINT_ID) ||
				operand[Defaults.DB_RELATED_DATAPOINT_ID] == DBNull.Value)
			{
				return false;
			}

			return int.TryParse(operand[Defaults.DB_RELATED_DATAPOINT_ID].ToString(), out relatedDatapointId);
		}

		private static DataRow FindFormulaOperand(
			DataTable formulaTable,
			IReadOnlyDictionary<string, int> expectedOrderByName,
			string expectedDataPointName)
		{
			if (formulaTable == null ||
				expectedOrderByName == null ||
				!formulaTable.Columns.Contains(nameof(DatapointFormulaChain.Order)) ||
				!expectedOrderByName.TryGetValue(expectedDataPointName, out int expectedOrder))
			{
				return null;
			}

			foreach (DataRow row in formulaTable.Rows)
			{
				if (row[nameof(DatapointFormulaChain.Order)] == DBNull.Value)
				{
					continue;
				}

				if (int.TryParse(row[nameof(DatapointFormulaChain.Order)].ToString(), out int order) &&
					order == expectedOrder)
				{
					return row;
				}
			}

			return null;
		}
	}
}
