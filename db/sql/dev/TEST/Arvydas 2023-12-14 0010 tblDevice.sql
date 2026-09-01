/*
   2023 m. gruodžio 14 d., ketvirtadienis15:21:44
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
ALTER TABLE dbo.tblDevice ADD
	ClientUsername varchar(128) NULL,
	ClientPassword varchar(128) NULL
GO
ALTER TABLE dbo.tblDevice SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDevice', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDevice', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDevice', 'Object', 'CONTROL') as Contr_Per 