ALTER PROCEDURE [dbo].[prcUserSearch] (
	@userId nvarchar(128)
	,@phrase nvarchar(128)
) AS BEGIN

	SELECT *
	FROM 
		AspNetUsers ANU
	WHERE 
		LOWER(ISNULL(ANU.Name, '')) LIKE CONCAT('%', LOWER(@phrase), '%')
	AND ANU.Id != @userId
	AND ANU.Id NOT IN (
		SELECT FriendUserId FROM tblFriend WHERE UserId = @userId
		UNION ALL
		SELECT BlockedUserId FROM tblBlocked WHERE UserId = @userId
		UNION ALL 
		SELECT UserId FROM tblBlocked WHERE BlockedUserId = @userId
	)
		

END
