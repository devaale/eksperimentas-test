SELECT DP.Id, DP.Name, DV.*
FROM tblDatapoint DP 
INNER JOIN tblDatapointValue DV
	ON DV.DatapointId = DP.Id
WHERE DV.Date = (
	SELECT MAX(DV1.Date)
	FROM tblDatapointValue DV1 
	WHERE DV1.DatapointId = DV.DatapointId
)