/*
   2022 m. gruodžio 1 d., ketvirtadienis19:51:46
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
ALTER TABLE dbo.tblBlocked ADD CONSTRAINT
	FK_tblBlocked_AspNetUsers_UserId FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblBlocked
	NOCHECK CONSTRAINT FK_tblBlocked_AspNetUsers_UserId
GO
ALTER TABLE dbo.tblBlocked ADD CONSTRAINT
	FK_tblBlocked_AspNetUsers_BlockedUserId FOREIGN KEY
	(
	BlockedUserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblBlocked
	NOCHECK CONSTRAINT FK_tblBlocked_AspNetUsers_BlockedUserId
GO
ALTER TABLE dbo.tblBlocked SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblBlocked', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblBlocked', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblBlocked', 'Object', 'CONTROL') as Contr_Per 