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
		WHERE
			M.[ReceiverUserId] = @userId
		AND M.[Read] IS NULL

	END ELSE BEGIN

		SELECT 
			@retVal = COUNT(*) 
		FROM 
			[tblMessage] M
		WHERE
			M.[ReceiverUserId] = @userId
		AND M.[SenderUserId] = @senderUserId
		AND M.[Read] IS NULL

	END
	
	RETURN @retVal
END
