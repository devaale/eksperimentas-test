DROP PROCEDURE IF EXISTS [prcUserAll]
GO

CREATE PROCEDURE [prcUserAll]
AS BEGIN

	SELECT * FROM [AspNetUsers]

END
GO

EXEC [prcUserAll]
GO








DROP PROCEDURE IF EXISTS prcWalletPrimarySystem
GO

CREATE PROCEDURE prcWalletPrimarySystem AS 
BEGIN

	SELECT
		W.* 
	FROM 
		[tblWallet] W
	WHERE 
		W.[System] = 1
	AND W.[Primary] = 1

END
GO






DROP PROCEDURE IF EXISTS prcWalletOfUser
GO

CREATE PROCEDURE prcWalletOfUser (
	@userId nvarchar(128)
) AS 
BEGIN

	SELECT
		W.* 
	FROM 
		[tblWallet] W
	WHERE 
		W.UserId = @userId

END
GO








DROP PROCEDURE IF EXISTS [prcWalletUpdate]
GO

CREATE PROCEDURE [prcWalletUpdate] (
	@userId nvarchar(128)
	,@address nvarchar(128)
	,@privateKey text
	,@publicKey text
) AS BEGIN

	BEGIN TRAN

	IF EXISTS(SELECT * FROM tblWallet WHERE UserId = @userId) BEGIN

		UPDATE
			[tblWallet]
		SET
			[Address] = @address
			,[PrivateKey] = @privateKey
			,[PublicKey] = @publicKey
		WHERE
			UserId = @userId

	END ELSE BEGIN
		
		INSERT tblWallet (
			[UserId],	[Address],	[PrivateKey],	[PublicKey],
			[System],	[Primary]
		) VALUES (
			@userId,	@address,	@privateKey,	@publicKey,
			0,			0
		)

	END

	COMMIT TRAN
	
END
GO

--EXEC [prcUserBlockchainFix] '34406CE2-C9E4-40F8-8C5A-E8CF97ADE6C1', 'lol1', 'lol2', 'lol2'






DROP PROCEDURE IF EXISTS [prcBlockchainLog]
GO

CREATE PROCEDURE [prcBlockchainLog] (

	@userId nvarchar(128)
	,@requestUri varchar(512)
	,@reqestParams ntext
	,@result ntext
	,@status int

) AS BEGIN

	INSERT INTO [tblBlockchainLog] (

		[UserId]
		,[RequestUri]
		,[ReqestParams]
		,[Result]
		,[Status]

	) VALUES (

		@userId
		,@requestUri
		,@reqestParams
		,@result
		,@status

	)

END
GO





DROP PROCEDURE IF EXISTS prcUserTokenBalanceUpdate
GO

CREATE PROCEDURE prcUserTokenBalanceUpdate (
	@userId nvarchar(128)
	,@tokens int
) AS BEGIN

	UPDATE 
		AspNetUsers
	SET
		Tokens = @tokens
	WHERE Id = @userId

END
GO
