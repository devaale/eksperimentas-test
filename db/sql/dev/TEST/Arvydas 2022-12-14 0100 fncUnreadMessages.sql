DROP FUNCTION IF EXISTS fncUnreadMessages
GO

CREATE FUNCTION [dbo].[fncUnreadMessages] (
	@userId nvarchar(128)) 

	RETURNS int AS
BEGIN
	DECLARE @retVal int

	SELECT 
		@retVal = COUNT(*) 
	FROM 
		[tblMessage] M
	WHERE
		M.[ReceiverUserId] = @userId
	AND M.[Read] IS NULL

	RETURN @retVal
	
END
GO