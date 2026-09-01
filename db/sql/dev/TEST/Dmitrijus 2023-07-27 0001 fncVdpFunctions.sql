USE [ExperimentDB]
GO

/****** Object:  UserDefinedFunction [dbo].[fncVDTFunctions]    Script Date: 2023-07-27 06:13:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[fncVdpFunctions] (
	@Date datetime,
	@Datepart varchar(32),
	@Function varchar(32))

	RETURNS DECIMAL(18,4)
BEGIN
	DECLARE @DateFrom AS datetime
	DECLARE @DateTo AS datetime
	DECLARE @SUM AS decimal(18,4)

	-- Split date to its all parts
	DECLARE @year int, @month int, @day int, @hour int

	SELECT 
		@year = YEAR(@Date),
		@month = MONTH(@Date),
		@day = DAY(@Date),
		@hour = DATEPART(HOUR, @Date)

	SET @DateTo = DATETIMEFROMPARTS(@year, @month, @day, @hour, 0, 0, 0) 

	-- Get DateTime From
	SELECT @DateFrom =
		CASE
			WHEN (@DatePart = '4') THEN DATEADD(HOUR, -1, @Date)
			WHEN (@DatePart = '5') THEN DATEADD(DAY, -1, @Date)
			WHEN (@DatePart = '6') THEN DATEADD(WEEK, -1, @Date)
			WHEN (@DatePart = '7') THEN DATEADD(MONTH, -1, @Date)
			WHEN (@DatePart = '8') THEN DATEADD(QUARTER, -1, @Date)
			WHEN (@DatePart = '9') THEN DATEADD(YEAR, -1, @Date)
		END

	-- Functions and calculations
	SELECT @SUM = 
		CASE
			-- Calculate difference
			WHEN (@Function = '50') THEN SUM(DiffValue)
			-- MIN
			WHEN (@Function = '60') THEN MIN([Value])
			-- AVG
			WHEN (@Function = '70') THEN AVG([Value])
			-- MAX
			WHEN (@Function = '80') THEN MAX([Value])
			-- SUM
			WHEN (@Function = '90') THEN SUM([Value])
			-- COUNT
			WHEN (@Function = '100') THEN COUNT([Value])
		END
	FROM
		vwDatapointValueAdv
	WHERE
		DatapointId = 1 AND [Date] BETWEEN @DateFrom AND @DateTo

	RETURN @SUM
END

GO


