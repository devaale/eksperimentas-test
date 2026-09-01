ALTER PROCEDURE [dbo].[prcUserInfo] (

	@currentUserId nvarchar(128)
	,@requestedUserId nvarchar(128)

) AS BEGIN

	DECLARE @isMe bit
	SET @isMe = CASE WHEN @currentUserId = @requestedUserId THEN 1 ELSE 0 END

	SELECT
		ANU.[Id]
		,ANU.[Name]
		,ANU.[Language]
		,F.[Id] [FriendId]
		,B.[Id] [BlockedId]
		,@isMe [IsMe]
		,CAST(CASE WHEN F.[Id] IS NULL THEN 0 ELSE 1 END  AS BIT) [IsFriend]
		,CAST(CASE WHEN B.[Id] IS NULL THEN 0 ELSE 1 END  AS BIT) [IsBlocked]
		-- Give real Tokens only in case if user requesting own info, elsewhere NO info or NULL
		,CAST(CASE WHEN ANU.[Id] = @currentUserId THEN ANU.[Tokens] ELSE 0 END as INT) [Tokens]
		-- We do not expose user's blockchain address to the strangers
		,CASE @isMe WHEN 1 THEN ANU.[Address] ELSE NULL END [Address]

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


EXEC prcUserInfo '26b33240-b13e-406d-a2a3-b4e90af3c459', '26b33240-b13e-406d-a2a3-b4e90af3c459'
GO