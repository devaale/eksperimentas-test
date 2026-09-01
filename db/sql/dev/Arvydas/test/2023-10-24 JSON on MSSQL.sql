-- Part of task is to convert all possible variables to DECIMAL(18,4)
-- Including ints, decimals or bools/bits
--
-- As we see OpenJson T-SQl function has 3rd column [type] to help determine type:
--
-- 1. String
-- 2. NUmbers
-- 3. Bits or bools
--
DECLARE @json NVARCHAR(2048) = N'{
	"ai_control_mode": false,
	"automatic_control_mode": false,
	"cooler_status": false,
	"energy_current": 500.0,
	"energy_generation": 0.0,
	"energy_max": 500.0,
	"energy_usage": 2.0,
	"heater_status": true,
	"simulation_speed": "real time",
	"sunny_weather": false,
	"target_hour_lower": 17.0,
	"target_hour_upper": 23.0,
	"temp_inside": 21.984041213989259,
	"temp_outside": 20.44915199279785,
	"temp_outside_max": 23.0,
	"temp_outside_min": 16.0,
	"temp_target_away": 20.0,
	"temp_target_home": 17.0,
	"time_day": 6.0,
	"time_hour": 20.0,
	"time_minute": 11.0,
	"time_second": 30.0,
	"time_step": "second"
}';

SELECT 
	[key]
	,CASE [type]
		WHEN 1 THEN NULL
		WHEN 2 THEN CAST([value] as DECIMAL(18,4))
		WHEN 3 THEN CASE WHEN [value] = 'true' THEN 1 ELSE 0 END 
		ELSE NULL 
	END AS [value]
	,[type]
FROM (
	SELECT *
	FROM OpenJson(@json)
	WHERE type <> 1
) JSON