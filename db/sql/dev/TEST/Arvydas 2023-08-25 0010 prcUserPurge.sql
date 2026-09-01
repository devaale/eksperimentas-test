DROP PROCEDURE IF EXISTS prcUserPurge 
GO
CREATE PROCEDURE prcUserPurge (
	@commit bit = 1,
	@debug bit = 0
) AS
BEGIN
	BEGIN TRAN

	IF @debug = 1 BEGIN PRINT 'DELETE FROM tblDatapointValue' END
	DELETE FROM DV
		FROM AspNetUsers U
		INNER JOIN tblObject OBJ ON OBJ.UserId = U.Id
		INNER JOIN tblDevice DEV ON DEV.ObjectId = OBJ.Id
		INNER JOIN tblDatapoint DTP ON DTP.DeviceId = DEV.Id
		INNER JOIN tblDatapointValue DV ON DV.DatapointId = DTP.Id
	WHERE
		U.RemovalRequested < GETDATE()

	IF @debug = 1 BEGIN PRINT 'DELETE FROM tblDatapointFormulaChain' END
	DELETE FROM DFC
		FROM AspNetUsers U
		INNER JOIN tblObject OBJ ON OBJ.UserId = U.Id
		INNER JOIN tblDevice DEV ON DEV.ObjectId = OBJ.Id
		INNER JOIN tblDatapoint DTP ON DTP.DeviceId = DEV.Id
		INNER JOIN tblDatapointFormulaChain DFC ON DFC.DatapointId = DTP.Id
	WHERE
		U.RemovalRequested < GETDATE()

	IF @debug = 1 BEGIN PRINT 'DELETE FROM tblGroupDatapoint' END
	DELETE FROM GDP
		FROM AspNetUsers U
		INNER JOIN tblObject OBJ ON OBJ.UserId = U.Id
		INNER JOIN tblDevice DEV ON DEV.ObjectId = OBJ.Id
		INNER JOIN tblDatapoint DTP ON DTP.DeviceId = DEV.Id
		INNER JOIN tblGroupDatapoint GDP ON GDP.DatapointId = DTP.Id
	WHERE
		U.RemovalRequested < GETDATE()

	IF @debug = 1 BEGIN PRINT 'DELETE FROM tblDatapoint' END
	DELETE FROM DTP
		FROM AspNetUsers U
		INNER JOIN tblObject OBJ ON OBJ.UserId = U.Id
		INNER JOIN tblDevice DEV ON DEV.ObjectId = OBJ.Id
		INNER JOIN tblDatapoint DTP ON DTP.DeviceId = DEV.Id
	WHERE
		U.RemovalRequested < GETDATE()

	IF @debug = 1 BEGIN PRINT 'DELETE FROM tblDevice' END
	DELETE FROM DEV
		FROM AspNetUsers U
		INNER JOIN tblObject OBJ ON OBJ.UserId = U.Id
		INNER JOIN tblDevice DEV ON DEV.ObjectId = OBJ.Id
	WHERE
		U.RemovalRequested < GETDATE()

	IF @debug = 1 BEGIN PRINT 'DELETE FROM tblGroup' END
	DELETE FROM GRP
		FROM AspNetUsers U
		INNER JOIN tblObject OBJ ON OBJ.UserId = U.Id
		INNER JOIN tblGroup GRP ON GRP.ObjectId = OBJ.Id
	WHERE
		U.RemovalRequested < GETDATE()

	IF @debug = 1 BEGIN PRINT 'DELETE FROM tblObjectPermission' END
	DELETE FROM OPR
		FROM AspNetUsers U
		INNER JOIN tblObject OBJ ON OBJ.UserId = U.Id
		INNER JOIN tblObjectPermission OPR ON OPR.ObjectId = OBJ.Id
	WHERE
		U.RemovalRequested < GETDATE()

	IF @debug = 1 BEGIN PRINT 'DELETE FROM tblAlgorithm' END
	DELETE FROM AGL
		FROM AspNetUsers U
		INNER JOIN tblObject OBJ ON OBJ.UserId = U.Id
		INNER JOIN tblAlgorithm AGL ON AGL.ObjectId = OBJ.Id
	WHERE
		U.RemovalRequested < GETDATE()

	IF @debug = 1 BEGIN PRINT 'DELETE FROM tblObject' END
	DELETE FROM OBJ
		FROM AspNetUsers U
		INNER JOIN tblObject OBJ ON OBJ.UserId = U.Id
	WHERE
		U.RemovalRequested < GETDATE()

	IF @debug = 1 BEGIN PRINT 'DELETE FROM tblDashboardSetting' END
	DELETE FROM DS
		FROM AspNetUsers U
		INNER JOIN tblDashboardSetting DS ON DS.UserId = U.Id
	WHERE
		U.RemovalRequested < GETDATE()

	IF @debug = 1 BEGIN PRINT 'DELETE FROM tblDashboardDatapoint' END
	DELETE FROM DD
		FROM AspNetUsers U
		INNER JOIN tblDashboardDatapoint DD ON DD.UserId = U.Id
	WHERE
		U.RemovalRequested < GETDATE()

	IF @debug = 1 BEGIN PRINT 'DELETE FROM tblMessage' END
	DELETE FROM MSG
		FROM tblMessage MSG
	WHERE
		MSG.SenderUserId IN (SELECT Id FROM AspNetUsers WHERE RemovalRequested < GETDATE())
	OR	MSG.ReceiverUserId IN (SELECT Id FROM AspNetUsers WHERE RemovalRequested < GETDATE())

	IF @debug = 1 BEGIN PRINT 'DELETE FROM tblPostReaction' END
	DELETE FROM PR
		FROM AspNetUsers U
		INNER JOIN tblPost P ON P.UserId = U.Id
		INNER JOIN tblPostReaction PR ON PR.PostId = P.Id
	WHERE
		U.RemovalRequested < GETDATE()

	IF @debug = 1 BEGIN PRINT 'DELETE FROM tblPostImage' END
	DELETE FROM POI
		FROM AspNetUsers U
		INNER JOIN tblPost P ON P.UserId = U.Id
		INNER JOIN tblPostImage POI ON POI.PostId = P.Id
	WHERE
		U.RemovalRequested < GETDATE()

	IF @debug = 1 BEGIN PRINT 'DELETE FROM tblPost' END
	DELETE FROM P
		FROM AspNetUsers U
		INNER JOIN tblPost P ON P.UserId = U.Id
	WHERE
		U.RemovalRequested < GETDATE()

	IF @debug = 1 BEGIN PRINT 'DELETE FROM tblFriend' END
	DELETE FROM FD
		FROM tblFriend FD
	WHERE
		FD.UserId IN (SELECT Id FROM AspNetUsers WHERE RemovalRequested < GETDATE())
	OR	FD.RelatedUserId IN (SELECT Id FROM AspNetUsers WHERE RemovalRequested < GETDATE())

	IF @debug = 1 BEGIN PRINT 'DELETE FROM tblBlocked' END
	DELETE FROM BD
		FROM tblBlocked BD
	WHERE
		BD.UserId IN (SELECT Id FROM AspNetUsers WHERE RemovalRequested < GETDATE())
	OR	BD.RelatedUserId IN (SELECT Id FROM AspNetUsers WHERE RemovalRequested < GETDATE())
	
	IF @debug = 1 BEGIN  PRINT 'DELETE FROM tblReportRequest' END
	DELETE FROM RR
		FROM AspNetUsers U
		INNER JOIN tblReportRequest RR ON RR.UserId = U.Id
	WHERE
		U.RemovalRequested < GETDATE()

	--IF @debug = 1 BEGIN PRINT 'DELETE FROM tblWallet' END
	--DELETE FROM tblWallet WHERE UserId IN (SELECT Id FROM AspNetUsers WHERE RemovalRequested > GETDATE())

	IF @debug = 1 BEGIN PRINT 'DELETE FROM AspNetUsers' END
	DELETE FROM AspNetUsers WHERE RemovalRequested < GETDATE()

	IF @commit = 1 BEGIN
		COMMIT TRAN
	END ELSE BEGIN
		ROLLBACK TRAN
	END
END
GO

EXEC prcUserPurge 0, 1
GO
