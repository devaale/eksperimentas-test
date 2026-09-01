/*
   2022 m. gruodžio 19 d., pirmadienis15:24:22
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
EXECUTE sp_rename N'dbo.tblBlocked.BlockedUserId', N'Tmp_RelatedUserId', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.tblBlocked.Tmp_RelatedUserId', N'RelatedUserId', 'COLUMN' 
GO
ALTER TABLE dbo.tblBlocked ADD
	Name  AS ([dbo].[fncUsernameById]([RelatedUserId]))
GO
ALTER TABLE dbo.tblBlocked SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblBlocked', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblBlocked', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblBlocked', 'Object', 'CONTROL') as Contr_Per 