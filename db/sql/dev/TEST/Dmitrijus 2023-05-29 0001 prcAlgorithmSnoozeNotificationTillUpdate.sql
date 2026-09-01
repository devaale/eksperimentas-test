CREATE PROCEDURE [dbo].[prcAlgorithmSnoozeNotificationTillUpdate] (
	@AlgorithmId int, 
	@SnoozeNotificationTill datetime
) AS BEGIN

	UPDATE
		tblAlgorithm
	SET
		SnoozeNotificationTill = @SnoozeNotificationTill,
		[Read] = NULL
	WHERE
		Id = @AlgorithmId
		
END