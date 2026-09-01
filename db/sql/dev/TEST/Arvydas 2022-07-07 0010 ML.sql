EXEC prcUiWordUpdate 'date-range',		N'Date range',		N'Datos diapazonas',	N'Диапазон дат'

EXEC prcUiWordUpdate 'today',			N'Today',			N'Šiandien',	N'Сегодня'
EXEC prcUiWordUpdate 'this-week',		N'This week',		N'Ši savaitė',	N'Текущая неделя'
EXEC prcUiWordUpdate 'this-month',		N'This month',		N'Šis mėnuo',	N'Текущий месяц'
EXEC prcUiWordUpdate 'this-year',		N'This year',		N'Šie metai',	N'Текущий год'

EXEC prcUiWordUpdate 'last24hours',		N'Last 24 hours',	N'Paskutinės 24 valandos',	N'Последние 24 часа'
EXEC prcUiWordUpdate 'last7days',		N'Last 7 days',		N'Paskutinės 7 dienos',		N'Последние 7 дней'
EXEC prcUiWordUpdate 'last12months',	N'Last 12 months',	N'Paskutiniai 12 mėnesių',	N'Последние 12 месяцев'

EXEC prcUiWordUpdate 'server',			N'Server',			N'Serveris',	N'Сервер'


EXEC prcUiWordUpdate 'aggregation',		N'Aggregation',		N'Agregavimas',			N'Агрегация'
EXEC prcUiWordUpdate 'real-value',		N'Real value',		N'Reali vertė',			N'Реальное значение'
EXEC prcUiWordUpdate 'minimum-value',	N'Minimal value',	N'Minimali vertė',		N'Минимальное значение'
EXEC prcUiWordUpdate 'maximum-value',	N'Maximum value',	N'Maksimali vertė',		N'Максимальное значение'
EXEC prcUiWordUpdate 'average-value',	N'Average value',	N'Vidurkis',			N'Среднее значение'
EXEC prcUiWordUpdate 'sum',				N'Sum',				N'Suma',				N'Сумма'

EXEC prcUiWordUpdate 'download',		N'Download',		N'Atsisiųsti',			N'Скачать'

EXEC prcUiWordUpdate 'back',			N'Back',			N'Atgal',				N'Назад'
EXEC prcUiWordUpdate 'forward',			N'Forward',			N'Pirmyn',				N'Вперед'


DROP FUNCTION IF EXISTS dbo.fncAggregateDate
GO

CREATE FUNCTION [dbo].[fncAggregateDate] (
	@date datetime, 
	@measureUnit varchar(32)) 

	RETURNS SMALLDATETIME AS
BEGIN
	RETURN CASE @measureUnit
		WHEN 'Hour' THEN CAST(CONVERT(VARCHAR(13), @date, 120) +':00:00' AS SMALLDATETIME)
		WHEN 'Day' THEN CAST(CONVERT(VARCHAR(10), @date, 120) +' 00:00:00' AS SMALLDATETIME)
		WHEN 'Week' THEN CAST(CONVERT(VARCHAR(19), DATEADD(DAY, 2 - DATEPART(WEEKDAY, @date), CAST(@date AS DATE)), 120) + ' 00:00:00' AS SMALLDATETIME)
		WHEN 'Quarter' THEN CAST(CONVERT(VARCHAR(19), DATEADD(q, DATEDIFF(q, 0, @date), 0), 120) AS SMALLDATETIME)
		WHEN 'Month' THEN CAST(CONVERT(VARCHAR(7), @date, 120) +'-01 00:00:00' AS SMALLDATETIME)
		WHEN 'Year' THEN CAST(CONVERT(VARCHAR(4), @date, 120) +'-01-01 00:00:00' AS SMALLDATETIME)
		ELSE CAST(@date AS SMALLDATETIME) END
END
GO
/*
SELECT dbo.fncAggregateDate(GETDATE(), 'Hour')
SELECT dbo.fncAggregateDate(GETDATE(), 'Day')
SELECT dbo.fncAggregateDate(GETDATE(), 'Week')
SELECT dbo.fncAggregateDate(GETDATE(), 'Month')
SELECT dbo.fncAggregateDate(GETDATE(), 'Quarter')
SELECT dbo.fncAggregateDate(GETDATE(), 'Year')
*/

DROP PROCEDURE IF EXISTS prcDatapointValueList
GO
CREATE PROCEDURE prcDatapointValueList (
	@dateFrom datetime,
	@dateTo datetime,
	@datapointIds varchar(max),
	@measureUnit varchar(32),
	@aggregation varchar(32)
) AS
BEGIN

	IF @measureUnit = 'Minute' BEGIN

		SELECT
			DV.[Id]
			,DV.[DatapointId]
			,DV.[Date]
			,DV.[Value]
		FROM 
			tblDatapoint DP
		INNER JOIN tblDatapointValue DV ON DV.DatapointId = DP.Id
		WHERE
			DV.[Date] BETWEEN @dateFrom AND @dateTo
		AND DP.Id IN (SELECT [value] FROM STRING_SPLIT(@datapointIds, '|'))

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
				WHEN 'MinimalValue' THEN MIN(DV.[Value])
				WHEN 'MaximumValue' THEN MAX(DV.[Value])
				WHEN 'SumValue'		THEN SUM(DV.[Value])
				WHEN 'AverageValue' THEN AVG(DV.[Value])
				ELSE AVG(DV.[Value]) -- AVG
			END [Value]
		FROM 
			tblDatapoint DP
		INNER JOIN tblDatapointValue DV ON DV.DatapointId = DP.Id

		WHERE
			DV.[Date] BETWEEN @dateFrom AND @dateTo
		AND DV.DatapointId IN (SELECT [value] FROM STRING_SPLIT(@datapointIds, '|'))

		GROUP BY
			DP.Id
			,dbo.fncAggregateDate(DV.[Date], @measureUnit)

		ORDER BY
			DP.Id
			,dbo.fncAggregateDate(DV.[Date], @measureUnit)

	END
END
GO

--prcDatapointValueList '2022-01-01', '2022-01-02', '27|28|29|30', 'Day', 'MinimalValue'
--EXECUTE prcDatapointValueList '2022-07-11 00:00:00', '2022-07-17 23:59:59', '29|30', 'Day', 'AverageValue'
--EXECUTE prcDatapointValueList '2022-07-11 00:00:00', '2022-07-17 23:59:59', '29|30', 'Minute', 'RealValue'
--EXECUTE prcDatapointValueList '2022-01-01 00:00:00', '2022-12-31 23:59:59', '29|30', 'Month', 'AverageValue'

EXECUTE prcDatapointValueList '2022-07-11 00:00:00', '2022-07-17 23:59:59', '27|28', 'Hour', 'AverageValue'