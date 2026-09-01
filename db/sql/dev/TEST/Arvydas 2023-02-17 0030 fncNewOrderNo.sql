DROP FUNCTION IF EXISTS [dbo].[fncNewOrderNo]
GO

CREATE FUNCTION [dbo].[fncNewOrderNo] () 
RETURNS nvarchar(16) AS BEGIN
	DECLARE @retInt int

	SELECT @retInt = (COUNT(*) + 1) FROM tblOrder

	IF @retInt > 1 BEGIN
		SELECT @retInt = (CAST(MAX(OrderNo) AS INT) + 1) FROM tblOrder
	END

	RETURN FORMAT(@retInt,'0000000#')
END
GO

/*
	DECLARE @retInt int

	SELECT @retInt = (COUNT(*) + 1) FROM tblOrder

	IF @retInt > 1 BEGIN
		SELECT @retInt = CAST(MAX(OrderNo) AS INT) + 1 FROM tblOrder
	END

	SELECT FORMAT(@retInt,'0000000#')
*/