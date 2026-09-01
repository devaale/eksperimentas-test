ALTER PROCEDURE [dbo].[prcDatapointValueList] (
	@dateFrom datetime,
	@dateTo datetime,
	@datapointIds varchar(max),
	@measureUnit varchar(32),
	@aggregation varchar(32),
	@type int = 0
) AS
BEGIN

	-- START OF DEBUG
	DECLARE @debug BIT
	SET @debug = 0

	IF @debug = 1 BEGIN
		-- Debug log
		INSERT INTO tblDebugLog (Body) VALUES ('['+ 
			CONVERT(NVARCHAR, @dateFrom, 120) +']:['+ 
			CONVERT(NVARCHAR, @dateTo, 120) +'], ['+ 
			@datapointIds +'], [' +
			@measureUnit +'], [' +
			@aggregation +'], [' +
			CAST(@type as NVARCHAR) + ']'
		)
	END
	-- END OF DEBUG

	IF @measureUnit = 'Minute' BEGIN

		SELECT
			DV.[Id]
			,DV.[DatapointId]
			,DV.[Date]
			,CASE @type WHEN 0 THEN DV.[Value] ELSE DV.[DiffValue] END [Value]
		FROM 
			tblDatapoint DP

		INNER JOIN vwDatapointValueAdv DV 
			ON DV.DatapointId = DP.Id

		WHERE
			DV.[Date] BETWEEN @dateFrom AND @dateTo
		AND DP.Id IN (
			SELECT [value] FROM STRING_SPLIT(@datapointIds, '|')
		)

	END ELSE BEGIN 

		SELECT
			-- This needed for EF, as it needs unique Id, elsewhere not working
			CAST(ROW_NUMBER() OVER(
				ORDER BY 
					DP.Id
					,dbo.fncAggregateDate(DV.[Date], @measureUnit)
				) as int) [Id]
			,DP.Id [DatapointId]
			,dbo.fncAggregateDate(DV.[Date], @measureUnit) [Date]
			,CASE @aggregation
				WHEN 'MinimalValue' THEN MIN(CASE @type WHEN 0 THEN DV.[Value] ELSE DV.[DiffValue] END)
				WHEN 'MaximumValue' THEN MAX(CASE @type WHEN 0 THEN DV.[Value] ELSE DV.[DiffValue] END)
				WHEN 'SumValue'		THEN SUM(CASE @type WHEN 0 THEN DV.[Value] ELSE DV.[DiffValue] END)
				WHEN 'AverageValue' THEN AVG(CASE @type WHEN 0 THEN DV.[Value] ELSE DV.[DiffValue] END)
				ELSE AVG(CASE @type WHEN 0 THEN DV.[Value] ELSE DV.[DiffValue] END) -- AVG
			END [Value]

		FROM 
			tblDatapoint DP

		INNER JOIN vwDatapointValueAdv DV 
			ON DV.DatapointId = DP.Id

		WHERE
			DV.[Date] BETWEEN @dateFrom AND @dateTo
		AND DV.DatapointId IN (
			SELECT [value] FROM STRING_SPLIT(@datapointIds, '|')
		)

		GROUP BY
			DP.Id
			,dbo.fncAggregateDate(DV.[Date], @measureUnit)

		ORDER BY
			DP.Id
			,dbo.fncAggregateDate(DV.[Date], @measureUnit)

	END
END
GO