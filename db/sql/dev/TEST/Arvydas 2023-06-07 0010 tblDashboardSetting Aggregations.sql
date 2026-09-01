/*
   2023 m. birželio 7 d., trečiadienis18:35:06
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
ALTER TABLE dbo.tblDashboardSetting
	DROP CONSTRAINT DF_tblDashboardSetting_Graph1Type
GO
ALTER TABLE dbo.tblDashboardSetting
	DROP CONSTRAINT DF_tblDashboardSetting_Graph2Type
GO
ALTER TABLE dbo.tblDashboardSetting
	DROP CONSTRAINT DF_tblDashboardSetting_Graph3Type
GO
ALTER TABLE dbo.tblDashboardSetting
	DROP CONSTRAINT DF_tblDashboardSetting_Graph4Type
GO
CREATE TABLE dbo.Tmp_tblDashboardSetting
	(
	UserId nvarchar(128) NOT NULL,
	IntervalDatepart tinyint NOT NULL,
	Graph1Type tinyint NOT NULL,
	Graph1Aggregation tinyint NOT NULL,
	Graph2Type tinyint NOT NULL,
	Graph2Aggregation tinyint NOT NULL,
	Graph3Type tinyint NOT NULL,
	Graph3Aggregation tinyint NOT NULL,
	Graph4Type tinyint NOT NULL,
	Graph4Aggregation tinyint NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tblDashboardSetting SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_tblDashboardSetting ADD CONSTRAINT
	DF_tblDashboardSetting_IntervalDatepart DEFAULT ((4)) FOR IntervalDatepart
GO
ALTER TABLE dbo.Tmp_tblDashboardSetting ADD CONSTRAINT
	DF_tblDashboardSetting_Graph1Type DEFAULT ((2)) FOR Graph1Type
GO
ALTER TABLE dbo.Tmp_tblDashboardSetting ADD CONSTRAINT
	DF_tblDashboardSetting_Graph1Aggregation DEFAULT 4 FOR Graph1Aggregation
GO
ALTER TABLE dbo.Tmp_tblDashboardSetting ADD CONSTRAINT
	DF_tblDashboardSetting_Graph2Type DEFAULT ((2)) FOR Graph2Type
GO
ALTER TABLE dbo.Tmp_tblDashboardSetting ADD CONSTRAINT
	DF_tblDashboardSetting_Graph2Aggregation DEFAULT 4 FOR Graph2Aggregation
GO
ALTER TABLE dbo.Tmp_tblDashboardSetting ADD CONSTRAINT
	DF_tblDashboardSetting_Graph3Type DEFAULT ((2)) FOR Graph3Type
GO
ALTER TABLE dbo.Tmp_tblDashboardSetting ADD CONSTRAINT
	DF_tblDashboardSetting_Graph3Aggregation DEFAULT 4 FOR Graph3Aggregation
GO
ALTER TABLE dbo.Tmp_tblDashboardSetting ADD CONSTRAINT
	DF_tblDashboardSetting_Graph4Type DEFAULT ((2)) FOR Graph4Type
GO
ALTER TABLE dbo.Tmp_tblDashboardSetting ADD CONSTRAINT
	DF_tblDashboardSetting_Graph4Aggregation DEFAULT 4 FOR Graph4Aggregation
GO
IF EXISTS(SELECT * FROM dbo.tblDashboardSetting)
	 EXEC('INSERT INTO dbo.Tmp_tblDashboardSetting (UserId, IntervalDatepart, Graph1Type, Graph2Type, Graph3Type, Graph4Type)
		SELECT UserId, CONVERT(tinyint, IntervalDatepart), Graph1Type, Graph2Type, Graph3Type, Graph4Type FROM dbo.tblDashboardSetting WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.tblDashboardSetting
GO
EXECUTE sp_rename N'dbo.Tmp_tblDashboardSetting', N'tblDashboardSetting', 'OBJECT' 
GO
ALTER TABLE dbo.tblDashboardSetting ADD CONSTRAINT
	PK_tblDashboardSetting_1 PRIMARY KEY CLUSTERED 
	(
	UserId
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