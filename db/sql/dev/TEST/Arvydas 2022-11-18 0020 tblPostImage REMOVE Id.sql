/*
   2022 m. lapkričio 18 d., penktadienis21:37:13
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
ALTER TABLE dbo.tblPostImage
	DROP CONSTRAINT PK_tblPostImage
GO
ALTER TABLE dbo.tblPostImage ADD CONSTRAINT
	PK_tblPostImage_1 PRIMARY KEY CLUSTERED 
	(
	rawId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblPostImage
	DROP COLUMN Id
GO
ALTER TABLE dbo.tblPostImage SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblPostImage', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblPostImage', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblPostImage', 'Object', 'CONTROL') as Contr_Per 