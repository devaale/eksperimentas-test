SELECT
	SV._dataPointId
	,DP.[name]
	,YEAR(SV.[date])
	,COUNT(*)
FROM 
	EnergusDB1.dbo.tblScanValues SV
INNER JOIN
	EnergusDB1.dbo.tblDataPoint DP
		ON DP._id = SV._dataPointId
WHERE
	SV.value IS NOT NULL

GROUP BY
	SV._dataPointId
	,DP.[name]
	,YEAR(SV.[date])

--HAVING YEAR(SV.[date]) = 2023
	
ORDER BY
	YEAR(SV.[date]) DESC
	,SV._dataPointId




