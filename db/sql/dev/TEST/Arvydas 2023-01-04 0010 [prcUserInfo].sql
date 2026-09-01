ALTER PROCEDURE [dbo].[prcUserInfo] (

	@currentUserId nvarchar(128)
	,@requestedUserId nvarchar(128)

) AS BEGIN

	SELECT
		ANU.[Id]
		,ANU.[Name]
		,ANU.[Language]
		,F.[Id] [FriendId]
		,B.[Id] [BlockedId]
		,CAST(CASE WHEN ANU.[Id] = @currentUserId THEN 1 ELSE 0 END as BIT) [IsMe]
		,CAST(CASE WHEN F.[Id] IS NULL THEN 0 ELSE 1 END  AS BIT) [IsFriend]
		,CAST(CASE WHEN B.[Id] IS NULL THEN 0 ELSE 1 END  AS BIT) [IsBlocked]
		-- Give real Tokens only in case if user requesting own info, elsewhere NO info or NULL
		,CAST(CASE WHEN ANU.[Id] = @currentUserId THEN ANU.[Tokens] ELSE 0 END as INT) [Tokens]

	FROM
		[AspNetUsers] ANU
	
	LEFT JOIN
		tblFriend F ON
			F.RelatedUserId = ANU.Id
		AND F.UserId = @currentUserId

	LEFT JOIN
		tblBlocked B ON
			B.RelatedUserId = ANU.Id
		AND B.UserId = @currentUserId

	WHERE
		ANU.[Id] = @requestedUserId

END 
GO


EXEC prcUserInfo '26b33240-b13e-406d-a2a3-b4e90af3c459', 'a383d38b-1a87-49a8-96c8-e661e9ce618e'