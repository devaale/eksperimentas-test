CREATE FUNCTION [dbo].[fncThermalComfort] (
	@param1 DECIMAL(18,4),
	@param2 DECIMAL(18,4),
	@param3 DECIMAL(18,4)
) 
	RETURNS DECIMAL(18,4)
BEGIN
DECLARE @PMV DECIMAL(18,4)
DECLARE @PPD DECIMAL(18,4)
DECLARE @retVal DECIMAL(18,4)

-- Get FangerPMV Value
SET @PMV = @param2

-- Calculate PPD
SET @PPD = 100 - 95 * Exp(-0.03353 * POWER(@PMV, 4) - 0.2197 * POWER(@PMV, 2))

-- Calculate Thermal Comfort
SET @retval = (@param1 - @PPD) / @param3

RETURN @retval

END