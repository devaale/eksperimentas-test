/*
   2023 m. birželio 15 d., ketvirtadienis19:07:08
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
EXECUTE sp_rename N'dbo.tblDashboardSetting.Graph1Aggregation', N'Tmp_Graph1Interval', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.tblDashboardSetting.Graph2Aggregation', N'Tmp_Graph2Interval_1', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.tblDashboardSetting.Graph3Aggregation', N'Tmp_Graph3Interval_2', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.tblDashboardSetting.Graph4Aggregation', N'Tmp_Graph4Interval_3', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.tblDashboardSetting.Tmp_Graph1Interval', N'Graph1Interval', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.tblDashboardSetting.Tmp_Graph2Interval_1', N'Graph2Interval', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.tblDashboardSetting.Tmp_Graph3Interval_2', N'Graph3Interval', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.tblDashboardSetting.Tmp_Graph4Interval_3', N'Graph4Interval', 'COLUMN' 
GO
ALTER TABLE dbo.tblDashboardSetting SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDashboardSetting', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDashboardSetting', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDashboardSetting', 'Object', 'CONTROL') as Contr_Per 