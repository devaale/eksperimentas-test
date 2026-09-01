ALTER PROCEDURE [dbo].[prcCalcDepreciation] (
	@DeviceId int
) AS BEGIN

	SET NOCOUNT ON;	-- saves the day as elsewhere PHP MSSQL Driver failing to retrieve the result.

	DECLARE @GL AS DECIMAL(18,4)
	DECLARE @A AS DECIMAL(18,4)
	DECLARE @LIR AS DECIMAL(18,4)
	DECLARE @RL AS DECIMAL(18,4)
	DECLARE @C AS DECIMAL(18,4)
	DECLARE @SD AS DECIMAL(18,4)

	DECLARE @result AS DECIMAL(18,4)

	SELECT
		@GL = DEV.DeprGL,
		@A = DEV.DeprA,
		@LIR = DEV.DeprLIR,
		@RL = DEV.DeprRL,
		@C = DEV.DeprC,
		@SD = DEV.DeprSD
	FROM
		tblDevice DEV
	WHERE
		DEV.Id = @DeviceId

	SET @result = [dbo].[fncDeprecation](@GL, @A, @LIR, @RL, @C, @SD)

	SELECT @result

END
GO

ALTER FUNCTION [dbo].[fncDeprecation] (
	@GL DECIMAL(18,4),
	@A DECIMAL(18,4),
	@LIR DECIMAL(18,4),
	@RL DECIMAL(18,4),
	@C DECIMAL(18,4),
	@SD DECIMAL(18,4))

	RETURNS DECIMAL(18,4)
	AS
BEGIN
	DECLARE @retVal DECIMAL(18,4)
	SET @retVal = 1.0/(LOG((@GL*@RL+@C-((@C+@GL)/2.0-@LIR))/((@C+@GL)/2.0-@LIR))-LOG((@C-((@C+@GL)/2.0-@LIR))/(@GL*@RL+(@C+@GL)/2.0-@LIR)))*(LOG((@GL*@RL+@C-((@C+@GL)/2.0-@LIR))/((@C+@GL)/2.0-@LIR))-LOG((@GL*@RL+@C)/(@A+(@C+@GL)/2.0-@LIR)-1.0))

	RETURN @retVal
END
GO

EXEC prcGetVirtualDatapoints;

--EXEC prcCalcDepreciation 2851;

DECLARE @DeviceId int
SET @DeviceId = 2851

	DECLARE @GL AS DECIMAL(18,4)
	DECLARE @A AS DECIMAL(18,4)
	DECLARE @LIR AS DECIMAL(18,4)
	DECLARE @RL AS DECIMAL(18,4)
	DECLARE @C AS DECIMAL(18,4)
	DECLARE @SD AS DECIMAL(18,4)

	DECLARE @result AS DECIMAL(18,4)

	SELECT
		@GL = DEV.DeprGL,
		@A = DEV.DeprA,
		@LIR = DEV.DeprLIR,
		@RL = DEV.DeprRL,
		@C = DEV.DeprC,
		@SD = DEV.DeprSD
	FROM
		tblDevice DEV
	WHERE
		DEV.Id = @DeviceId

	SET @result = [dbo].[fncDeprecation](@GL, @A, @LIR, @RL, @C, @SD)

	SELECT @result

	ISSUES IN FUNCTION