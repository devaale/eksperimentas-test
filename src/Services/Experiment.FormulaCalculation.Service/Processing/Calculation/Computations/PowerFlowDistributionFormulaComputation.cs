using System;
using System.Data;

using Experiment.Core;

namespace Experiment.FormulaCalculation.Service
{
	internal sealed class PowerFlowDistributionFormulaComputation : IFormulaComputation
	{
		private const int FORMULA_ID = 2010;

		// Preserve the legacy fallback until all power-flow formulas are configured with a user-selected nordpool datapoint.
		private const int SYSTEM_WIDE_NORDPOOL_DATAPOINT_ID = 178;

		private const string EXPECTED_GLOBAL_HORIZONTAL_IRRADIANCE = "globalHorizontalIrradiance";
		private const string EXPECTED_TOTAL_DC_POWER = "totalDcPower";
		private const string EXPECTED_LOAD_POWER = "loadPower";
		private const string EXPECTED_SOC_LEVEL = "socLvl";
		private const string EXPECTED_NORDPOOL = "nordpool";

		private const decimal IRRADIANCE_DAYLIGHT_THRESHOLD = 150m;
		private const decimal POWER_SURPLUS_FACTOR = 1.05m;

		private const decimal SOC_HIGH_THRESHOLD = 95m;
		private const decimal SOC_LOW_THRESHOLD = 80m;
		private const decimal SOC_DISCHARGE_THRESHOLD = 45m;

		private const decimal NORDPOOL_VERY_LOW_THRESHOLD = 20m;
		private const decimal NORDPOOL_LOW_THRESHOLD = 40m;
		private const decimal NORDPOOL_HIGH_THRESHOLD = 60m;
		private const decimal NORDPOOL_VERY_HIGH_THRESHOLD = 80m;

		private const decimal STATE_HOLD_SELF_CONSUMPTION = 0m;
		private const decimal STATE_CHARGE_GRID_TO_100 = 1m;
		private const decimal STATE_CHARGE_GRID_TO_80_ON_CLOUDY_IF_SOC_LOW = 2m;
		private const decimal STATE_DISCHARGE_WHEN_EXPENSIVE_IF_SOC_HIGH = 3m;
		private const decimal STATE_DISCHARGE_AGGRESSIVE = 4m;
		private const decimal STATE_CHARGE_FROM_PV = 5m;
		private const decimal STATE_LIMIT_CHARGING_AT_HIGH_SOC = 6m;

		private readonly IFormulaPresetChainResolver _presetChainResolver;

		public PowerFlowDistributionFormulaComputation(IFormulaPresetChainResolver presetChainResolver)
		{
			_presetChainResolver = presetChainResolver;
		}

		public bool CanHandle(int formulaId) => formulaId == FORMULA_ID;

		public decimal Compute(FormulaCalculationContext context)
		{
			var expectedOrderByName = _presetChainResolver.GetExpectedOrderByName(context.Db, FORMULA_ID);

			bool hasGlobalHorizontalIrradiance = _presetChainResolver.TryResolveRelatedDatapointId(
				context.FormulaTable,
				expectedOrderByName,
				EXPECTED_GLOBAL_HORIZONTAL_IRRADIANCE,
				out int globalHorizontalIrradianceDatapointId);

			bool hasTotalDcPower = _presetChainResolver.TryResolveRelatedDatapointId(
				context.FormulaTable,
				expectedOrderByName,
				EXPECTED_TOTAL_DC_POWER,
				out int totalDcPowerDatapointId);

			bool hasLoadPower = _presetChainResolver.TryResolveRelatedDatapointId(
				context.FormulaTable,
				expectedOrderByName,
				EXPECTED_LOAD_POWER,
				out int loadPowerDatapointId);

			bool hasSocLevel = _presetChainResolver.TryResolveRelatedDatapointId(
				context.FormulaTable,
				expectedOrderByName,
				EXPECTED_SOC_LEVEL,
				out int socLevelDatapointId);

			_presetChainResolver.TryResolveRelatedDatapointId(
				context.FormulaTable,
				expectedOrderByName,
				EXPECTED_NORDPOOL,
				out int nordpoolDatapointId);

			if (!hasGlobalHorizontalIrradiance ||
				!hasTotalDcPower ||
				!hasLoadPower ||
				!hasSocLevel ||
				globalHorizontalIrradianceDatapointId <= 0 ||
				totalDcPowerDatapointId <= 0 ||
				loadPowerDatapointId <= 0 ||
				socLevelDatapointId <= 0)
			{
				return 0m;
			}

			var globalHorizontalIrradiance = GetDailyNonZeroAverage(context.Db, globalHorizontalIrradianceDatapointId);
			var totalDcPower = GetLastDatapointValue(context.Db, totalDcPowerDatapointId);
			var loadPower = GetLastDatapointValue(context.Db, loadPowerDatapointId);
			var socLevel = GetLastDatapointValue(context.Db, socLevelDatapointId);
			var nordpool = GetLastDatapointValue(
				context.Db,
				nordpoolDatapointId > 0
					? nordpoolDatapointId
					: SYSTEM_WIDE_NORDPOOL_DATAPOINT_ID);

			return CalculatePowerFlowDistributionResult(
				globalHorizontalIrradiance,
				totalDcPower,
				loadPower,
				socLevel,
				nordpool);
		}

