/*
   2023 m. balandžio 21 d., penktadienis17:34:37
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
ALTER TABLE dbo.tblReportRequest
	DROP CONSTRAINT DF_tblReportRequest_Id
GO
ALTER TABLE dbo.tblReportRequest
	DROP CONSTRAINT DF_tblReportRequest_Tyoe
GO
CREATE TABLE dbo.Tmp_tblReportRequest
	(
	Id uniqueidentifier NOT NULL,
	UserId nvarchar(128) NOT NULL,
	Params nvarchar(MAX) NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tblReportRequest SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_tblReportRequest ADD CONSTRAINT
	DF_tblReportRequest_Id DEFAULT (newid()) FOR Id
GO
IF EXISTS(SELECT * FROM dbo.tblReportRequest)
	 EXEC('INSERT INTO dbo.Tmp_tblReportRequest (Id, UserId)
		SELECT Id, UserId FROM dbo.tblReportRequest WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.tblReportRequest
GO
EXECUTE sp_rename N'dbo.Tmp_tblReportRequest', N'tblReportRequest', 'OBJECT' 
GO
ALTER TABLE dbo.tblReportRequest ADD CONSTRAINT
	PK_tblReportRequest PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblReportRequest', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblReportRequest', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblReportRequest', 'Object', 'CONTROL') as Contr_Per 