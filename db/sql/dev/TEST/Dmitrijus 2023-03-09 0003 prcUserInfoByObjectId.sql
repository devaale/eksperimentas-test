USE [ExperimentDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[prcUserInfoByObjectId] (
	@ObjectId int
) AS BEGIN

	SELECT
		U.Id,
		U.[Name],
		U.Email
	FROM
		tblObject OB
	INNER JOIN
		AspNetUsers U ON OB.UserId = U.Id
	WHERE
		OB.Id = @ObjectId
END