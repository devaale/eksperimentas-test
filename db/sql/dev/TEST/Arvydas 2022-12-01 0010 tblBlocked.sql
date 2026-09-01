/*
   2022 m. gruodžio 1 d., ketvirtadienis19:49:43
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
CREATE TABLE dbo.tblBlocked
	(
	Id int NOT NULL IDENTITY (1, 1),
	UserId nvarchar(128) NOT NULL,
	BlockedUserId nvarchar(128) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tblBlocked ADD CONSTRAINT
	PK_tblBlocked PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblBlocked SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblBlocked', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblBlocked', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblBlocked', 'Object', 'CONTROL') as Contr_Per 