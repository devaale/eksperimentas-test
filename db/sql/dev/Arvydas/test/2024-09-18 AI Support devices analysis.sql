
DECLARE @usedId uniqueidentifier
SET @usedId = '{684d1c94-a60c-4967-ba04-8930e3608ed5}'

SELECT * 
FROM AspNetUsers 
WHERE Id = @usedId 
ORDER BY [Name]

DECLARE @objectId int
SET @objectId = 38

SELECT * FROM tblObject WHERE UserId = @usedId

SELECT * 
FROm
	tblDevice 
WHERE
	ObjectId = @objectId
AND Protocol = 100


DECLARE @RelevantDeviceid int

SELECT @RelevantDeviceid = Id 
From tblDevice 
WHERE ObjectId = 38

SELECT @RelevantDeviceid  "@RelevantDeviceid"
/*
SELECT Id, Name, Alias
FROM tblDatapoint
WHERE 
	DeviceId = @RelevantDeviceid
AND Alias IS NOT NULL

SELECT * 
FROM tblDatapointValue 
WHERE DatapointId = 347
ORDER BY [Date] DESC

--SELECT * FROM tb*/