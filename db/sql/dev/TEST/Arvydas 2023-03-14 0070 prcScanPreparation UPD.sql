ALTER PROCEDURE [dbo].[prcScanPreparation] (@_deviceId int) AS BEGIN

	-- Used in device sanning (.NET module)
	SELECT * FROM tblDevice WHERE Id = @_deviceId

	SELECT *
	FROM
		tblDatapoint
	WHERE DeviceId = @_deviceId AND
		DatapointType = 1 AND
		[ReadWrite] = 0

END
GO