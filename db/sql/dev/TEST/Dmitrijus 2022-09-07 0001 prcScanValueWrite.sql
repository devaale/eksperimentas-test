USE [ExperimentDB]
GO
/****** Object:  StoredProcedure [dbo].[prcScanValueWrite]    Script Date: 2022-09-06 17:04:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[prcScanValueWrite] (
	@_deviceId int,
	@_dataPointId int,
	@value float
) AS

DECLARE @scanDate datetime
SET @scanDate = GETDATE()

INSERT INTO tblDatapointValue(
	DatapointId,		[Value],	[Date]
) VALUES (
	@_dataPointId,		@value,		@scanDate
)

UPDATE tblDevice SET lastScanTime = getDate() WHERE Id = @_deviceId

return @@IDENTITY