DROP PROCEDURE IF EXISTS prcUserInfo
GO

CREATE PROCEDURE prcUserInfo (

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

	FROM
		[AspNetUsers] ANU
	
	LEFT JOIN
		tblFriend F ON
			F.FriendUserId = ANU.Id
		AND F.UserId = @currentUserId

	LEFT JOIN
		tblBlocked B ON
			B.BlockedUserId = ANU.Id
		AND B.UserId = @currentUserId

	WHERE
		ANU.[Id] = @requestedUserId

END 
GO


EXEC prcUserInfo '26b33240-b13e-406d-a2a3-b4e90af3c459','05cbaea7-50c0-4748-8562-d59dac2e74f8'
EXEC prcUserInfo '26b33240-b13e-406d-a2a3-b4e90af3c459','49268f13-a1f4-4714-8bd0-03b06fae1106'
EXEC prcUserInfo '26b33240-b13e-406d-a2a3-b4e90af3c459','26b33240-b13e-406d-a2a3-b4e90af3c459'