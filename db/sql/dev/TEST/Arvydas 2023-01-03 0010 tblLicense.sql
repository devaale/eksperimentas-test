/*
   2023 m. sausio 3 d., antradienis20:49:11
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
CREATE TABLE dbo.tblLicense
	(
	Id int NOT NULL IDENTITY (1, 1),
	UserId nvarchar(128) NOT NULL,
	Type int NOT NULL,
	ValidFrom datetime NOT NULL,
	ValidUntil datetime NOT NULL,
	Active bit NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tblLicense ADD CONSTRAINT
	DF_tblLicense_Type DEFAULT 0 FOR Type
GO
ALTER TABLE dbo.tblLicense ADD CONSTRAINT
	DF_tblLicense_Active DEFAULT 0 FOR Active
GO
ALTER TABLE dbo.tblLicense ADD CONSTRAINT
	PK_tblLicense PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblLicense SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblLicense', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblLicense', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblLicense', 'Object', 'CONTROL') as Contr_Per 