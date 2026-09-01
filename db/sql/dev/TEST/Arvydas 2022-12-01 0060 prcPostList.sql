USE [ExperimentDB]
GO
/****** Object:  StoredProcedure [dbo].[prcPostList]    Script Date: 2022-12-01 20:11:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[prcPostList] (
	@userId nvarchar(128),
	@feedType int,
	@sinceDate DATETIME
) AS 
BEGIN

	-- Date
	DECLARE @date DATETIME
	IF @sinceDate IS NOT NULL BEGIN
		SET @date = @sinceDate
	END ELSE BEGIN
		SET @date = GETDATE()
	END

	-- Selection

	SELECT TOP 5 
		PF.[Id]
		,PF.[Date]
		,PF.[Body]
		,PF.[Author]
		,PF.[Audience]
		,PF.[ImageId]
		,PF.[Likes]
		,CAST(CASE WHEN PR2.Id IS NULL THEN 0 ELSE 1 END AS BIT) [Liked]

	FROM 
		vwPostsFeed PF

	LEFT JOIN
		tblPostReaction PR2 ON
			PR2.PostId = PF.Id
		AND PR2.UserId = @userId

	WHERE 
		PF.[Date] <= @date
	AND (
		-- Private
		(PF.Audience = 0 AND PF.UserId = @userId) OR
		-- Friends only
		(PF.Audience = 1 AND (PF.UserId = @userId OR PF.UserId IN (SELECT FriendUserId FROM tblFriend WHERE UserId = @userId))) OR
		-- Public
		PF.Audience = 2
	)
	AND PF.UserId NOT IN (
		SELECT BlockedUserId FROM tblBlocked WHERE UserId = @userId
		UNION ALL
		SELECT UserId FROM tblBlocked WHERE BlockedUserId = @userId
	)

	ORDER BY
		CASE @feedType
			WHEN 0 THEN PF.[Date] 
			WHEN 1 THEN PF.[Likes]
		END DESC, PF.[Date] DESC
		

	
END
GO

EXEC prcPostList '26b33240-b13e-406d-a2a3-b4e90af3c459', 0, NULL