/*
   2023 m. gegužės 22 d., pirmadienis18:22:42
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
ALTER TABLE dbo.tblDashboardDatapoint
	DROP CONSTRAINT FK_tblDashboardDatapoint_tblDatapoint
GO
ALTER TABLE dbo.tblDatapoint SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDatapoint', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDatapoint', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDatapoint', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblDashboardDatapoint
	DROP CONSTRAINT FK_tblDashboardDatapoint_AspNetUsers
GO
ALTER TABLE dbo.AspNetUsers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblDashboardDatapoint
	DROP CONSTRAINT DF_tblDashboardDatapoint_GraphId
GO
CREATE TABLE dbo.Tmp_tblDashboardDatapoint
	(
	Id int NOT NULL IDENTITY (1, 1),
	UserId nvarchar(128) NOT NULL,
	GraphId tinyint NOT NULL,
	DatapointId int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tblDashboardDatapoint SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_tblDashboardDatapoint ADD CONSTRAINT
	DF_tblDashboardDatapoint_GraphId DEFAULT ((0)) FOR GraphId
GO
SET IDENTITY_INSERT dbo.Tmp_tblDashboardDatapoint OFF
GO
IF EXISTS(SELECT * FROM dbo.tblDashboardDatapoint)
	 EXEC('INSERT INTO dbo.Tmp_tblDashboardDatapoint (UserId, GraphId, DatapointId)
		SELECT UserId, GraphId, DatapointId FROM dbo.tblDashboardDatapoint WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.tblDashboardDatapoint
GO
EXECUTE sp_rename N'dbo.Tmp_tblDashboardDatapoint', N'tblDashboardDatapoint', 'OBJECT' 
GO
ALTER TABLE dbo.tblDashboardDatapoint ADD CONSTRAINT
	PK_tblDashboardDatapoint PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblDashboardDatapoint WITH NOCHECK ADD CONSTRAINT
	FK_tblDashboardDatapoint_AspNetUsers FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblDashboardDatapoint
	NOCHECK CONSTRAINT FK_tblDashboardDatapoint_AspNetUsers
GO
ALTER TABLE dbo.tblDashboardDatapoint WITH NOCHECK ADD CONSTRAINT
	FK_tblDashboardDatapoint_tblDatapoint FOREIGN KEY
	(
	DatapointId
	) REFERENCES dbo.tblDatapoint
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblDashboardDatapoint
	NOCHECK CONSTRAINT FK_tblDashboardDatapoint_tblDatapoint
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDashboardDatapoint', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDashboardDatapoint', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDashboardDatapoint', 'Object', 'CONTROL') as Contr_Per 