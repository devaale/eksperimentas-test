USE [ExperimentDB]
GO
/****** Object:  StoredProcedure [dbo].[prcUserInfo]    Script Date: 2022-12-22 15:38:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
		,CAST(CASE WHEN ANU.[Id] = @currentUserId THEN ANU.[Tokens] ELSE NULL END as INT) [Tokens]

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

-- User retrieves own info
EXEC [prcUserInfo] '26b33240-b13e-406d-a2a3-b4e90af3c459', '26b33240-b13e-406d-a2a3-b4e90af3c459'
-- User retrieves another user info (Tokens should be NULL)
EXEC [prcUserInfo] '26b33240-b13e-406d-a2a3-b4e90af3c459', '05cbaea7-50c0-4748-8562-d59dac2e74f8'