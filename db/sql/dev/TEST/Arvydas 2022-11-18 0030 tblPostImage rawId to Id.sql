/*
   2022 m. lapkričio 18 d., penktadienis21:38:27
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
EXECUTE sp_rename N'dbo.tblPostImage.rawId', N'Tmp_Id_2', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.tblPostImage.Tmp_Id_2', N'Id', 'COLUMN' 
GO
ALTER TABLE dbo.tblPostImage SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblPostImage', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblPostImage', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblPostImage', 'Object', 'CONTROL') as Contr_Per 