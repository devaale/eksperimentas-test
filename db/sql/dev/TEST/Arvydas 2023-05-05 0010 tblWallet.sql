/*
   2023 m. gegužės 5 d., penktadienis15:11:37
   User: sa
   Server: TERRA
   Database: EnergusDB1
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.tblWallet
	(
	Id uniqueidentifier NOT NULL,
	UserId nvarchar(128) NULL,
	PrivateKey text NOT NULL,
	PublicKey text NOT NULL,
	System bit NOT NULL,
	[Primary] bit NOT NULL,
	Created datetime NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.tblWallet ADD CONSTRAINT
	DF_tblWallet_Id DEFAULT newid() FOR Id
GO
ALTER TABLE dbo.tblWallet ADD CONSTRAINT
	DF_tblWallet_System DEFAULT 0 FOR System
GO
ALTER TABLE dbo.tblWallet ADD CONSTRAINT
	DF_Table_1_Primary DEFAULT 0 FOR [Primary]
GO
ALTER TABLE dbo.tblWallet ADD CONSTRAINT
	DF_tblWallet_Created DEFAULT GETDATE() FOR Created
GO
ALTER TABLE dbo.tblWallet ADD CONSTRAINT
	PK_tblWallet PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblWallet SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblWallet', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblWallet', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblWallet', 'Object', 'CONTROL') as Contr_Per 