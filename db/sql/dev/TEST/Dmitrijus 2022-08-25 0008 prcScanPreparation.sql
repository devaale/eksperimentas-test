USE [ExperimentDB]
GO
/****** Object:  StoredProcedure [dbo].[prcScanPreparation]    Script Date: 2022-08-25 12:44:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[prcScanPreparation] (@_deviceId int) AS 

-- Used in device sanning (.NET module)

SELECT * FROM tblDevice WHERE Id = @_deviceId AND _active = 1 AND _deleted = 0