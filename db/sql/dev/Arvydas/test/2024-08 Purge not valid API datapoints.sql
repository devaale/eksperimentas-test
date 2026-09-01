/*
DECLARE @VALID TABLE
(
	Alias nvarchar(256)
)

INSERT INTO @VALID (Alias) VALUES 
('temp_inside'),
('temp_outside'),
('temp_target_now'),
('temp_target_user_in_building'),
('temp_target_user_away'),
('user_in_building_from'),
('user_in_building_till'),
('time_now'),
('decision')
*/
-- VALID DPS of UnitId = 1 NOT UnitId = 0 NOT! Those have different datapoints
--SELECT * FROM @VALID

-- REQ devices
--SELECT Id, [Name] FROM tblDevice DEV WHERE DEV.Protocol = 100 AND DEV.UnitId = 1

-- Datapoints which do not belong to currently needed AI support datapoints of Devices with UnitId = 1 (IMPORTANT)
SELECT
	--DEV.Id	"DEV_ID", 
	--DEV.Name	"DEV",
	DTP.Id		"DTP_ID",
	DTP.Name	"DTP"
FROM
	tblDevice DEV 
INNER JOIN
	tblDatapoint DTP ON DTP.DeviceId = DEV.Id
WHERE
	DEV.Protocol = 100
AND DEV.UnitId = 1
AND DTP.Alias NOT IN (
	SELECT [Name] FROM tblDatapointSetting WHERE Protocol = 100
)

-- FIRST DELETE tblDatapointValues which WERE redundant
--SELECT * FROM tblDatapointValue
DELETE FROM tblDatapointValue
WHERE DatapointId IN  (
	SELECT
		DTP.Id
	FROM
		tblDevice DEV 
	INNER JOIN
		tblDatapoint DTP ON DTP.DeviceId = DEV.Id
	WHERE
		DEV.Protocol = 100
	AND DEV.UnitId = 1
	AND DTP.Alias NOT IN (
		SELECT [Name] FROM tblDatapointSetting WHERE Protocol = 100
	)
)

--SELECT * FROM tblDatapoint
DELETE FROM tblDatapoint
WHERE Id IN (
	SELECT
		DTP.Id
	FROM
		tblDevice DEV 
	INNER JOIN
		tblDatapoint DTP ON DTP.DeviceId = DEV.Id
	WHERE
		DEV.Protocol = 100
	AND DEV.UnitId = 1
	AND DTP.Alias NOT IN (
		SELECT [Name] FROM tblDatapointSetting WHERE Protocol = 100
	)
)


SELECT
	DEV.Id		"DEV_ID",
	DEV.Name	"DEV",
	DTP.Id		"DTP_ID",
	DTP.Name	"DTP"
FROM 
	tblDevice DEV
INNER JOIN 
	tblDatapoint DTP ON DTP.DeviceId = DEV.Id
WHERE
	DEV.Protocol = 100 -- API


