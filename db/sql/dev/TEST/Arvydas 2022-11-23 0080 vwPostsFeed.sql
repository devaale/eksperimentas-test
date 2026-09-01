DROP VIEW IF EXISTS vwPostsFeed 
GO

CREATE VIEW vwPostsFeed AS

	SELECT 
		P.[Id]
		,P.[UserId]
		,U.[Name] [Author]
		,P.[Date]
		,P.[Body]
		,P.[Audience]
		,PI.Id [ImageId]
		,COUNT(PR1.Id) [Likes]

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
		tblPostReaction PR1 ON
			PR1.PostId = P.Id

	GROUP BY
		P.[Id]
		,P.[UserId]
		,P.[Date]
		,P.[Body]
		,U.[Name]
		,P.[Audience]
		,PI.Id
		,CAST(CASE WHEN PR1.Id IS NULL THEN 0 ELSE 1 END AS BIT)


GO

SELECT * FROM vwPostsFeed