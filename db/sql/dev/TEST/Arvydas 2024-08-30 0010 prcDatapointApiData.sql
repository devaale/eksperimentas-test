DROP PROCEDURE IF EXISTS prcDatapointApiData
GO
CREATE PROCEDURE prcDatapointApiData (
	@DeviceId int
) AS BEGIN

-- DEBUG
--DECLARE @DeviceId int
--SET @DeviceId = 4873

	SELECT
		DTP.[Id] [DatapointId]
		,DTP.[Alias]
		,DTP.[Multiplier]
		,DTS.[Direction]
		,DTS.[ValueType]
		,DTS.[Mandatory]
		,[DV].[Value]

	-- We just make all by the book, from the device
	FROM
		tblDevice DEV

	-- But really we need just datapoints, reutilizing structure and relationships
	INNER JOIN
		tblDatapoint DTP ON 
			DTP.DeviceId = DEV.Id

	-- Filtering of unneeded datapoints and getting processing rules from this table
	INNER JOIN
		tblDatapointSetting DTS ON
			DTS.[Name] = DTP.[Alias]
		AND DTS.Protocol = 100

	-- Now we need values, which may not present
	LEFT JOIN  (
		SELECT
			DV.DatapointId
			,MAX(DV.[Date]) [Date]
			,DV.[Value]
		FROM
			tblDatapointValue DV
		GROUP BY 
			DV.DatapointId
			,DV.[Value]
	) DV ON DV.DatapointId = DTP.Id

	WHERE
		DEV.Id = @DeviceId
		--DST.Protocol = 100 -- API

END
GO

INSERT INTO tblDatapointValue (
	DatapointId, Value
) VALUES
	(1086693, 32),
	(1086694, 36),
	(1087678, 19)

EXEC [prcDatapointApiData] 4873 -- local Arvydas sandbox id