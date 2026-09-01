USE [ExperimentDB]
GO
/****** Object:  StoredProcedure [dbo].[prcSysVarGet]    Script Date: 2022-08-25 10:37:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[prcSysVarGet] (@name en_sys_name) AS

SELECT [value] FROM tblVars WHERE @name = @name
