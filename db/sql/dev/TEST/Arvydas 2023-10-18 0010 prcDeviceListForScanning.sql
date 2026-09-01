ALTER PROCEDURE [dbo].[prcDeviceListForScanning] AS BEGIN

	SELECT *

	FROM 
		tblDevice D
	WHERE
		D.Protocol IN (
			10		-- modbus
			,100	-- api
		)
	AND D._active = 1
	AND D._deleted = 0
	AND D.projectedScanTime <= GETDATE()

END
GO

-- TEST
--UPDATE tblDevice SET lastScanTime  = DATEADD(s, 0 - interval, GETDATE()) WHERE Id = 2859
EXEC prcDeviceListForScanning
GO

