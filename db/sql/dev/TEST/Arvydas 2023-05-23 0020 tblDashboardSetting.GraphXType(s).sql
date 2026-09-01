/*
   2023 m. gegužės 31 d., trečiadienis15:16:04
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
ALTER TABLE dbo.tblDashboardSetting ADD
	Graph1Type tinyint NOT NULL CONSTRAINT DF_tblDashboardSetting_Graph1Type DEFAULT 2,
	Graph2Type tinyint NOT NULL CONSTRAINT DF_tblDashboardSetting_Graph2Type DEFAULT 2,
	Graph3Type tinyint NOT NULL CONSTRAINT DF_tblDashboardSetting_Graph3Type DEFAULT 2,
	Graph4Type tinyint NOT NULL CONSTRAINT DF_tblDashboardSetting_Graph4Type DEFAULT 2
GO
ALTER TABLE dbo.tblDashboardSetting SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDashboardSetting', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDashboardSetting', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDashboardSetting', 'Object', 'CONTROL') as Contr_Per 