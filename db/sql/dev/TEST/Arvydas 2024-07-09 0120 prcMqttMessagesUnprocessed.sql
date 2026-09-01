DROP PROCEDURE IF EXISTS [prcMqttMessagesUnprocessed]
GO

CREATE PROCEDURE [prcMqttMessagesUnprocessed] AS BEGIN
	SELECT
		MM.[Id]
		,MM.[DeviceId]
		,MM.[DeviceTopicId]
		,MM.[Topic]
		,MM.[Payload]
	FROM
		[tblMqttMessage] MM
	WHERE 
		[FinishDate] IS NULL
	ORDER BY
		[CreationDate] ASC
END
GO