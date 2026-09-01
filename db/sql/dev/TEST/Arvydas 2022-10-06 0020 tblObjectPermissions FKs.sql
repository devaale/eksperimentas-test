/*
   2022 m. spalio 6 d., ketvirtadienis18:39:29
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
ALTER TABLE dbo.tblObject SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblObject', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblObject', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblObject', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblObjectPermission ADD CONSTRAINT
	FK_tblObjectPermission_ObjectId FOREIGN KEY
	(
	ObjectId
	) REFERENCES dbo.tblObject
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblObjectPermission
	NOCHECK CONSTRAINT FK_tblObjectPermission_ObjectId
GO
ALTER TABLE dbo.tblObjectPermission ADD CONSTRAINT
	FK_tblObjectPermission_UserId FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblObjectPermission
	NOCHECK CONSTRAINT FK_tblObjectPermission_UserId
GO
ALTER TABLE dbo.tblObjectPermission ADD CONSTRAINT
	FK_tblObjectPermission_FriendUserId FOREIGN KEY
	(
	FriendUserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblObjectPermission
	NOCHECK CONSTRAINT FK_tblObjectPermission_FriendUserId
GO
ALTER TABLE dbo.tblObjectPermission SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblObjectPermission', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblObjectPermission', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblObjectPermission', 'Object', 'CONTROL') as Contr_Per 