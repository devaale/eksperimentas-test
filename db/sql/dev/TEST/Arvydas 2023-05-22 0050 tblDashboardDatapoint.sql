/*
   2023 m. gegužės 22 d., pirmadienis18:05:51
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
CREATE TABLE dbo.tblDashboardDatapoint
	(
	UserId nvarchar(128) NOT NULL,
	GraphId tinyint NOT NULL,
	DatapointId int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tblDashboardDatapoint ADD CONSTRAINT
	DF_tblDashboardDatapoint_GraphId DEFAULT 0 FOR GraphId
GO
ALTER TABLE dbo.tblDashboardDatapoint ADD CONSTRAINT
	PK_tblDashboardDatapoint PRIMARY KEY CLUSTERED 
	(
	UserId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblDashboardDatapoint SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDashboardDatapoint', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDashboardDatapoint', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDashboardDatapoint', 'Object', 'CONTROL') as Contr_Per 