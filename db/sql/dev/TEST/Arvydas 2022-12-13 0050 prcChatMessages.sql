DROP PROCEDURE IF EXISTS prcChatMessages
GO

CREATE PROCEDURE prcChatMessages ( 

	@senderUserId nvarchar(128)
	,@receiverUserId nvarchar(128)
	,@sinceDate DATETIME = NULL

) AS BEGIN 

	DECLARE @date DATETIME
	IF @sinceDate IS NOT NULL BEGIN
		SET @date = @sinceDate
	END ELSE BEGIN
		SET @date = GETDATE()
	END

	
	SELECT TOP 5 

		M.[Id]
		,M.[Date]
		,SND.[Name] [Author]
		,M.[Body]
		,CASE WHEN M.SenderUserId = @senderUserId THEN GETDATE() ELSE M.[Read] END [Read]
		,CAST((CASE WHEN M.SenderUserId = @senderUserId THEN 1 ELSE 0 END) AS BIT) [IsMyMessage]

	FROM 
		tblMessage M

	INNER JOIN
		AspNetUsers SND ON
			SND.Id = M.SenderUserId

	WHERE 
		M.[Date] < @date
	AND 
		((SenderUserId = @senderUserId AND ReceiverUserId = @receiverUserId) OR	(SenderUserId = @receiverUserId AND ReceiverUserId = @senderUserId))

	ORDER BY
		M.[Date] DESC
	
	
END
GO


EXEC prcChatMessages '26b33240-b13e-406d-a2a3-b4e90af3c459', '05cbaea7-50c0-4748-8562-d59dac2e74f8', '2022-12-05 22:00:04.000'
EXEC prcChatMessages '26b33240-b13e-406d-a2a3-b4e90af3c459', '05cbaea7-50c0-4748-8562-d59dac2e74f8', NULL