ALTER PROCEDURE [dbo].[prcDeviceListForScanning] AS BEGIN

	SELECT *

	FROM 
		tblDevice D
	WHERE (
		D.Protocol IN (
			10		-- modbus
		) OR (
			-- API
			D.Protocol = 100 AND D.UnitId = 0
		)
	)
	AND D._active = 1
	AND D._deleted = 0
	AND D.projectedScanTime <= GETDATE()

END
GO

prcDeviceListForScanning
