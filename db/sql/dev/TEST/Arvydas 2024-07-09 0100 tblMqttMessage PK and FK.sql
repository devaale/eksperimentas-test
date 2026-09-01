/*
   2024 m. liepos 9 d., antradienis17:10:38
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
ALTER TABLE dbo.tblDeviceTopic SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDeviceTopic', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDeviceTopic', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDeviceTopic', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblDevice SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDevice', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDevice', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDevice', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblMqttMessage
	DROP CONSTRAINT DF_Table_1__cdate
GO
ALTER TABLE dbo.tblMqttMessage
	DROP CONSTRAINT DF_Table_1__state
GO
CREATE TABLE dbo.Tmp_tblMqttMessage
	(
	Id int NOT NULL IDENTITY (1, 1),
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
ALTER TABLE dbo.Tmp_tblMqttMessage SET (LOCK_ESCALATION = TABLE)
GO
DECLARE @v sql_variant 
SET @v = N'Record''s creation date (auto filled).'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_tblMqttMessage', N'COLUMN', N'CreationDate'
GO
ALTER TABLE dbo.Tmp_tblMqttMessage ADD CONSTRAINT
	DF_Table_1__cdate DEFAULT (getdate()) FOR CreationDate
GO
ALTER TABLE dbo.Tmp_tblMqttMessage ADD CONSTRAINT
	DF_Table_1__state DEFAULT ((0)) FOR State
GO
SET IDENTITY_INSERT dbo.Tmp_tblMqttMessage ON
GO
IF EXISTS(SELECT * FROM dbo.tblMqttMessage)
	 EXEC('INSERT INTO dbo.Tmp_tblMqttMessage (Id, DeviceId, DeviceTopicId, Topic, Payload, CreationDate, FinishDate, State)
		SELECT Id, DeviceId, DeviceTopicId, Topic, Payload, CreationDate, FinishDate, State FROM dbo.tblMqttMessage WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_tblMqttMessage OFF
GO
DROP TABLE dbo.tblMqttMessage
GO
EXECUTE sp_rename N'dbo.Tmp_tblMqttMessage', N'tblMqttMessage', 'OBJECT' 
GO
ALTER TABLE dbo.tblMqttMessage ADD CONSTRAINT
	PK_tblMqttMessage PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblMqttMessage ADD CONSTRAINT
	FK_tblMqttMessage_tblDevice FOREIGN KEY
	(
	DeviceId
	) REFERENCES dbo.tblDevice
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblMqttMessage
	NOCHECK CONSTRAINT FK_tblMqttMessage_tblDevice
GO
ALTER TABLE dbo.tblMqttMessage ADD CONSTRAINT
	FK_tblMqttMessage_tblDeviceTopic FOREIGN KEY
	(
	DeviceTopicId
	) REFERENCES dbo.tblDeviceTopic
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblMqttMessage
	NOCHECK CONSTRAINT FK_tblMqttMessage_tblDeviceTopic
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblMqttMessage', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblMqttMessage', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblMqttMessage', 'Object', 'CONTROL') as Contr_Per 