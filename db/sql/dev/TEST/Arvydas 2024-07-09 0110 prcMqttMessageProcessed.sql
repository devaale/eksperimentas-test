DROP PROCEDURE IF EXISTS prcMqttMessageProcessed
GO

CREATE PROCEDURE [dbo].[prcMqttMessageProcessed] (
	@id int,
	@state int
) AS BEGIN

	/*
		Once this procedure called on specific message and it considered will be as processed

		2024-05-15 Created (AG)
	*/

	UPDATE
		[tblMqttMessage]
	SET
		[FinishDate] = GETDATE(),
		[State] = @state
	WHERE
		[Id] = @id
END
GO