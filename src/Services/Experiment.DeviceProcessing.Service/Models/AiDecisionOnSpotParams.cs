using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using Experiment.Core;
using Experiment.Data.Enums;

namespace Experiment.DeviceProcessing.Service.Models{
	/// <summary>
	/// Parameters sent to the AI decision endpoint.
	/// </summary>
	public class AiDecisionOnSpotParams
	{
		public const string JSONP_CURRENT_TEMP = "current_temp";
		public const string JSONP_AIR_TEMP_MIN = "air_temp_min";
		public const string JSONP_AIR_TEMP_MAX = "air_temp_max";
		public const string JSONP_TARGET_TEMP = "target_temp";
		public const string JSONP_CURRENT_AIR_TEMP = "current_air_temp";
		public const string JSONP_TARGET_TEMP_FIRST = "target_temp_first";
		public const string JSONP_TARGET_TEMP_SECOND = "target_temp_second";
		public const string JSONP_ABS_CURRENT_TEMP_MINUS_TARGET_TEMP = "abs_current_temp_minus_target_temp";
		public const string JSONP_CURRENT_HOUR = "current_hour";
		public const string JSONP_TARGET_HOUR_START = "target_hour_start";
		public const string JSONP_TARGET_HOUR_END = "target_hour_end";
		public const string JSONP_CURRENT_ENERGY = "current_energy";
		public const string JSONP_ENERGY_MAX_CAPACITY = "energy_max_capacity";
		public const string JSONP_ENERGY_PRODUCTION_RATE = "energy_production_rate";
		public const string JSONP_ENERGY_PRODUCTION_MAX = "energy_production_max";
		public const string JSONP_HEATER_ENERGY_CONSUMPTION_RATE = "heater_energy_consumption_rate";
		public const string JSONP_COOLER_ENERGY_CONSUMPTION_RATE = "cooler_energy_consumption_rate";
		public const string JSONP_HEATING_TEMP_INFLUENCE = "heating_temp_influence";
		public const string JSONP_COOLING_TEMP_INFLUENCE = "cooling_temp_influence";
		public const string JSONP_IS_SUNNY = "is_sunny";
		public const string JSONP_HEATING_ON = "heating_on";
		public const string JSONP_COOLING_ON = "cooling_on";
		public const string JSONP_SUNNY_DAY_CHANCE = "sunny_day_chance";
		public const string JSONP_TEMPERATURE_INCREASE_WHEN_SUNNY_CHANCE = "temperature_increase_when_sunny_chance";
		public const string JSONP_TEMPERATURE_INCREASE_WHEN_CLOUDY_CHANCE = "temperature_increase_when_cloudy_chance";

		[JsonProperty(JSONP_CURRENT_TEMP)]
		public decimal CurrentTemp { get; set; }

		[JsonProperty(JSONP_AIR_TEMP_MIN)]
		public decimal AirTempMin { get; set; }

		[JsonProperty(JSONP_AIR_TEMP_MAX)]
		public decimal AirTempMax { get; set; }

		[JsonProperty(JSONP_TARGET_TEMP)]
		public decimal TargetTemp { get; set; }

		[JsonProperty(JSONP_CURRENT_AIR_TEMP)]
		public decimal CurrentAirTemp { get; set; }

		[JsonProperty(JSONP_TARGET_TEMP_FIRST)]
		public decimal TargetTempFirst { get; set; }

		[JsonProperty(JSONP_TARGET_TEMP_SECOND)]
		public decimal TargetTempSecond { get; set; }

		[JsonProperty(JSONP_ABS_CURRENT_TEMP_MINUS_TARGET_TEMP)]
		public decimal AbsCurrentTempMinusTargetTemp { get; set; }

		[JsonProperty(JSONP_CURRENT_HOUR)]
		public decimal CurrentHour { get; set; }

		[JsonProperty(JSONP_TARGET_HOUR_START)]
		public decimal TargetHourStart { get; set; }

		[JsonProperty(JSONP_TARGET_HOUR_END)]
		public decimal TargetHourEnd { get; set; }

		[JsonProperty(JSONP_CURRENT_ENERGY)]
		public decimal CurrentEnergy { get; set; }

		[JsonProperty(JSONP_ENERGY_MAX_CAPACITY)]
		public decimal EnergyMaxCapacity { get; set; }

		[JsonProperty(JSONP_ENERGY_PRODUCTION_RATE)]
		public decimal EnergyProductionRate { get; set; }

		[JsonProperty(JSONP_ENERGY_PRODUCTION_MAX)]
		public decimal EnergyProductionMax { get; set; }

		[JsonProperty(JSONP_HEATER_ENERGY_CONSUMPTION_RATE)]
		public decimal HeaterEnergyConsumptionRate { get; set; }

		[JsonProperty(JSONP_COOLER_ENERGY_CONSUMPTION_RATE)]
		public decimal CoolerEnergyConsumptionRate { get; set; }

		[JsonProperty(JSONP_HEATING_TEMP_INFLUENCE)]
		public decimal HeatingTempInfluence { get; set; }

		[JsonProperty(JSONP_COOLING_TEMP_INFLUENCE)]
		public decimal CoolingTempInfluence { get; set; }

		[JsonProperty(JSONP_IS_SUNNY)]
		public bool IsSunny { get; set; }

		[JsonProperty(JSONP_HEATING_ON)]
		public bool HeatingOn { get; set; }

		[JsonProperty(JSONP_COOLING_ON)]
		public bool CoolingOn { get; set; }

		[JsonProperty(JSONP_SUNNY_DAY_CHANCE)]
		public decimal SunnyDayChance { get; set; }

		[JsonProperty(JSONP_TEMPERATURE_INCREASE_WHEN_SUNNY_CHANCE)]
		public decimal TemperatureIncreaseWhenSunnyChance { get; set; }

		[JsonProperty(JSONP_TEMPERATURE_INCREASE_WHEN_CLOUDY_CHANCE)]
		public decimal TemperatureIncreaseWhenCloudyChance { get; set; }

		public static AiDecisionOnSpotParams From(IEnumerable<AiDatapointInfo> datapoints)
		{
			Validation.RequireValid(datapoints, nameof(datapoints));

			var sb = new StringBuilder();
			sb.Append("{");

			foreach (var p in datapoints.Where(pl =>
				pl.Direction == ParameterDirection.Out ||
				pl.Direction == ParameterDirection.Both))
			{
				if (p.Value.HasValue || p.ValueType == DatapointSettingValueType.CurrentTime)
				{
					var value = ResolveValue(p);
					sb.Append($"\"{p.Alias}\": \"{value}\",\r\n");
				}
			}

			sb.Append("}");
			return JsonConvert.DeserializeObject<AiDecisionOnSpotParams>(sb.ToString());
		}

		private static string ResolveValue(AiDatapointInfo p)
		{
			switch (p.ValueType)
			{
				case DatapointSettingValueType.Boolean:
					return (p.Value.GetValueOrDefault() > 0).ToString();
				case DatapointSettingValueType.CurrentTime:
					return DateTime.Now.ToString(Defaults.DEFAULT_DATETIME_FORMAT);
				default:
					return p.Value.GetValueOrDefault().ToString().Replace(",", ".");
			}
		}
	}
}
