/*
   2022 m. gruodžio 22 d., ketvirtadienis18:59:59
   User: sa
   Server: TERRA
   Database: ExperimentDB
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
CREATE TABLE dbo.tblTokenTransaction
	(
	Id uniqueidentifier NOT NULL,
	Date datetime NOT NULL,
	SenderUserId nvarchar(128) NOT NULL,
	ReceiverUserId nvarchar(128) NOT NULL,
	Tokens int NOT NULL,
	Status tinyint NOT NULL,
	StatusUserId nvarchar(128) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tblTokenTransaction ADD CONSTRAINT
	DF_tblTokenTransaction_Id DEFAULT NEWID() FOR Id
GO
ALTER TABLE dbo.tblTokenTransaction ADD CONSTRAINT
	DF_tblTokenTransaction_Date DEFAULT GETDATE() FOR Date
GO
ALTER TABLE dbo.tblTokenTransaction ADD CONSTRAINT
	DF_tblTokenTransaction_Tokens DEFAULT 0 FOR Tokens
GO
ALTER TABLE dbo.tblTokenTransaction ADD CONSTRAINT
	DF_tblTokenTransaction_Status DEFAULT 0 FOR Status
GO
ALTER TABLE dbo.tblTokenTransaction ADD CONSTRAINT
	PK_tblTokenTransaction PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblTokenTransaction SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblTokenTransaction', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblTokenTransaction', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblTokenTransaction', 'Object', 'CONTROL') as Contr_Per 