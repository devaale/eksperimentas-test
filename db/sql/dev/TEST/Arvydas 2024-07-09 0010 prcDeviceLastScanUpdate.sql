DROP PROCEDURE IF EXISTS [prcDeviceLastScanUpdate]
GO

CREATE PROCEDURE [prcDeviceLastScanUpdate] (
	@deviceId int,
	@date datetime = NULL
) AS BEGIN

	/*
		2024-03 Creation (AG)
	*/

	IF @date IS NULL BEGIN
		SET @date = GETDATE()
	END

	UPDATE tblDevice
	SET lastScanTime = @date
	WHERE
		Id = @deviceId
	--AND _deleted = 0
	--AND _active = 1

END
GO