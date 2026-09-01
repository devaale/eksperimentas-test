DROP VIEW IF EXISTS vwDatapointValueAdv
GO

CREATE VIEW vwDatapointValueAdv AS

SELECT 
	DV.*,
	LAG([Value], 1) OVER (PARTITION BY [DatapointId] ORDER BY [Date] ASC) AS [PrevValue],
	LAG([Value], 1) OVER (PARTITION BY [DatapointId] ORDER BY [Date] ASC) - DV.[Value] AS [DiffValue]
FROM
	tblDatapointValue DV

GO

-- TEST
SELECT * 
FROM vwDatapointValueAdv VW 
ORDER BY VW.[DatapointId], VW.[Date]