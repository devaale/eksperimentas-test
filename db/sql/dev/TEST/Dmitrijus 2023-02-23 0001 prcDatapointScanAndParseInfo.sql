GO
/****** Object:  StoredProcedure [dbo].[prcDatapointScanAndParseInfo]    Script Date: 2023-02-23 02:34:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[prcDatapointScanAndParseInfo] (
	@DataPointId int
) AS

SELECT
	DAP.RegisterAddress,
	DAP.RegisterType,
	DAP.FunctionCode,
	DAP.Multiplier,
	DAP.Offset,
	DEV.[URL],
	DEV.[UnitId]
FROM 
	tblDatapoint DAP 
INNER JOIN 
	tblDevice DEV ON DEV.Id = DAP.DeviceId
WHERE 
	DAP.Id = @DataPointId

