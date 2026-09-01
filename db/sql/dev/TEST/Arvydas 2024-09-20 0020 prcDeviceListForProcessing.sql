
ALTER PROCEDURE [dbo].[prcDeviceListForProcessing] AS BEGIN

	BEGIN TRAN

	-- Devices
	SELECT *
	FROM 
		tblDevice DEV
	WHERE (
		DEV.Protocol = 10
	OR	(
			DEV.Protocol = 100
		AND DEV.ObjectId IN (
				SELECT Id 
				FROM tblObject
				WHERE UserId IN (
					SELECT UserId
					FROM tblLicense LIC
					WHERE 
						LIC.[Type] = 3
					AND LIC.Active = 1
					AND	LIC.ValidUntil >= GETDATE()
				)
			)
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
				DEV.Protocol = 10
			OR	(
					DEV.Protocol = 100
				AND DEV.ObjectId IN (
						SELECT Id 
						FROM tblObject
						WHERE UserId IN (
							SELECT UserId
							FROM tblLicense LIC
							WHERE 
								LIC.[Type] = 3
							AND LIC.Active = 1
							AND	LIC.ValidUntil >= GETDATE()
						)
					)
				)
			)
			AND DEV._active = 1
			AND DEV._deleted = 0
			AND DEV.projectedScanTime <= DATEADD(s, 5, GETDATE()) -- 5 secs in future?
		) 
		AND DP.Alias IS NOT NULL	-- ONLY THOSE DATAPOINTS WHICH HAVE ALIAS or are real API datapoints

	COMMIT TRAN
END
GO


EXEC prcDeviceListForProcessing