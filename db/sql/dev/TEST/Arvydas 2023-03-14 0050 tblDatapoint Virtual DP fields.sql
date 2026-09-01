/*
   2023 m. kovo 14 d., antradienis14:25:42
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
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT FK_tblDatapoint_tblDevice
GO
ALTER TABLE dbo.tblDevice SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDevice', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDevice', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDevice', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint__datapointTypeId
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint_registerAddress
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint_registerType
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint_functionCode
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint_multiplier
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint_offset
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint_Function
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint__active
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint__deleted
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint__cdate
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint__mdate
GO
CREATE TABLE dbo.Tmp_tblDatapoint
	(
	Id int NOT NULL IDENTITY (1, 1),
	DeviceId int NOT NULL,
	Name dbo.en_name NOT NULL,
	Description dbo.en_desc NULL,
	MeasureUnit dbo.en_name NULL,
	DatapointTypeId int NOT NULL,
	RegisterAddress int NOT NULL,
	RegisterType int NOT NULL,
	FunctionCode int NOT NULL,
	Alias dbo.en_name NULL,
	Multiplier decimal(18, 4) NOT NULL,
	Offset decimal(18, 4) NOT NULL,
	ReadWrite int NOT NULL,
	DatapointFormulaId int NULL,
	Interval int NOT NULL,
	IntervalDatepart nvarchar(32) NOT NULL,
	_active bit NOT NULL,
	_deleted bit NOT NULL,
	_cdate datetime NOT NULL,
	_cuserId int NULL,
	_mdate datetime NULL,
	_muserId int NULL,
	_ddate datetime NULL,
	_duserId int NULL,
	_madate datetime NULL,
	_mauserId int NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tblDatapoint SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint__datapointTypeId DEFAULT ((1)) FOR DatapointTypeId
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint_registerAddress DEFAULT ((0)) FOR RegisterAddress
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint_registerType DEFAULT ((16)) FOR RegisterType
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint_functionCode DEFAULT ((3)) FOR FunctionCode
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint_multiplier DEFAULT ((1)) FOR Multiplier
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint_offset DEFAULT ((0)) FOR Offset
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint_Function DEFAULT ((0)) FOR ReadWrite
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint_Interval DEFAULT 1 FOR Interval
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint_IntervalDatepart DEFAULT ('day') FOR IntervalDatepart
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint__active DEFAULT ((1)) FOR _active
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint__deleted DEFAULT ((0)) FOR _deleted
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint__cdate DEFAULT (getdate()) FOR _cdate
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint__mdate DEFAULT (getdate()) FOR _mdate
GO
SET IDENTITY_INSERT dbo.Tmp_tblDatapoint ON
GO
IF EXISTS(SELECT * FROM dbo.tblDatapoint)
	 EXEC('INSERT INTO dbo.Tmp_tblDatapoint (Id, DeviceId, Name, Description, MeasureUnit, DatapointTypeId, RegisterAddress, RegisterType, FunctionCode, Alias, Multiplier, Offset, ReadWrite, _active, _deleted, _cdate, _cuserId, _mdate, _muserId, _ddate, _duserId, _madate, _mauserId)
		SELECT Id, DeviceId, Name, Description, MeasureUnit, DatapointTypeId, RegisterAddress, RegisterType, FunctionCode, Alias, Multiplier, Offset, ReadWrite, _active, _deleted, _cdate, _cuserId, _mdate, _muserId, _ddate, _duserId, _madate, _mauserId FROM dbo.tblDatapoint WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_tblDatapoint OFF
GO
ALTER TABLE dbo.tblDatapointFormulaDatapoint
	DROP CONSTRAINT FK_tblDatapointFormulaDatapoint_tblDatapoint_DatapointId
GO
ALTER TABLE dbo.tblDatapointFormulaDatapoint
	DROP CONSTRAINT FK_tblDatapointFormulaDatapoint_tblDatapoint_RelatedDatapointId
GO
ALTER TABLE dbo.tblDatapointValue
	DROP CONSTRAINT FK_tblDatapointValue_tblDatapoint
GO
ALTER TABLE dbo.tblGroupDatapoint
	DROP CONSTRAINT FK_tblGroupDatapoint_DatapointId
GO
DROP TABLE dbo.tblDatapoint
GO
EXECUTE sp_rename N'dbo.Tmp_tblDatapoint', N'tblDatapoint', 'OBJECT' 
GO
ALTER TABLE dbo.tblDatapoint ADD CONSTRAINT
	PK_tblDatapoint PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblDatapoint WITH NOCHECK ADD CONSTRAINT
	FK_tblDatapoint_tblDevice FOREIGN KEY
	(
	DeviceId
	) REFERENCES dbo.tblDevice
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblDatapoint
	NOCHECK CONSTRAINT FK_tblDatapoint_tblDevice
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDatapoint', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDatapoint', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDatapoint', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblGroupDatapoint ADD CONSTRAINT
	FK_tblGroupDatapoint_DatapointId FOREIGN KEY
	(
	DatapointId
	) REFERENCES dbo.tblDatapoint
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tblGroupDatapoint SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblGroupDatapoint', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblGroupDatapoint', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblGroupDatapoint', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblDatapointValue WITH NOCHECK ADD CONSTRAINT
	FK_tblDatapointValue_tblDatapoint FOREIGN KEY
	(
	DatapointId
	) REFERENCES dbo.tblDatapoint
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.tblDatapointValue
	NOCHECK CONSTRAINT FK_tblDatapointValue_tblDatapoint
GO
ALTER TABLE dbo.tblDatapointValue SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDatapointValue', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDatapointValue', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDatapointValue', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblDatapointFormulaDatapoint WITH NOCHECK ADD CONSTRAINT
	FK_tblDatapointFormulaDatapoint_tblDatapoint_DatapointId FOREIGN KEY
	(
	DatapointId
	) REFERENCES dbo.tblDatapoint
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblDatapointFormulaDatapoint
	NOCHECK CONSTRAINT FK_tblDatapointFormulaDatapoint_tblDatapoint_DatapointId
GO
ALTER TABLE dbo.tblDatapointFormulaDatapoint WITH NOCHECK ADD CONSTRAINT
	FK_tblDatapointFormulaDatapoint_tblDatapoint_RelatedDatapointId FOREIGN KEY
	(
	RelatedDatapointId
	) REFERENCES dbo.tblDatapoint
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblDatapointFormulaDatapoint
	NOCHECK CONSTRAINT FK_tblDatapointFormulaDatapoint_tblDatapoint_RelatedDatapointId
GO
ALTER TABLE dbo.tblDatapointFormulaDatapoint SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDatapointFormulaDatapoint', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDatapointFormulaDatapoint', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDatapointFormulaDatapoint', 'Object', 'CONTROL') as Contr_Per 