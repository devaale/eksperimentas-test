SELECT * FROM tblDevice

SELECT * FROM tblDatapoint

SELECT DatapointId, count(*)
FROM [tblDatapointValue] 
GROUP BY [DatapointId]


/*
-- Purge all
DELETE FROM tblDevice
DELETE FROM tblDatapoint
DELETE FROM tblDatapointValue

*/