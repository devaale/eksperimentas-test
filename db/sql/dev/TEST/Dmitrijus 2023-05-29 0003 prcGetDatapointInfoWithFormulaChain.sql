CREATE PROCEDURE [dbo].[prcGetDatapointInfoWithFormulaChain](
	@DatapointId int
) AS BEGIN

	SELECT *
	FROM
		tblDatapoint dtp
	WHERE
		dtp.Id = @DatapointId

	SELECT *
	FROM
		tblDatapointFormulaChain dtpfc
	WHERE
		dtpfc.DatapointId = @DatapointId
		
END
