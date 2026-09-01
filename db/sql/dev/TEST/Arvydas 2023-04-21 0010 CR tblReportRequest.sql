/*
   2023 m. balandžio 21 d., penktadienis16:09:35
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
CREATE TABLE dbo.tblReportRequest
	(
	Id uniqueidentifier NOT NULL,
	UserId nvarchar(128) NULL,
	Tyoe int NOT NULL,
	DateFrom datetime NULL,
	DateTo datetime NULL,
	Ids varchar(MAX) NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.tblReportRequest ADD CONSTRAINT
	DF_tblReportRequest_Id DEFAULT newid() FOR Id
GO
ALTER TABLE dbo.tblReportRequest ADD CONSTRAINT
	DF_tblReportRequest_Tyoe DEFAULT 0 FOR Tyoe
GO
ALTER TABLE dbo.tblReportRequest ADD CONSTRAINT
	PK_tblReportRequest PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblReportRequest SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblReportRequest', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblReportRequest', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblReportRequest', 'Object', 'CONTROL') as Contr_Per 