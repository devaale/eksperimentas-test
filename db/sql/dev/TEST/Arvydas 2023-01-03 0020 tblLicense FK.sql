/*
   2023 m. sausio 3 d., antradienis20:50:59
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
ALTER TABLE dbo.tblLicense ADD CONSTRAINT
	FK_tblLicense_AspNetUsers FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblLicense
	NOCHECK CONSTRAINT FK_tblLicense_AspNetUsers
GO
ALTER TABLE dbo.tblLicense SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblLicense', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblLicense', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblLicense', 'Object', 'CONTROL') as Contr_Per 