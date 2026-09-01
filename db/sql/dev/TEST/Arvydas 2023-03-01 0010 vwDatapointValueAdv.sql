DROP VIEW IF EXISTS vwDatapointValueAdv
GO

CREATE VIEW vwDatapointValueAdv AS

SELECT 
	DV.*,
	LAG([Value], 1) OVER (PARTITION BY [DatapointId] ORDER BY [Date] ASC) AS [PrevValue],
	DV.[Value] - LAG([Value], 1) OVER (PARTITION BY [DatapointId] ORDER BY [Date] ASC)  AS [DiffValue]
FROM
	tblDatapointValue DV

GO

-- TEST
SELECT * 
FROM vwDatapointValueAdv VW 
ORDER BY VW.[DatapointId], VW.[Date]