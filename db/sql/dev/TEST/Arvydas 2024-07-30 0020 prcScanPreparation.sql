ALTER PROCEDURE [dbo].[prcScanPreparation] (@_deviceId int) AS 

/*
	Used in device sanning (.NET module)

	2024-07-30 Revision (AG)
*/

SELECT * 
FROM tblDevice 
WHERE
	Id = @_deviceId 
AND _active = 1
AND _deleted = 0

SELECT *
FROM
	tblDatapoint
WHERE
	DeviceId = @_deviceId
AND	DatapointType = 1
AND	[ReadWrite] = 0
AND _active = 1
AND _deleted = 0
GO