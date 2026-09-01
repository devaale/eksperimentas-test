CREATE PROCEDURE [dbo].[prcLastFormulaCalcTimeUpdate] (
	@DatapointId int
) AS BEGIN

	UPDATE
		tblDatapoint
	SET 
		LastFormulaCalcTime = getdate()
	WHERE
		Id = @DatapointId
		
END