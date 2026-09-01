DROP PROCEDURE IF EXISTS [prcDeviceListForScanning]
DROP PROCEDURE IF EXISTS [prcDeviceListForProcessing]
GO

CREATE PROCEDURE [dbo].[prcDeviceListForProcessing] AS BEGIN

	BEGIN TRAN

	-- Devices
	SELECT *
	FROM 
		tblDevice DEV
	WHERE (
		DEV.Protocol IN (
			10,	-- modbus
			100	-- API
		)
	)
	AND DEV._active = 1
	AND DEV._deleted = 0
	AND DEV.projectedScanTime <= DATEADD(s, 5, GETDATE()) -- 5 secs in future?

	ORDER BY 
		DEV.projectedScanTime DESC

	-- Datapoints
	SELECT * 
	FROM
		tblDatapoint DP
	WHERE
		DP.DeviceId IN (
			SELECT Id 
			FROM tblDevice DEV
			WHERE (
				DEV.Protocol IN (
					10,	-- MODBUS
					100 -- API
				)
			)
			AND DEV._active = 1
			AND DEV._deleted = 0
			AND DEV.projectedScanTime <= GETDATE()
		) 
		AND DP.Alias IS NOT NULL	-- ONLY THOSE DATAPOINTS WHICH HAVE ALIAS or are real API datapoints

	COMMIT TRAN
END
GO

[prcDeviceListForProcessing]
GO