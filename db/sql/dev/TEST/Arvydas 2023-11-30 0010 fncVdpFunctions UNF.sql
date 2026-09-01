ALTER FUNCTION [dbo].[fncVdpFunctions] (
	@datapointId int)

	RETURNS DECIMAL(18,4)
BEGIN
	-- Date
	DECLARE @date DATETIME

	-- Virtual datapoint params
	DECLARE @datepart tinyint
	DECLARE @formulaId int

	SELECT	
		@date = GETDATE() 
		,@datepart = AggregationDatepart
		,@formulaId = DatapointFormulaId
	FROM
		tblDatapoint
	WHERE
		Id = @datapointId

	-- Relevant for calculations datapoint id
	DECLARE @relDatapointId int

	SELECT TOP 1 @relDatapointId = DFC.RelatedDatapointId
	FROM tblDatapointFormulaChain DFC
	WHERE DatapointId = @datapointId


	DECLARE @dateFrom AS datetime
	DECLARE @dateTo AS datetime
	DECLARE @SUM AS decimal(18,4)

	-- Split date to its all parts
	DECLARE @year int, @month int, @day int, @hour int

	SELECT 
		@year = YEAR(@date),
		@month = MONTH(@date),
		@day = DAY(@date),
		@hour = DATEPART(HOUR, @date)

	SET @dateTo = DATETIMEFROMPARTS(@year, @month, @day, @hour, 0, 0, 0) 

	-- Get DateTime From
	SELECT @dateFrom =
		CASE
			WHEN (@datepart = 4) THEN DATEADD(HOUR, -1, @dateTo)
			WHEN (@datepart = 5) THEN DATEADD(DAY, -1, @dateTo)
			WHEN (@datepart = 6) THEN DATEADD(WEEK, -1, @dateTo)
			WHEN (@datepart = 7) THEN DATEADD(MONTH, -1, @dateTo)
			WHEN (@datepart = 8) THEN DATEADD(QUARTER, -1, @dateTo)
			WHEN (@datepart = 9) THEN DATEADD(YEAR, -1, @dateTo)
		END

	-- Functions and calculations
	SELECT 
		@SUM = CASE
			-- Calculate difference
			WHEN (@formulaId = 50) THEN SUM(DiffValue)
			-- MIN
			WHEN (@formulaId = 60) THEN MIN([Value])
			-- AVG
			WHEN (@formulaId = 70) THEN AVG([Value])
			-- MAX
			WHEN (@formulaId = 80) THEN MAX([Value])
			-- SUM
			WHEN (@formulaId = 90) THEN SUM([Value])
			-- COUNT
			WHEN (@formulaId = 100) THEN COUNT([Value])
		END
	FROM
		vwDatapointValueAdv
	WHERE
		DatapointId = @relDatapointId
	AND [Date] BETWEEN @dateFrom AND @dateTo

	RETURN @SUM
END
GO

SELECT dbo.fncVdpFunctions(158)