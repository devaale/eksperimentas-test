/*
   2023 m. gegužės 5 d., penktadienis16:06:09
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
CREATE TABLE dbo.tblBlockchainLog
	(
	Id int NOT NULL IDENTITY (1, 1),
	UserId nvarchar(128) NULL,
	Created datetime NOT NULL,
	RequestUri varchar(512) NULL,
	ReqestParams ntext NULL,
	Result ntext NULL,
	Status int NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.tblBlockchainLog ADD CONSTRAINT
	DF_Table_1_Date DEFAULT GETDATE() FOR Created
GO
ALTER TABLE dbo.tblBlockchainLog ADD CONSTRAINT
	PK_tblBlockchainLog PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblBlockchainLog SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblBlockchainLog', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblBlockchainLog', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblBlockchainLog', 'Object', 'CONTROL') as Contr_Per 