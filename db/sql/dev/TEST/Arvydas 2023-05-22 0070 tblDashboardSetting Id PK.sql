/*
   2023 m. gegužės 22 d., pirmadienis18:21:23
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
ALTER TABLE dbo.tblDashboardSetting
	DROP CONSTRAINT FK_tblDashboardSetting_AspNetUsers
GO
ALTER TABLE dbo.AspNetUsers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblDashboardSetting
	DROP CONSTRAINT DF_tblDashboardSetting_IntervalDatepart
GO
CREATE TABLE dbo.Tmp_tblDashboardSetting
	(
	Id int NOT NULL IDENTITY (1, 1),
	UserId nvarchar(128) NOT NULL,
	IntervalDatepart int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tblDashboardSetting SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_tblDashboardSetting ADD CONSTRAINT
	DF_tblDashboardSetting_IntervalDatepart DEFAULT ((4)) FOR IntervalDatepart
GO
SET IDENTITY_INSERT dbo.Tmp_tblDashboardSetting OFF
GO
IF EXISTS(SELECT * FROM dbo.tblDashboardSetting)
	 EXEC('INSERT INTO dbo.Tmp_tblDashboardSetting (UserId, IntervalDatepart)
		SELECT UserId, IntervalDatepart FROM dbo.tblDashboardSetting WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.tblDashboardSetting
GO
EXECUTE sp_rename N'dbo.Tmp_tblDashboardSetting', N'tblDashboardSetting', 'OBJECT' 
GO
ALTER TABLE dbo.tblDashboardSetting ADD CONSTRAINT
	PK_tblDashboardSetting PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblDashboardSetting WITH NOCHECK ADD CONSTRAINT
	FK_tblDashboardSetting_AspNetUsers FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblDashboardSetting
	NOCHECK CONSTRAINT FK_tblDashboardSetting_AspNetUsers
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDashboardSetting', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDashboardSetting', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDashboardSetting', 'Object', 'CONTROL') as Contr_Per 