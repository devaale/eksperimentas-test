ALTER FUNCTION [dbo].[fncUnreadMessages] (
	@userId nvarchar(128), @senderUserId nvarchar(128) = NULL) 

	RETURNS int AS
BEGIN
	DECLARE @retVal int

	IF @senderUserId IS NULL BEGIN

		SELECT 
			@retVal = COUNT(*) 
		FROM 
			[tblMessage] M

		-- Check are users not deleted
		INNER JOIN
			AspNetUsers SU ON 
				SU.Id = M.SenderUserId
		INNER JOIN
			AspNetUsers RU ON 
				RU.Id = M.ReceiverUserId
		WHERE
			M.[ReceiverUserId] = @userId
		AND M.[Read] IS NULL

	END ELSE BEGIN

		SELECT 
			@retVal = COUNT(*) 
		FROM 
			[tblMessage] M

		-- Check are users not deleted
		INNER JOIN
			AspNetUsers SU ON 
				SU.Id = M.SenderUserId
		INNER JOIN
			AspNetUsers RU ON 
				RU.Id = M.ReceiverUserId
		WHERE
			M.[ReceiverUserId] = @userId
		AND M.[SenderUserId] = @senderUserId
		AND M.[Read] IS NULL

	END
	
	RETURN @retVal
END
GO

DELETE FROM tblMessage WHERE SenderUserId = ReceiverUserId
GO

prcSituation 'ff122391-ddb7-4bc2-8d22-b04ca15e9e2b'
GO