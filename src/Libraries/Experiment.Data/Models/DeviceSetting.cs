using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Enums;

namespace Experiment.Data.Models{
	public class DeviceSetting
	{
		#region Properties
		public string Name { get; set; }
		public int UnitId { get; set; }
		public DeviceProtocol Protocol { get; set; }
		public int Interval { get; set; }
		public List<DatapointSetting> Datapoints { get; set; }
		#endregion

		#region Static
		static DeviceSetting _AiSupportDeviceSetting;
		public static DeviceSetting AiSupportDeviceSetting
		{
			get
			{
				if (_AiSupportDeviceSetting == null)
				{
					_AiSupportDeviceSetting = new DeviceSetting()
					{
						Name = "AI Support",
						UnitId = 1,
						Protocol = DeviceProtocol.API,
						Interval = 3600,
						Datapoints = new List<DatapointSetting>()
						{
							new DatapointSetting()
							{
								Name = "current_temp",
								Description = "Current temperature.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "air_temp_min",
								Description = "Minimum air temperature.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "air_temp_max",
								Description = "Maximum air temperature.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "target_temp",
								Description = "Target temperature.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "current_air_temp",
								Description = "Current air temperature.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "target_temp_first",
								Description = "Primary target temperature.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "target_temp_second",
								Description = "Secondary target temperature.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "abs_current_temp_minus_target_temp",
								Description = "Absolute difference between current and target temperature.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "current_hour",
								Description = "Current hour of day.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "target_hour_start",
								Description = "Start hour of target window.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "target_hour_end",
								Description = "End hour of target window.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "current_energy",
								Description = "Current energy.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "energy_max_capacity",
								Description = "Maximum energy capacity.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "energy_production_rate",
								Description = "Energy production rate.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "energy_production_max",
								Description = "Maximum energy production.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "heater_energy_consumption_rate",
								Description = "Heater energy consumption rate.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "cooler_energy_consumption_rate",
								Description = "Cooler energy consumption rate.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "heating_temp_influence",
								Description = "Heating temperature influence.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "cooling_temp_influence",
								Description = "Cooling temperature influence.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "is_sunny",
								Description = "Sunny weather flag.",
								ValueType = DatapointSettingValueType.Boolean,
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "heating_on",
								Description = "Heating enabled flag.",
								ValueType = DatapointSettingValueType.Boolean,
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "cooling_on",
								Description = "Cooling enabled flag.",
								ValueType = DatapointSettingValueType.Boolean,
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "sunny_day_chance",
								Description = "Chance of sunny day.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "temperature_increase_when_sunny_chance",
								Description = "Chance of temperature increase when sunny.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},
							new DatapointSetting()
							{
								Name = "temperature_increase_when_cloudy_chance",
								Description = "Chance of temperature increase when cloudy.",
								Direction = ParameterDirection.Out,
								Mandatory = true,
							},

							// Returning params

							new DatapointSetting()
							{
								Name = "decision",
								Description = "Decision made by AI: 0 - do nothing, 1 - turn on heating, 2 - turn on cooling., see Experiment.Data.Enums.TemperatureAction",
								Direction = ParameterDirection.In,
							},
						},
					};
				}
				return _AiSupportDeviceSetting;
			}
		}


		#endregion
	}
}
