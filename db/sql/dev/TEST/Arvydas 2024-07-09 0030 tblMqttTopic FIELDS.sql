/*
   2024 m. liepos 9 d., antradienis17:04:27
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
EXECUTE sp_rename N'dbo.tblMqttTopic._id', N'Tmp_Id', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.tblMqttTopic.url', N'Tmp_Url_1', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.tblMqttTopic.topic', N'Tmp_Topic_2', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.tblMqttTopic.date', N'Tmp_Date_3', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.tblMqttTopic.Tmp_Id', N'Id', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.tblMqttTopic.Tmp_Url_1', N'Url', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.tblMqttTopic.Tmp_Topic_2', N'Topic', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.tblMqttTopic.Tmp_Date_3', N'Date', 'COLUMN' 
GO
ALTER TABLE dbo.tblMqttTopic SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblMqttTopic', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblMqttTopic', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblMqttTopic', 'Object', 'CONTROL') as Contr_Per 