		private static decimal CalculatePowerFlowDistributionResult(
			decimal globalHorizontalIrradiance,
			decimal totalDcPower,
			decimal loadPower,
			decimal socLevel,
			decimal nordpool)
		{
			bool isDaylightIrradiance = globalHorizontalIrradiance > IRRADIANCE_DAYLIGHT_THRESHOLD;
			if (HasPowerSurplus(totalDcPower, loadPower))
			{
				return socLevel < SOC_HIGH_THRESHOLD
					? STATE_CHARGE_FROM_PV
					: STATE_LIMIT_CHARGING_AT_HIGH_SOC;
			}

			if (nordpool <= NORDPOOL_VERY_LOW_THRESHOLD)
			{
				return STATE_CHARGE_GRID_TO_100;
			}

			if (nordpool <= NORDPOOL_LOW_THRESHOLD)
			{
				return ResolveLowNordpoolState(isDaylightIrradiance, socLevel);
			}

			if (nordpool >= NORDPOOL_VERY_HIGH_THRESHOLD)
			{
				return STATE_DISCHARGE_AGGRESSIVE;
			}

			if (nordpool >= NORDPOOL_HIGH_THRESHOLD)
			{
				return socLevel > SOC_DISCHARGE_THRESHOLD
					? STATE_DISCHARGE_WHEN_EXPENSIVE_IF_SOC_HIGH
					: STATE_HOLD_SELF_CONSUMPTION;
			}

			return STATE_HOLD_SELF_CONSUMPTION;
		}

		private static bool HasPowerSurplus(decimal totalDcPower, decimal loadPower)
		{
			return totalDcPower > (loadPower * POWER_SURPLUS_FACTOR);
		}

		private static decimal ResolveLowNordpoolState(bool isDaylightIrradiance, decimal socLevel)
		{
			if (isDaylightIrradiance)
			{
				return STATE_HOLD_SELF_CONSUMPTION;
			}

			return socLevel < SOC_LOW_THRESHOLD
				? STATE_CHARGE_GRID_TO_80_ON_CLOUDY_IF_SOC_LOW
				: STATE_HOLD_SELF_CONSUMPTION;
		}

		private static decimal GetDailyNonZeroAverage(Experiment.Core.BL.Data.ExpSql db, int datapointId)
		{
			if (datapointId <= 0)
			{
				return 0;
			}

			var sql = string.Format(@"
SELECT
	ISNULL(AVG(CAST([Value] AS decimal(18, 4))), 0) AS [Value]
FROM
	vwDatapointValueAdv
WHERE
	DatapointId = {0}
	AND [Date] >= DATEADD(DAY, DATEDIFF(DAY, 0, GETDATE()), 0)
	AND [Date] < DATEADD(DAY, DATEDIFF(DAY, 0, GETDATE()) + 1, 0)
	AND [Value] > 0", datapointId);

			var table = db.Query(sql);
			return ReadSingleValue(table);
		}

		private static decimal GetLastDatapointValue(Experiment.Core.BL.Data.ExpSql db, int datapointId)
		{
			if (datapointId <= 0)
			{
				return 0;
			}

			var table = db.GetLastDatapointValue(datapointId);
			return ReadSingleValue(table);
		}

		private static decimal ReadSingleValue(DataTable table)
		{
			if (table == null ||
				table.Rows.Count == 0 ||
				!table.Columns.Contains(Defaults.DB_VALUE) ||
				table.Rows[0][Defaults.DB_VALUE] == DBNull.Value)
			{
				return 0;
			}

			object rawValue = table.Rows[0][Defaults.DB_VALUE];
			if (rawValue is decimal decimalValue)
			{
				return decimalValue;
			}

			if (decimal.TryParse(rawValue.ToString(), out decimal parsed))
			{
				return parsed;
			}

			return 0;
		}
	}
}
