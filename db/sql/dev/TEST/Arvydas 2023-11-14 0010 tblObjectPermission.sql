/*
   2023 m. lapkričio 14 d., antradienis14:30:29
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
ALTER TABLE dbo.tblObjectPermission ADD
	PermWrite bit NOT NULL CONSTRAINT DF_tblObjectPermission_PermWrite DEFAULT 0,
	PermDevice bit NOT NULL CONSTRAINT DF_tblObjectPermission_PermDevice DEFAULT 0,
	PermAlgorithm bit NOT NULL CONSTRAINT DF_tblObjectPermission_PermAlgorithm DEFAULT 0,
	PermGroup bit NOT NULL CONSTRAINT DF_tblObjectPermission_PermGroup DEFAULT 0
GO
ALTER TABLE dbo.tblObjectPermission SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblObjectPermission', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblObjectPermission', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblObjectPermission', 'Object', 'CONTROL') as Contr_Per 