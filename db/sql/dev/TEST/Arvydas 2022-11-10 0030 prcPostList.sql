ALTER PROCEDURE [dbo].[prcPostList] (
	@userId nvarchar(128),
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

	SELECT DISTINCT TOP 5 
		P.[Id]
		,P.[Date]
		,P.[Body]
		,U.[Name] [Author]
		,P.[Audience]
		,PI.ImageUrl
		,COUNT(PR.Id) [Likes]
		,CAST(CASE WHEN PR.Id IS NULL THEN 0 ELSE 1 END AS BIT) [Liked]

	FROM 
		tblPost P
	JOIN
		AspNetUsers U ON 
			U.Id = P.UserId
	LEFT JOIN
		tblPostImage PI	ON PI.Id =
         (
			 SELECT  TOP 1 PI.Id
			 FROM    tblPostImage
			 WHERE   PI.PostId = P.Id
         )
	LEFT JOIN
		tblPostReaction PR ON
			PR.PostId = P.Id

	WHERE 
		P.[Date] <= @date
	AND (
		-- Private
		(P.Audience = 0 AND P.UserId = @userId) OR
		-- Friends only
		(P.Audience = 1 AND (P.UserId = @userId OR P.UserId IN (SELECT FriendUserId FROM tblFriend WHERE UserId = @userId))) OR
		-- Public
		P.Audience = 2
	)

	GROUP BY
		P.[Id]
		,P.[Date]
		,P.[Body]
		,U.[Name]
		,P.[Audience]
		,PI.ImageUrl
		,CAST(CASE WHEN PR.Id IS NULL THEN 0 ELSE 1 END AS BIT)

	ORDER BY
		P.[Date] DESC

	
END
GO

EXEC prcPostList '26b33240-b13e-406d-a2a3-b4e90af3c459', NULL
GO