DROP FUNCTION IF EXISTS fncDeviceProtocol
GO

CREATE FUNCTION fncDeviceProtocol ( @deviceId int)
	RETURNS INT AS
BEGIN
	DECLARE @retVal int
	SELECT @retVal = D.Protocol
	FROM tblDevice D
	WHERE 
		D.Id = @deviceId
	RETURN @retVal
END
GO
