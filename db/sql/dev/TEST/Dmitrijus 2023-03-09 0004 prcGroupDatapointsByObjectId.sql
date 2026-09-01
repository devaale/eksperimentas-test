USE [ExperimentDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[prcGroupDatapointsByGroupId] (
	@GroupId int
) AS BEGIN

	SELECT
		*
	FROM
		tblGroupDatapoint GDTP
	WHERE
		GDTP.GroupId = @GroupId

END