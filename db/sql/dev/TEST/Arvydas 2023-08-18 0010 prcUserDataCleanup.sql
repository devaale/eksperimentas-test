DROP PROCEDURE IF EXISTS prcUserDataCleanup
GO
CREATE PROCEDURE prcUserDataCleanup (
	@clean bit = 0
) 
AS BEGIN

	-- Datapoint values
	IF @clean = 0 BEGIN

		-- tblBlocked
		SELECT * FROM tblBlocked
		WHERE UserId NOT IN (
			SELECT USR.Id
			FROM AspNetUsers USR
		) OR RelatedUserId NOT IN (
			SELECT USR.Id
			FROM AspNetUsers USR
		)

		-- tblFriend
		SELECT * FROM tblFriend
		WHERE UserId NOT IN (
			SELECT USR.Id
			FROM AspNetUsers USR
		) OR RelatedUserId NOT IN (
			SELECT USR.Id
			FROM AspNetUsers USR
		)

		-- tblDashboardSetting
		SELECT * FROM tblDashboardSetting
		WHERE UserId NOT IN (
			SELECT USR.Id
			FROM AspNetUsers USR
		)

		-- tblDashboardDatapoint
		SELECT * FROM tblDashboardDatapoint
		WHERE UserId NOT IN (
			SELECT USR.Id
			FROM AspNetUsers USR
		)

		-- tblDatapointValue
		SELECT * FROM tblDatapointValue
		WHERE DatapointId NOT IN (
			SELECT DTP.Id
			FROM AspNetUsers USR
			INNER JOIN tblObject OBJ ON OBJ.UserId = USR.Id
			INNER JOIN tblDevice DEV ON DEV.ObjectId = OBJ.Id
			INNER JOIN tblDatapoint DTP ON DTP.DeviceId = DEV.Id
		)

		-- tblDatapointFormulaChain
		SELECT * FROM tblDatapointFormulaChain
		WHERE DatapointId NOT IN (
			SELECT DTP.Id
			FROM AspNetUsers USR
			INNER JOIN tblObject OBJ ON OBJ.UserId = USR.Id
			INNER JOIN tblDevice DEV ON DEV.ObjectId = OBJ.Id
			INNER JOIN tblDatapoint DTP ON DTP.DeviceId = DEV.Id
		)

		-- tblDatapoint
		SELECT * FROM tblDatapoint
		WHERE DeviceId NOT IN (
			SELECT DEV.Id
			FROM AspNetUsers USR
			INNER JOIN tblObject OBJ ON OBJ.UserId = USR.Id
			INNER JOIN tblDevice DEV ON DEV.ObjectId = OBJ.Id
		)

		-- tblDevice
		SELECT * FROM tblDevice
		WHERE ObjectId NOT IN (
			SELECT OBJ.Id
			FROM AspNetUsers USR
			INNER JOIN tblObject OBJ ON OBJ.UserId = USR.Id
		)

		-- tblObjectPermission
		SELECT * FROM tblObjectPermission OP
		WHERE OP.ObjectId NOT IN (
			SELECT OBJ.Id
			FROM AspNetUsers USR
			INNER JOIN tblObject OBJ ON OBJ.UserId = USR.Id
		) OR OP.FriendUserId NOT IN (
			SELECT USR.Id
			FROM AspNetUsers USR
		)

		-- tblGroupDatapoint
		SELECT * FROM tblGroupDatapoint GD
		WHERE GD.GroupId  NOT IN (
			SELECT GRP.Id
			FROM AspNetUsers USR
			INNER JOIN tblObject OBJ ON OBJ.UserId = USR.Id
			INNER JOIN tblGroup GRP ON GRP.ObjectId = OBJ.Id
		) OR GD.DatapointId NOT IN (
			SELECT DTP.Id
			FROM AspNetUsers USR
			INNER JOIN tblObject OBJ ON OBJ.UserId = USR.Id
			INNER JOIN tblDevice DEV ON DEV.ObjectId = OBJ.Id
			INNER JOIN tblDatapoint DTP ON DTP.DeviceId = DEV.Id
		)

		-- tblObject
		SELECT * FROM tblObject
		WHERE UserId NOT IN (
			SELECT USR.Id
			FROM AspNetUsers USR
		)

		-- tblMessage
		SELECT * FROM tblMessage M
		WHERE M.SenderUserId NOT IN (
			SELECT USR.Id
			FROM AspNetUsers USR
		) OR M.ReceiverUserId NOT IN (
			SELECT USR.Id
			FROM AspNetUsers USR
		)

		-- tblPost...


	END

END
GO

EXEC prcUserDataCleanup 0