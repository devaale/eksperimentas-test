ALTER PROCEDURE [dbo].[prcChatMessages] ( 

	@senderUserId nvarchar(128)
	,@receiverUserId nvarchar(128)
	,@listLoadMode tinyint = 0
	,@firstDate DATETIME = NULL
	,@lastDate DATETIME = NULL

) AS BEGIN 

/*
	PARAMS

		@senderUserId Sender User Id

		@receiverUserId Receiver User Id

		@listLoadMode:

			0 - FULL RELOAD FROM 0 or full REFRESH

			1 - CHECK ONLY FOR NEW DATA BY DATE

			2 - SCROLL DOWN, LOAD OF OLDER MESSAGES
		
		@firstDate	Date of first loaded post

		@lastDate	Date of last loaded post

*/

	IF @listLoadMode = 1 BEGIN

		SELECT 

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
			M.[Date] > @firstDate
		AND 
			((SenderUserId = @senderUserId AND ReceiverUserId = @receiverUserId) OR	(SenderUserId = @receiverUserId AND ReceiverUserId = @senderUserId))

		ORDER BY
			M.[Date] DESC

	END IF @listLoadMode = 0 OR @listLoadMode = 2 BEGIN

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
			(
					@listLoadMode = 0
				OR	(@listLoadMode = 2 AND M.[Date] < @lastDate)
			)
		AND 
			((SenderUserId = @senderUserId AND ReceiverUserId = @receiverUserId) OR	(SenderUserId = @receiverUserId AND ReceiverUserId = @senderUserId))

		ORDER BY
			M.[Date] DESC
	END

END
GO

-- FULL RELOAD
EXEC prcChatMessages '26b33240-b13e-406d-a2a3-b4e90af3c459', '05cbaea7-50c0-4748-8562-d59dac2e74f8', 0, NULL, NULL
-- ADDING NEW MESSAGES ABOVE OLDER
--EXEC prcChatMessages '26b33240-b13e-406d-a2a3-b4e90af3c459', '05cbaea7-50c0-4748-8562-d59dac2e74f8', 1, '2022-12-07 18:20:14.907', '2022-12-05 22:00:06.000'
-- SCROLL DOWN - LOAD OF FURTHER OLDER MESSAGES
--EXEC prcChatMessages '26b33240-b13e-406d-a2a3-b4e90af3c459', '05cbaea7-50c0-4748-8562-d59dac2e74f8', 2, '2022-12-07 18:20:14.907', '2022-12-05 22:00:06.000'


