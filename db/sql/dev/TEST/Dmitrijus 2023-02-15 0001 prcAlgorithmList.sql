USE [ExperimentDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[prcAlgorithmList] AS BEGIN

	SET NOCOUNT ON;	-- saves the day as elsewhere PHP MSSQL Driver failing to retrieve the result.

	SELECT *
	FROM  
		tblAlgorithm

END
