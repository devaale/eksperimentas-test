DECLARE @UserId nvarchar(128)
SET @UserId = '26b33240-b13e-406d-a2a3-b4e90af3c459'

SELECT DISTINCT OBJ.*
FROM tblObject OBJ
LEFT JOIN tblObjectPermission OPR ON OPR.ObjectId = OBJ.Id
WHERE 
(
	UserId = @UserId
OR	OPR.FriendUserId = @UserId
)
AND Deleted IS NULL