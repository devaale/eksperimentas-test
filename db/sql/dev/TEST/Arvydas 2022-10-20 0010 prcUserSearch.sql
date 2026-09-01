DROP PROCEDURE IF EXISTS prcUserSearch
GO

CREATE PROCEDURE prcUserSearch (
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
	)
		

END
GO

EXEC prcUserSearch '26b33240-b13e-406d-a2a3-b4e90af3c459', 'a'