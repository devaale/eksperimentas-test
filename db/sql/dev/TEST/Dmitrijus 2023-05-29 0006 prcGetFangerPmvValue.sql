CREATE PROCEDURE [dbo].[prcGetFangerPmvValue] (
	@DatapointId int
) AS BEGIN

	DECLARE @TimeNow datetime = GETDATE()
	DECLARE @TimeSubstracted datetime

	-- Subtract 1 hour, formatted
	SET @TimeSubstracted = convert(varchar, dateadd(hour, -1, @TimeNow), 120)

	SELECT TOP 1 *
	FROM
		tblDatapointValue dtp
	WHERE
		dtp.[Date] >= @TimeSubstracted AND
		dtp.DatapointId = @DatapointId
		
END