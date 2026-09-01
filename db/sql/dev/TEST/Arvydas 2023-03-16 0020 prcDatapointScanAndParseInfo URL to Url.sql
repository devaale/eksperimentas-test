ALTER PROCEDURE [dbo].[prcDatapointScanAndParseInfo] (
	@DataPointId int
) AS BEGIN

SELECT
	DAP.RegisterAddress,
	DAP.RegisterType,
	DAP.FunctionCode,
	DAP.Multiplier,
	DAP.Offset,
	DEV.[Url],
	DEV.[UnitId]
FROM 
	tblDatapoint DAP 
INNER JOIN 
	tblDevice DEV ON DEV.Id = DAP.DeviceId
WHERE 
	DAP.Id = @DataPointId

END
GO