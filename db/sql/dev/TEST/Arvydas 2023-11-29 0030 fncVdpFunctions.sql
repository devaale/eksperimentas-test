ALTER FUNCTION [dbo].[fncVdpFunctions] (
	@datapointId int)

	RETURNS DECIMAL(18,4)
BEGIN
	-- Datapoint params
	DECLARE @date DATETIME
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

	DECLARE @DateFrom AS datetime
	DECLARE @DateTo AS datetime
	DECLARE @SUM AS decimal(18,4)

	-- Split date to its all parts
	DECLARE @year int, @month int, @day int, @hour int

	SELECT 
		@year = YEAR(@date),
		@month = MONTH(@date),
		@day = DAY(@date),
		@hour = DATEPART(HOUR, @date)

	SET @DateTo = DATETIMEFROMPARTS(@year, @month, @day, @hour, 0, 0, 0) 

	-- Get DateTime From
	SELECT @DateFrom =
		CASE
			WHEN (@datepart = 4) THEN DATEADD(HOUR, -1, @date)
			WHEN (@datepart = 5) THEN DATEADD(DAY, -1, @date)
			WHEN (@datepart = 6) THEN DATEADD(WEEK, -1, @date)
			WHEN (@datepart = 7) THEN DATEADD(MONTH, -1, @date)
			WHEN (@datepart = 8) THEN DATEADD(QUARTER, -1, @date)
			WHEN (@datepart = 9) THEN DATEADD(YEAR, -1, @date)
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
		DatapointId = @datapointId
	AND [Date] BETWEEN @DateFrom AND @DateTo

	RETURN @SUM
END
GO
