USE [ExperimentDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[prcCalcDepreciation] (
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