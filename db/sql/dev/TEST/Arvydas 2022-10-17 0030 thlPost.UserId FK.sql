/*
   2022 m. spalio 17 d., pirmadienis19:29:04
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
ALTER TABLE dbo.tblPost ADD CONSTRAINT
	FK_tblPost_AspNetUsers FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblPost
	NOCHECK CONSTRAINT FK_tblPost_AspNetUsers
GO
ALTER TABLE dbo.tblPost SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblPost', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblPost', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblPost', 'Object', 'CONTROL') as Contr_Per 