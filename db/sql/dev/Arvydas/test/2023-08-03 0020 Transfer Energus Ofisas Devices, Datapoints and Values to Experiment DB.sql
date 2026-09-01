	DECLARE @eseObjectId int, @expObjectId int, @eseDeviceId int
	SELECT @eseObjectId = 1, @eseDeviceId = 27 -- ENERGUS OFISAS
	SELECT @expObjectId = 2 -- Kavalkas

	BEGIN TRAN

	PRINT 'PURGE DATA..'
	DELETE FROM ExperimentDB.dbo.tblDatapointValue
	DELETE FROM ExperimentDB.dbo.tblDatapoint
	DELETE FROM ExperimentDB.dbo.tblDevice


	PRINT 'DEVICES CYCLE..'
	DECLARE @deviceId int, @deviceName nvarchar(256)
	DECLARE @newDeviceId int

	DECLARE devices CURSOR FOR 
	SELECT _id, [name] 
		FROM EnergusDB1.dbo.tblDevice
			WHERE _objectId = @eseObjectId
			AND _id = @eseDeviceId
	OPEN devices  

	FETCH NEXT FROM devices INTO @deviceId, @deviceName
	WHILE @@FETCH_STATUS = 0  
	BEGIN
		INSERT INTO ExperimentDB.dbo.tblDevice (
			[ObjectId],		[Name],			[Type]
		) VALUES (
			@expObjectId,	@deviceName,	0
		)
		SET @newDeviceId = @@IDENTITY

		PRINT 'DATAPOINTS CYCLE..'
		DECLARE @datapointId int, @datapointName nvarchar(256)
		DECLARE @newDatapointId int

		DECLARE datapoints CURSOR FOR 
		SELECT DP._id, DP.[name]
		FROM EnergusDB1.dbo.tblDataPoint DP
		INNER JOIN EnergusDB1.dbo.tblScanValues SV ON SV._dataPointId = DP._id
		WHERE _deviceId = @deviceId AND YEAR(SV.[date]) = 2023
		GROUP BY DP._id, DP.[name]
		HAVING COUNT(*) > 15000
		
		OPEN datapoints  

		FETCH NEXT FROM datapoints INTO @datapointId, @datapointName
		WHILE @@FETCH_STATUS = 0  
		BEGIN

			INSERT INTO ExperimentDB.dbo.tblDatapoint (
				[Name], [DeviceId]
			) VALUES (
				@datapointName, @newDeviceId
			)
			SET @newDatapointId = @@IDENTITY

			PRINT 'DATAPOINT VALUES..'
			INSERT INTO ExperimentDB.dbo.tblDatapointValue (
				[DatapointId]
				,[Date]
				,[Value]
			)
			SELECT 
				@newDatapointId
				,SV.[date]
				,SV.[value]
			FROM 
				EnergusDB1.dbo.tblScanValues SV
			WHERE
				_dataPointId = @datapointId
			AND	[value] IS NOT NULL


			FETCH NEXT FROM datapoints INTO @datapointId, @datapointName
		END

		CLOSE datapoints
		DEALLOCATE datapoints 

		FETCH NEXT FROM devices INTO @deviceId, @deviceName
	END

	CLOSE devices
	DEALLOCATE devices 

	COMMIT TRAN

