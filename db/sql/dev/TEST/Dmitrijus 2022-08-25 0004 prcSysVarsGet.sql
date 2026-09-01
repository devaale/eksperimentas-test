USE [ExperimentDB]
GO
/****** Object:  StoredProcedure [dbo].[prcSysVarsGet]    Script Date: 2022-08-25 10:46:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[prcSysVarsGet] (@module en_sys_name) AS

IF @module is null OR @module = '' BEGIN
	SELECT 
		[name], [value] 
	FROM 
		tblVars 
	ORDER BY 
		[name]
END
ELSE
BEGIN
	SELECT 
		[name], [value] 
	FROM 
		tblVars 
	WHERE 
		module = @module 
	ORDER BY 
		[name]
END

