-- Environmental Impact. Change amount of parameters
UPDATE [dbo].[tblDatapointFormula]
SET
	NumDatapoints = 3
WHERE
	Id = 1010

-- Thermal comfort. Change amount of parameters
UPDATE [dbo].[tblDatapointFormula]
SET
	NumDatapoints = 3
WHERE
	Id = 1020

-- Delete FangerPMV
DELETE FROM
	[dbo].[tblDatapointFormula]
WHERE
	Id = 1030