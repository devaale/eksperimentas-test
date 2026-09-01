/*
   2022 m. spalio 17 d., pirmadienis19:28:18
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
CREATE TABLE dbo.tblPost
	(
	Id int NOT NULL IDENTITY (1, 1),
	UserId nvarchar(128) NOT NULL,
	Date datetime NOT NULL,
	Body nvarchar(MAX) NOT NULL,
	Audience tinyint NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.tblPost ADD CONSTRAINT
	DF_tblPost_Date DEFAULT GETDATE() FOR Date
GO
ALTER TABLE dbo.tblPost ADD CONSTRAINT
	DF_tblPost_Audience DEFAULT 0 FOR Audience
GO
ALTER TABLE dbo.tblPost ADD CONSTRAINT
	PK_tblPost PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblPost SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblPost', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblPost', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblPost', 'Object', 'CONTROL') as Contr_Per 