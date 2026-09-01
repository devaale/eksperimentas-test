DROP PROCEDURE IF EXISTS prcMqttValueSave
GO

CREATE PROCEDURE [dbo].[prcMqttValueSave](
	@deviceId int,
	@topic nvarchar(64),
	@path nvarchar(128),
	@value decimal(18,4),
	@date datetime = NULL) AS
BEGIN

	/*
		2024-07-10 Creation (AG)
	*/

	BEGIN TRAN

	IF @date IS NULL BEGIN
		SET @date = GETDATE()
	END

	If @path = '' BEGIN
		SET @path = NULL
	END

	-- Extracting datapoint
	DECLARE @datapointId int--, @_active bit
	-- From all, except deleted
	SELECT 
		@datapointId = Id--, @_active = _active
	FROM 
		tblDataPoint
	WHERE 
		[DeviceId] = @deviceId
	AND [Topic] = @topic
	AND ((@path IS NOT NULL AND [Path] = @path) OR (@path IS NULL AND [Path] IS NULL))
	AND [_deleted] = 0

	-- If datapoint not found, creating new one
	IF @datapointId IS NULL BEGIN
		INSERT INTO tblDataPoint (
			DeviceId,		[DatapointType],	[name],		
			[topic],		[Path],				[_active]--,	DeviceProtocol (computed from Device)
		) VALUES (
			@deviceId,		1,					CASE WHEN @path IS NULL THEN 'Default' ELSE @path END, 
			@topic,			@path,				1--,			30 -- MQTT
		)

		SET @datapointId = @@IDENTITY
	END

	-- IF _datapointId NOT NULL?
	IF @datapointId IS NOT NULL BEGIN

		-- If datapoint value not null?
		IF @value IS NOT NULL BEGIN

			-- NOT NULL VALUE CASE
			INSERT INTO tblDatapointValue(
				[Date]
				,[DatapointId]
				,[Value]
			) VALUES (
				@date,
				@datapointId,
				@value
			)

		END ---ELSE BEGIN

			-- NULL VALUE CASE
			-- LOGIC: If last value is NULL, we won't insert NULL again.
			---DECLARE @lastValueIsNull BIT
			--- Function dbo.fncDatapointLastValueIsNull exist only in ESE (not ported)
			---SET @lastValueIsNull = dbo.fncDatapointLastValueIsNull(@_datapointId)

			---IF @lastValueIsNull = 0 BEGIN
			---	INSERT INTO tblDatapointValue(
			---		[Date]
			---		,[DatapointId]
			---		,[Value]
			---	) VALUES (
			---		@date,
			---		@datapointId,
			---		@value
			---	)
			---END
		---END

	END

	COMMIT TRAN

END
GO