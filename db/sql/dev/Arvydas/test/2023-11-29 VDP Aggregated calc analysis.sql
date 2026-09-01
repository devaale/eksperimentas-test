DECLARE @datapointId INT
SET @datapointId = 139

--SELECT * FROM tblDatapoint WHERE Id = @datapointId

-- Retrieve virtual datapoint data
DECLARE @AggregationDatepart tinyint
DECLARe @DatapointFormulaId int

SELECT 
	@AggregationDatepart = AggregationDatepart 
	,@DatapointFormulaId = DatapointFormulaId
FROM 
	tblDatapoint WHERE Id = @datapointId

EXEC prcGetDatapointInfoWithFormulaChain @datapointId

-- Retrieve datapoint formula data
SELECT * FROM tblDatapointFormula WHERE Id = @DatapointFormulaId

-- Sort of calculation of VDP
SELECT [dbo].[fncVdpFunctions](@datapointId, @AggregationDatepart, @DatapointFormulaId)

-- Datapoint values
SELECT * FROM tblDatapointValue WHERE DatapointId = @datapointId