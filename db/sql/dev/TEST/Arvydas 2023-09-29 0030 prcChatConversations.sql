ALTER PROCEDURE [dbo].[prcChatConversations] (
	@userId nvarchar(128)
) AS BEGIN


	SELECT
		M.[Date]
		,M.SenderUserId
		,US.[Name] [Sender]
		,M.ReceiverUserId
		,UR.[Name] [Receiver]
		,M.[Body]
		,CASE WHEN M.SenderUserId = @userId THEN GETDATE() ELSE M.[Read] END [Read]
		,CAST(CASE WHEN M.SenderUserId = @userId THEN 1 ELSE 0 END as BIT) [IsMyMessage]
		,dbo.fncUnreadMessages(@userId, AGM.ReceiverUserId) [NumUnread]
		,CAST(CASE WHEN dbo.fncUnreadMessages(@userId, AGM.ReceiverUserId) > 0 THEN 1 ELSE 0 END AS BIT) [HasUnread]
	FROM (
		SELECT 
			MAX(M.[Date]) [Date]
			,CASE WHEN M.SenderUserId = @userId THEN M.SenderUserId ELSE M.ReceiverUserId END [SenderUserId]
			,CASE WHEN M.SenderUserId = @userId THEN M.ReceiverUserId ELSE M.SenderUserId END [ReceiverUserId]
		FROM 
			tblMessage M
		GROUP BY
			CASE WHEN M.SenderUserId = @userId THEN M.SenderUserId ELSE M.ReceiverUserId END
			,CASE WHEN M.SenderUserId = @userId THEN M.ReceiverUserId ELSE M.SenderUserId END
	) AGM
	
	JOIN 
		tblMessage M ON 
	M.Id = (
		SELECT TOP 1 M.Id
		FROM tblMessage M
		WHERE 
			(M.SenderUserId = AGM.ReceiverUserId AND M.ReceiverUserId = @userId)
		OR (M.ReceiverUserId = AGM.ReceiverUserId AND M.SenderUserId = @userId)
		ORDER BY M.[Date] DESC
	)

	LEFT JOIN
		AspNetUsers US ON
		US.[Id] = M.[SenderUserId]

	LEFT JOIN
		AspNetUsers UR ON
		UR.[Id] = M.[ReceiverUserId]

	WHERE
		AGM.[SenderUserId] = @userId

	ORDER BY
		AGM.[Date] DESC

END
GO

EXEC prcChatConversations '26b33240-b13e-406d-a2a3-b4e90af3c459'
GO