DROP PROCEDURE IF EXISTS prcMqttServiceData
GO

CREATE PROCEDURE [dbo].[prcMqttServiceData] AS
BEGIN
	/*
		Procedure for Mqtt Service

		2024-07-10 Adapted from ESE (AG)
	*/
	BEGIN TRAN

	-- MQTT DEVICES (Protocol==30)
	SELECT 
		DEV.[Id]					--[_id]
		,DEV.[Name]					--[name]
		,DEV.[Url]					--[url]
		,DEV.[ClientUsername]		[username]
		,DEV.[ClientPassword]		[password]
		,DEV.[Interval]				[interval]
		,DEV.[lastScanTime]			[lastScanTime]
		,DEV.[projectedScanTime]	[projectedScanTime]
	
	FROM 
		tblDevice DEV

	WHERE
		-- Only MQTT type
		DEV.Protocol = 30 -- MQTT
	AND LEN(TRIM(ISNULL(DEV.[Url], ''))) > 0
	--ESE-AND DEV._deleted = 0
	--ESE-AND DEV._active = 1
	--ESE-AND DEV._id IN (SELECT _deviceId FROM vwHealthyToDeviceStructure)
	AND EXISTS(
		SELECT * FROM tblDeviceTopic DT WHERE DT.DeviceId = DEV.Id AND LEN(TRIM(ISNULL(DT.Topic,''))) > 0
	)

	-- MQTT devices topics
	SELECT
		DT.DeviceId
		,DT.Topic
	FROM 
		tblDeviceTopic DT
	WHERE 
		--ESE-DT.DeviceId IN (SELECT DeviceId FROM vwHealthyToDeviceStructure)
		DT.DeviceId IN (SELECT Id FROM tblDevice WHERE Protocol = 30)
	AND LEN(TRIM(ISNULL(DT.topic,''))) > 0

	-- MQTT Datapoints
	SELECT 
		DTP.[Id]				--[_id]
		,DTP.[DeviceId]			--[_deviceId]
		,DTP.[Name]				--[name]
		,DTP.[Topic]			--[topic]
		,DTP.[Path]				--[path]
	FROM
		tblDatapoint DTP
	--ESE-INNER JOIN 
	--ESE-	vwHealthyParamStructure HPS ON HPS._dataPointId = DTP._id
	WHERE
		DTP.DeviceProtocol = 30 -- MQTT
	AND DTP.DatapointType = 1
	-- Filter also only devices with topics
	AND EXISTS (
		SELECT * FROM tblDeviceTopic DT WHERE DT.DeviceId = DTP.DeviceId AND LEN(TRIM(ISNULL(DT.Topic,''))) > 0
	)

	
	COMMIT TRAN

END
GO

[prcMqttServiceData]
GO