CREATE FUNCTION [dbo].[fncEnvironmentalImpact] (
	@param1 DECIMAL(18,4),
	@param2 DECIMAL(18,4),
	@param3 DECIMAL(18,4)
) 
	RETURNS DECIMAL(18,4)
BEGIN
DECLARE @retVal DECIMAL(18,4)

SET @retval = @param1 * @param2 / @param3

RETURN @retval

END