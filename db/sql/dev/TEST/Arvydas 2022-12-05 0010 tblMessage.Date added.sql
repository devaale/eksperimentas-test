/*
   2022 m. gruodžio 5 d., pirmadienis22:58:52
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
ALTER TABLE dbo.tblMessage
	DROP CONSTRAINT FK_tblMessage_AspNetUsers_SenderUserId
GO
ALTER TABLE dbo.tblMessage
	DROP CONSTRAINT FK_tblMessage_AspNetUsers_ReceiverUserId
GO
ALTER TABLE dbo.AspNetUsers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_tblMessage
	(
	Id int NOT NULL IDENTITY (1, 1),
	Date datetime NOT NULL,
	SenderUserId nvarchar(128) NOT NULL,
	ReceiverUserId nvarchar(128) NOT NULL,
	Body nvarchar(MAX) NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tblMessage SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_tblMessage ADD CONSTRAINT
	DF_tblMessage_Date DEFAULT GETDATE() FOR Date
GO
SET IDENTITY_INSERT dbo.Tmp_tblMessage ON
GO
IF EXISTS(SELECT * FROM dbo.tblMessage)
	 EXEC('INSERT INTO dbo.Tmp_tblMessage (Id, SenderUserId, ReceiverUserId, Body)
		SELECT Id, SenderUserId, ReceiverUserId, Body FROM dbo.tblMessage WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_tblMessage OFF
GO
DROP TABLE dbo.tblMessage
GO
EXECUTE sp_rename N'dbo.Tmp_tblMessage', N'tblMessage', 'OBJECT' 
GO
ALTER TABLE dbo.tblMessage ADD CONSTRAINT
	PK_tblMessage PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblMessage WITH NOCHECK ADD CONSTRAINT
	FK_tblMessage_AspNetUsers_SenderUserId FOREIGN KEY
	(
	SenderUserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblMessage
	NOCHECK CONSTRAINT FK_tblMessage_AspNetUsers_SenderUserId
GO
ALTER TABLE dbo.tblMessage WITH NOCHECK ADD CONSTRAINT
	FK_tblMessage_AspNetUsers_ReceiverUserId FOREIGN KEY
	(
	ReceiverUserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblMessage
	NOCHECK CONSTRAINT FK_tblMessage_AspNetUsers_ReceiverUserId
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblMessage', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblMessage', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblMessage', 'Object', 'CONTROL') as Contr_Per 