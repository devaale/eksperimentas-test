/*
   2022 m. spalio 17 d., pirmadienis19:30:27
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
CREATE TABLE dbo.tblPostImage
	(
	Id int NOT NULL IDENTITY (1, 1),
	PostId int NOT NULL,
	ImageUrl dbo.en_url NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tblPostImage ADD CONSTRAINT
	PK_tblPostImage PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblPostImage SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblPostImage', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblPostImage', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblPostImage', 'Object', 'CONTROL') as Contr_Per 