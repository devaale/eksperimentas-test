/*
   2024 m. liepos 9 d., antradienis17:08:36
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
CREATE TABLE dbo.tblMqttMessage
	(
	Id int NOT NULL,
	DeviceId int NOT NULL,
	DeviceTopicId int NOT NULL,
	Topic nvarchar(64) COLLATE Lithuanian_100_CS_AS NOT NULL,
	Payload nvarchar(MAX) COLLATE Lithuanian_100_CS_AS NULL,
	CreationDate datetime NOT NULL,
	FinishDate datetime NULL,
	State int NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
DECLARE @v sql_variant 
SET @v = N'Record''s creation date (auto filled).'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'tblMqttMessage', N'COLUMN', N'CreationDate'
GO
ALTER TABLE dbo.tblMqttMessage ADD CONSTRAINT
	DF_Table_1__cdate DEFAULT (getdate()) FOR CreationDate
GO
ALTER TABLE dbo.tblMqttMessage ADD CONSTRAINT
	DF_Table_1__state DEFAULT ((0)) FOR State
GO
ALTER TABLE dbo.tblMqttMessage SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblMqttMessage', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblMqttMessage', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblMqttMessage', 'Object', 'CONTROL') as Contr_Per 