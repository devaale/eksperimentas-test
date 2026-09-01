USE [ExperimentDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[prcLastDatapointValue] (
	@DatapointId int
) AS BEGIN

	SET NOCOUNT ON;	-- saves the day as elsewhere PHP MSSQL Driver failing to retrieve the result.

	SELECT
		TOP 1 *
	FROM
		tblDatapointValue DTP
	WHERE
		DTP.DatapointId = @DatapointId
	ORDER BY [Date]

END