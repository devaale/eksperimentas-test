/*
   2022 m. gruodžio 2 d., penktadienis20:53:07
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
ALTER TABLE dbo.AspNetUsers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblMessage ADD CONSTRAINT
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
ALTER TABLE dbo.tblMessage ADD CONSTRAINT
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
ALTER TABLE dbo.tblMessage SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblMessage', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblMessage', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblMessage', 'Object', 'CONTROL') as Contr_Per 