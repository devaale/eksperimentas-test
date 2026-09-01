DROP FUNCTION IF EXISTS dbo.fncUserAddress
GO

CREATE FUNCTION dbo.fncUserAddress (@userId nvarchar(128))
RETURNS nvarchar(128) AS BEGIN

	DECLARE @retVal nvarchar(128)

	SELECT @retVal = W.[Address]
	FROM tblWallet W
	WHERE W.UserId = @userId

	RETURN @retVal
END
GO