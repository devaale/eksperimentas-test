CREATE PROCEDURE [dbo].[prcGetDatapointFormula] (
	@DatapointId int
) AS BEGIN

	SELECT *
	FROM
		tblDatapointFormulaChain dtpfc
	WHERE
		dtpfc.DatapointId = @DatapointId
		
END