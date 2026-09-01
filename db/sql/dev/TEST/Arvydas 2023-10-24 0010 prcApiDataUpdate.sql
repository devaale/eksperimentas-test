DROP PROCEDURE IF EXISTS prcApiDataUpdate 
GO

CREATE PROCEDURE prcApiDataUpdate (
	@deviceId int
	,@jsonData nvarchar(max)
) AS BEGIN

	BEGIN TRAN

		PRINT 'Inserting all missing Datapoints of the device'
		INSERT INTO tblDatapoint (
			[DeviceId]
			,[Name]
			,[Alias]
			,[Multiplier]
		)
		SELECT
			@deviceId
			,[key] COLLATE SQL_Latin1_General_CP1_CI_AS as [name]
			,[key] COLLATE SQL_Latin1_General_CP1_CI_AS as [alias]
			,1 -- multiplier
		FROM 
			OpenJson(@jsonData) JSON

		WHERE 
			JSON.[type] <> 1
		AND NOT EXISTS (
				SELECT * 
				FROM tblDatapoint DP_SRC
				WHERE
					DP_SRC.[DeviceId] = @deviceId
				AND	DP_SRC.[Name] = JSON.[key] COLLATE SQL_Latin1_General_CP1_CI_AS
			)

		PRINT 'Inserting datapoint values'
		INSERT INTO tblDatapointValue (
			DatapointId
			,[Date]
			,[Value]
		)
		SELECT
			DP.Id
			,GETDATE()
			,CASE [type]
				WHEN 1 THEN NULL
				WHEN 2 THEN CAST([value] as DECIMAL(18,4))
				WHEN 3 THEN CASE WHEN [value] = 'true' THEN 1 ELSE 0 END 
				ELSE NULL 
			END * DP.Multiplier AS [value]
		FROM 
			OpenJson(@jsonData) JSON

		INNER JOIN 
			tblDatapoint DP ON 
				DP.DeviceId = @deviceId
			AND DP.Alias = JSON.[key] COLLATE SQL_Latin1_General_CP1_CI_AS

		WHERE JSON.[type] <> 1

		PRINT 'Update tblDevice.lastScanTime'
		UPDATE tblDevice
		SET lastScanTime = GETDATE()
		WHERE Id = @deviceId

	-- TESTING CODE
	--INSERT into tblDatapoint ([DeviceId], [Name]) VALUES (@deviceId, 'ABC')
	--SELECT * FROM tblDatapoint WHERE DeviceId = @deviceId
	--ROLLBACK TRAN

	COMMIT TRAN

END
GO

--------------------------------------------------------------------------------
-- TEST
--------------------------------------------------------------------------------

DECLARE @jsonData NVARCHAR(2048) = N'{
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

DECLARE @DeviceId int
SET @DeviceId = 2859

EXEC prcApiDataUpdate @DeviceId, @jsonData

-- Test of inserted data availability
--SELECT * FROM tblDevice WHERE Id = @DeviceId
--SELECT * FROM tblDatapoint WHERE DeviceId = @DeviceId ORDER BY [Name]
--SELECT * FROM tblDatapointValue WHERE DatapointId IN (SELECT Id FROM tblDatapoint WHERE DeviceId = @DeviceId) ORDER BY DatapointId, [Date]

--SELECT * FROM tblDatapoint WHERE DeviceId = 2859

-- PURGE OF TESTING DATA (USE AT YOUR OWN RISK)
--DELETE FROM tblDatapointValue WHERE DatapointId IN (SELECT Id FROM tblDatapoint WHERE DeviceId = 2859)
--DELETE FROM tblDatapoint WHERE DeviceId = 2859
--