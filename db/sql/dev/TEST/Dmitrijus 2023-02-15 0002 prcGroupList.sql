USE [ExperimentDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[prcGroupList] (
	@ReadWrite int
) AS BEGIN

	SET NOCOUNT ON;	-- saves the day as elsewhere PHP MSSQL Driver failing to retrieve the result.

	SELECT *
	FROM  
		tblGroup

END