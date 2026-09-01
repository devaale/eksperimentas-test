ALTER PROCEDURE [dbo].[prcGetVirtualDatapoints] (@includeChains bit = 0) AS BEGIN

	-- Table 1 - Virtual datapoints
	SELECT *
	FROM
		tblDatapoint DTP 
	WHERE
		DTP.DatapointType = 2
	ORDER BY
		DTP.[Order] ASC

	IF @includeChains = 1 BEGIN
		-- Table 2 - Only valid virtual datapoints chains
		SELECT *
		FROM
			tblDatapointFormulaChain DFC
		WHERE
			DFC.DatapointId IN (SELECT Id FROM tblDatapoint WHERE DatapointType = 2)
	END
		
END
GO

