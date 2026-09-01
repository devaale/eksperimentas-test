SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[prcUpdateDeprA] (
	@DeviceId int,
	@DatePart varchar(32)
) AS BEGIN

	SET NOCOUNT ON;	-- saves the day as elsewhere PHP MSSQL Driver failing to retrieve the result.

	DECLARE @A AS DECIMAL(18,4)
	DECLARE @PartOfYear AS DECIMAL(18,4)
	DECLARE @Speed AS DECIMAL(18,4)
	SET @Speed = 50.0

	-- Get DeprA (Device Age)
	SELECT
		@A = DEV.DeprA
	FROM
		tblDevice DEV
	WHERE
		DEV.Id = @DeviceId

	-- Change the age of the device depending on the DatePart
	SELECT @PartOfYear =
		CASE
			-- HOUR
			WHEN (@DatePart = '4') THEN (1.0 / (365.0 * 12.0))
			-- DAY
			WHEN (@DatePart = '5') THEN (1.0 / 365.0)
			-- WEEK
			WHEN (@DatePart = '6') THEN (1.0 / 52.1429)
			-- MONTH
			WHEN (@DatePart = '7') THEN (1.0 / 12.0)
			-- QUARTER
			WHEN (@DatePart = '8') THEN (1.0 / 4.0)
			-- YEAR
			WHEN (@DatePart = '9') THEN 1.0
		END

	-- Update DeprA (Device Age)
	UPDATE tblDevice
	SET DeprA = @A + (@PartOfYear * @Speed)
	WHERE
		Id = @DeviceId

END
GO


