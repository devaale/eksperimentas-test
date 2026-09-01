USE [ExperimentDB]
GO
/****** Object:  StoredProcedure [dbo].[prcScanValueWrite]    Script Date: 2023-06-02 14:29:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[prcCalcFormulaValueWrite] (
	@_deviceId int,
	@_dataPointId int,
	@value float
) AS

DECLARE @calcDate datetime
SET @calcDate = DATEADD(HOUR, DATEDIFF(HOUR, 0, GETDATE()) + DATEDIFF(HOUR, GETDATE(), GETDATE()), 0)


INSERT INTO tblDatapointValue(
	DatapointId,		[Value],	[Date]
) VALUES (
	@_dataPointId,		@value,		@calcDate
)

UPDATE tblDevice SET lastScanTime = @calcDate WHERE Id = @_deviceId

return @@IDENTITY