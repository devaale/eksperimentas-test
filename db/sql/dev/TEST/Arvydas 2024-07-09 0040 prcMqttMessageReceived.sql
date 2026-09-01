DROP PROCEDURE IF EXISTS prcMqttMessageReceived
GO
CREATE PROCEDURE [dbo].[prcMqttMessageReceived] (
	@url varchar(512),
	@topic nvarchar(64),
	@payload nvarchar(max),
	@date datetime = NULL) AS BEGIN

	BEGIN TRAN

	IF @date IS NULL BEGIN
		SET @date = GETDATE()
	END

	IF EXISTS(SELECT * FROM [tblMqttTopic] WHERE [Url] = @url AND [Topic] = @topic) BEGIN

		UPDATE [tblMqttTopic]
		SET [Date] = @date
		WHERE [Url] = @url AND [Topic] = @topic

	END ELSE BEGIN
		INSERT INTO [tblMqttTopic] (
			[Url],	[Topic],	[Date]
		) VALUES (
			@url,	@topic,		@date
		)
	END

	COMMIT TRAN

END
GO