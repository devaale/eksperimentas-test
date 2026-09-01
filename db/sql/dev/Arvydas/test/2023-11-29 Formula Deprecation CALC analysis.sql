DECLARE @deviceId int
SET @deviceId = 24

	DECLARE @GL AS DECIMAL(18,4)
	DECLARE @A AS DECIMAL(18,4)
	DECLARE @LIR AS DECIMAL(18,4)
	DECLARE @RL AS DECIMAL(18,4)
	DECLARE @C AS DECIMAL(18,4)
	DECLARE @SD AS DECIMAL(18,4)

	DECLARE @result AS DECIMAL(18,4)

	SELECT * FROM tblDevice WHERE Id = @deviceId

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

	SELECT
		@GL "GL",
		@A "A",
		@LIR "LIR",
		@RL "RL",
		@C "C",
		@SD "SD"

	--SET @result = [dbo].[fncDeprecation](@GL, @A, @LIR, @RL, @C, @SD)
	DECLARE @log1 decimal(18,4), @log2 decimal(18,4), @log3 decimal(18,4), @log4 decimal(18,4)

/*	
		SET @retVal = 1.0 / 
		(
			LOG((@GL*@RL+@C-((@C+@GL)/2.0-@LIR))/((@C+@GL)/2.0-@LIR)) - 
			LOG((@C-((@C+@GL)/2.0-@LIR))/(@GL*@RL+(@C+@GL)/2.0-@LIR))
		) *
		(
			LOG((@GL*@RL+@C-((@C+@GL)/2.0-@LIR))/((@C+@GL)/2.0-@LIR)) - 
			LOG((@GL*@RL+@C)/(@A+(@C+@GL)/2.0-@LIR)-1.0)
		)
*/
	PRINT 'Calc LOG1'
	SET @log1 = LOG((@GL*@RL+@C-((@C+@GL)/2.0-@LIR))/((@C+@GL)/2.0-@LIR))

	PRINT 'Calc LOG2'
	SET @log2 = LOG((@C-((@C+@GL)/2.0-@LIR))/(@GL*@RL+(@C+@GL)/2.0-@LIR))

	PRINT 'Calc LOG3'
	SET @log3 = LOG((@GL*@RL+@C-((@C+@GL)/2.0-@LIR))/((@C+@GL)/2.0-@LIR))

	PRINT 'Calc LOG4'
	SET @log4 = LOG((@GL*@RL+@C)/(@A+(@C+@GL)/2.0-@LIR)-1.0)

	PRINT 'SELECTING ALL LOG NODES'
	SELECT @log1 LOG1, @log2 LOG2, @log3 LOG3, @log4 LOG4

	PRINT 'DETAILED'
	DECLARE @LOG1_1 DECIMAL(18,4), @LOG1_2 DECIMAL(18,4)
	-- LOG((@GL*@RL+@C-((@C+@GL)/2.0-@LIR))/((@C+@GL)/2.0-@LIR))
	PRINT 'CALC @LOG1_1'
	SET @LOG1_1 = (@GL*@RL+@C-((@C+@GL)/2.0-@LIR))
	PRINT 'CALC @LOG1_2'
	SET @LOG1_2 = ((@C+@GL)/2.0-@LIR)

	SELECT @LOG1_1, @LOG1_2--, @LOG1_1 / @LOG1_2 == 0! NULL Divide By Zero

	--SELECT @result


--EXEC prcGetVirtualDatapoints 
--SELECT * FROM tblDatapoint