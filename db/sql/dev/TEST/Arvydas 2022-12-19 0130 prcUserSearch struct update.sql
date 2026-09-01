ALTER PROCEDURE [dbo].[prcUserSearch] (
	@userId nvarchar(128)
	,@type varchar(20)
	,@phrase nvarchar(128)
) AS BEGIN

	IF @type = 'Friend' BEGIN

		SELECT *
		FROM 
			AspNetUsers ANU
		WHERE 
			LOWER(ISNULL(ANU.Name, '')) LIKE CONCAT('%', LOWER(@phrase), '%')
		AND ANU.Id != @userId
		AND ANU.Id NOT IN (
			SELECT RelatedUserId FROM tblFriend WHERE UserId = @userId
			UNION ALL
			SELECT RelatedUserId FROM tblBlocked WHERE UserId = @userId
			UNION ALL 
			SELECT UserId FROM tblBlocked WHERE RelatedUserId = @userId
		)

	END ELSE IF @type = 'Blocked' BEGIN

		SELECT *
		FROM 
			AspNetUsers ANU
		WHERE 
			LOWER(ISNULL(ANU.Name, '')) LIKE CONCAT('%', LOWER(@phrase), '%')
		AND ANU.Id != @userId
		AND ANU.Id NOT IN (
			--SELECT RelatedUserId FROM tblFriend WHERE UserId = @userId
			--UNION ALL
			SELECT RelatedUserId FROM tblBlocked WHERE UserId = @userId
			UNION ALL 
			SELECT UserId FROM tblBlocked WHERE RelatedUserId = @userId
		)

	END

END
GO
