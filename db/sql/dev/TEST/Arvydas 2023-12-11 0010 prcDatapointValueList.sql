ALTER PROCEDURE [dbo].[prcDatapointValueList] (
	@dateFrom datetime,
	@dateTo datetime,
	@datapointIds varchar(max),
	@measureUnit varchar(32),
	@aggregation varchar(32),
	@type int = 0,
	@comparison varchar(max)
) AS
BEGIN

	-- START OF DEBUG
	DECLARE @debug BIT
	SET @debug = 1

	IF @debug = 1 BEGIN
		-- Debug log
		INSERT INTO tblDebugLog (Body) VALUES ('['+ 
			CONVERT(NVARCHAR, @dateFrom, 120) +']:['+ 
			CONVERT(NVARCHAR, @dateTo, 120) +'], ['+ 
			@datapointIds +'], [' +
			@measureUnit +'], [' +
			@aggregation +'], [' +
			CAST(@type as NVARCHAR) + '], [' +
			ISNULL(@comparison, 'NULL') +']'
		)
	END
	-- END OF DEBUG

	-- PREPARATION OF QUERY
	CREATE TABLE #datapointFilter (
		[DatapointId] [int],
		[YearOffset] [int],
	)

	INSERT INTO #datapointFilter
	SELECT 
		DPS.[value] [DatapointId]
		,ISNULL(OFS.[value],0) [YearOffset]
	FROM STRING_SPLIT(@datapointIds, '|') AS DPS
	FULL OUTER JOIN STRING_SPLIT(@comparison, '|') AS OFS ON 1 = 1

/*	
	-- 2023-12-11 Old and difficult code replaced with code above, starting from: INSERT INTO #datapointFilter ..
	DECLARE @datapointId int, @YearOffset int

	DECLARE dpCursor CURSOR FOR 
	SELECT [value] FROM STRING_SPLIT(@datapointIds, '|')
	OPEN dpCursor  

	FETCH NEXT FROM dpCursor INTO @datapointId
	WHILE @@FETCH_STATUS = 0  
	BEGIN
		IF @comparison IS NOT NULL BEGIN

			DECLARE offCursor CURSOR FOR 
				SELECT [value] FROM STRING_SPLIT(@comparison, '|')
			OPEN offCursor  

			FETCH NEXT FROM offCursor INTO @YearOffset
			WHILE @@FETCH_STATUS = 0  
			BEGIN
			
				INSERT INTO #datapointFilter (
					[DatapointId], [YearOffset]
				) VALUES (
					@datapointId, @YearOffset
				)

				FETCH NEXT FROM offCursor INTO @YearOffset
			END

			CLOSE offCursor
			DEALLOCATE offCursor 
		END
		
		INSERT INTO #datapointFilter (
			[DatapointId], [YearOffset]
		) VALUES (
			@datapointId, 0
		)

		FETCH NEXT FROM dpCursor INTO @datapointId
	END

	CLOSE dpCursor
	DEALLOCATE dpCursor 
	-- END OF PREPARATION
*/	
	-- NEVER ENABLE, ONLY FOR DEBUG
	--SELECT * FROM #datapointFilter

	-- DATA RETRIEVAL
	IF @measureUnit = 'Minute' BEGIN

		PRINT 'MINUTE!'
		-- We just off-setting date and date filter can remain the same
		SELECT
			DV.[Id]
			,DV.[DatapointId]
			,DATEADD(year, DF.YearOffset, DV.[Date]) [Date]
			,CASE @type WHEN 0 THEN DV.[Value] ELSE DV.[DiffValue] END [Value]
			,DF.[YearOffset] [Year]

		FROM
			#datapointFilter DF

		INNER JOIN vwDatapointValueAdv DV 
			ON DV.DatapointId = DF.DatapointId

		WHERE
			DATEADD(year, DF.YearOffset, DV.[Date]) BETWEEN @dateFrom AND @dateTo

		ORDER BY 
			DF.[YearOffset],
			DV.[Name],
			DATEADD(year, DF.YearOffset, DV.[Date])

	END ELSE BEGIN 

		PRINT 'NOT MINUTE!'
		SELECT
			-- This needed for EF, as it needs unique Id, elsewhere not working
			CAST(ROW_NUMBER() OVER(
				ORDER BY 
					DV.[DatapointId]
					,dbo.fncAggregateDate(DATEADD(year, DF.YearOffset, DV.[Date]), @measureUnit)
				) as int) [Id]
			,DV.[DatapointId]
			,dbo.fncAggregateDate(DATEADD(year, DF.YearOffset, DV.[Date]), @measureUnit) [Date]
			,CASE @aggregation
				WHEN 'MinimalValue' THEN MIN(CASE @type WHEN 0 THEN DV.[Value] ELSE DV.[DiffValue] END)
				WHEN 'MaximumValue' THEN MAX(CASE @type WHEN 0 THEN DV.[Value] ELSE DV.[DiffValue] END)
				WHEN 'SumValue'		THEN SUM(CASE @type WHEN 0 THEN DV.[Value] ELSE DV.[DiffValue] END)
				WHEN 'AverageValue' THEN AVG(CASE @type WHEN 0 THEN DV.[Value] ELSE DV.[DiffValue] END)
				ELSE AVG(CASE @type WHEN 0 THEN DV.[Value] ELSE DV.[DiffValue] END) -- AVG
			END [Value]
			,DF.[YearOffset] [Year]

		FROM
			#datapointFilter DF

		INNER JOIN vwDatapointValueAdv DV 
			ON DV.[DatapointId] = DF.[DatapointId]

		WHERE
			DATEADD(year, DF.YearOffset, DV.[Date]) BETWEEN @dateFrom AND @dateTo

		GROUP BY
			DF.[YearOffset] 
			,DV.[DatapointId]
			,DV.[Name]
			,dbo.fncAggregateDate(DATEADD(year, DF.YearOffset, DV.[Date]), @measureUnit)

		ORDER BY
			DF.[YearOffset] 
			,DV.[Name]
			,dbo.fncAggregateDate(DATEADD(year, DF.YearOffset, DV.[Date]), @measureUnit)

	END
	-- END OF DATA RETRIEVAL

	DROP TABLE #datapointFilter
END
GO

EXEC prcDatapointValueList '2023-12-11', '2023-12-11 23:59:59', '154', 'Minute', 'RealValue', 0, NULL

--SELECT * FROM tblDebugLog ORDER BY [Date] DESC