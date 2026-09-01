ALTER PROCEDURE [dbo].[prcOrderConfirm] (
	@ip nvarchar(32),
	@orderNo nvarchar(16),
	@data varchar(max),
	@success bit
) AS BEGIN

	BEGIN TRAN

	DECLARE @OrderId UNIQUEIDENTIFIER
	DECLARE @UserId NVARCHAR(128)
	DECLARE @UsedTokens INT
	DECLARE @Completed DATETIME

	DECLARE @now DATETIME
	DECLARE @longestLastingLicenseDate DATETIME
	DECLARE @newLicenseStartDate DATETIME

	-- Retrieving order info
	SELECT 
		@OrderId = O.[Id], 
		@UserId = O.[UserId], 
		@UsedTokens = O.[UsedTokens],
		@Completed = O.[Completed] 
	FROM 
		[tblOrder] O
	WHERE 
		O.[OrderNo] = @orderNo

	IF @Completed IS NULL BEGIN

		SET @now = GETDATE()

		-- Updating order info
		UPDATE 
			[tblOrder]
		SET
			[CompletedIp] = @ip,
			[Completed] = @now,
			[Data] = @data,
			[State] = CASE WHEN @success = 1 THEN 2 ELSE 4 END
		WHERE
			[Id] = @OrderId

		-- Only in case of success
		IF @success = 1 BEGIN

			DECLARE @licenseType INT
			DECLARE @NumMonths INT

			-- Retrieve Order/License details
			SELECT 
				@licenseType = LicenseType,
				@NumMonths = OD.NumMonths
			FROM 
				[tblOrderDetail] OD
			WHERE 
				OD.[OrderId] = @OrderId


			-- Determine the new license start date
			SELECT 
				@longestLastingLicenseDate = ISNULL(MAX([ValidUntil]), CAST('1900-01-01' AS DATETIME))
			FROM 
				[tblLicense]
			WHERE 
				[UserId] = @UserId
			AND [Type] = @licenseType
			AND [Active] = 1

			-- IF last specific type license date is longer then we prolonging it
			-- If not, we starting new license from now
			IF @longestLastingLicenseDate > @now BEGIN
				SET @newLicenseStartDate = @longestLastingLicenseDate
			END ELSE BEGIN
				SET @newLicenseStartDate = @now
			END

			-- CREATE NEW LICENSE
			INSERT INTO
				[tblLicense]
			(
				[UserId],				[OrderId],		[Type],			
				[ValidFrom],			[ValidUntil],	
				[Active]
			) VALUES (
				@UserId,				@OrderId,		@licenseType,
				@newLicenseStartDate,	DATEADD(MONTH, @NumMonths, @now),
				@success
			)

			-- IF tokens were used, removing used tokens from user's profile
			IF @UsedTokens > 0 BEGIN
				UPDATE 
					[AspNetUsers]
				SET
					[Tokens] = [Tokens] - @UsedTokens
				WHERE
					[Id] = @UserId
			END

		END
		
	END

	COMMIT TRAN

END
