using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Experiment.Data.Models{
	public class ApiInfo
	{
		/// <summary>
		/// "ai_control_mode": false,
		/// </summary>
		[JsonProperty(PropertyName = "ai_control_mode")]
		public bool AiControlMode { get; set; }

		/// <summary>
		/// "automatic_control_mode": false,
		/// </summary>
		[JsonProperty(PropertyName = "automatic_control_mode")]
		public bool AutomaticControlMode { get; set; }


		/// <summary>
		/// "cooler_status": false,
		/// </summary>
		[JsonProperty(PropertyName = "cooler_status")]
		public bool CoolerStatus { get; set; }

		/// <summary>
		/// "energy_current": 500.0,
		/// </summary>
		[JsonProperty(PropertyName = "energy_current")]
		public decimal EnergyCurrent { get; set; }


		/// <summary>
		/// "energy_generation": 0.0,
		/// </summary>
		[JsonProperty(PropertyName = "energy_generation")]
		public decimal EnergyGeneration { get; set; }

		/// <summary>
		/// "energy_max": 500.0,
		/// </summary>
		[JsonProperty(PropertyName = "energy_max")]
		public decimal EnergyMax { get; set; }

		/// <summary>
		/// "energy_usage": 2.0,
		/// </summary>
		[JsonProperty(PropertyName = "energy_usage")]
		public decimal EnergyUsage { get; set; }

		/// <summary>
		/// "heater_status": false,
		/// </summary>
		[JsonProperty(PropertyName = "heater_status")]
		public bool HeaterStatus { get; set; }

		/// <summary>
		/// "simulation_speed": "real time",
		/// 
		/// Unused as this is string
		/// </summary>
		[JsonProperty(PropertyName = "simulation_speed")]
		public string SimulationSpeed { get; set; }

		/// <summary>
		/// "sunny_weather": false,
		/// </summary>
		[JsonProperty(PropertyName = "sunny_weather")]
		public bool SunnyWeather { get; set; }

		/// <summary>
		/// "target_hour_lower": 17.0,
		/// </summary>
		[JsonProperty(PropertyName = "target_hour_lower")]
		public decimal TargetHourLower { get; set; }

		/// <summary>
		/// "target_hour_upper": 23.0,
		/// </summary>
		[JsonProperty(PropertyName = "target_hour_upper")]
		public decimal TargetHourUpper { get; set; }

		/// <summary>
		/// "temp_inside": 22.39253807067871,
		/// </summary>
		[JsonProperty(PropertyName = "temp_inside")]
		public decimal TempInside { get; set; }

		/// <summary>
		/// "temp_outside": 22.070621490478517,
		/// </summary>
		[JsonProperty(PropertyName = "temp_outside")]
		public decimal TempOutside { get; set; }

		/// <summary>
		/// "temp_outside_max": 23.0,
		/// </summary>
		[JsonProperty(PropertyName = "temp_outside_max")]
		public decimal TempOutsideMax { get; set; }

		/// <summary>
		/// "temp_outside_min": 16.0,
		/// </summary>
		[JsonProperty(PropertyName = "temp_outside_min")]
		public decimal TempOutsideMin { get; set; }

		/// <summary>
		/// "temp_target_away": 20.0,
		/// </summary>
		[JsonProperty(PropertyName = "temp_target_away")]
		public decimal TempTargetAway { get; set; }

		/// <summary>
		/// "temp_target_home": 17.0,
		/// </summary>
		[JsonProperty(PropertyName = "temp_target_home")]
		public decimal TempTargetHome { get; set; }

		/// <summary>
		/// "time_day": 6.0,
		/// </summary>
		[JsonProperty(PropertyName = "time_day")]
		public decimal TimeDay { get; set; }

		/// <summary>
		/// "time_hour": 18.0,
		/// </summary>
		[JsonProperty(PropertyName = "time_hour")]
		public decimal TimeHour { get; set; }

		/// <summary>
		/// "time_minute": 47.0,
		/// </summary>
		[JsonProperty(PropertyName = "time_minute")]
		public decimal TimeMinute { get; set; }

		/// <summary>
		/// "time_second": 6.0,
		/// </summary>
		[JsonProperty(PropertyName = "time_second")]
		public decimal TimeSecond { get; set; }

		/// <summary>
		/// "time_step": "second"
		/// 
		/// Unused as this is string
		/// </summary>
		[JsonProperty(PropertyName = "time_step")]
		public string TimeStep { get; set; }
	}
}
