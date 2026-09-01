/*
   2024 m. liepos 10 d., trečiadienis22:08:44
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
	DROP CONSTRAINT FK_tblDatapoint_tblDatapointFormula
GO
ALTER TABLE dbo.tblDatapointFormula SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDatapointFormula', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDatapointFormula', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDatapointFormula', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint_Order
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
	DROP CONSTRAINT DF_tblDatapoint_IntervalDatepart
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint_LastFormulaCalcTime
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint_AggregationDatepart
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
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint_Instance
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint_BACnetObjectType
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint_BACnetPropertyId
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint_BACnetFunctionCode
GO
ALTER TABLE dbo.tblDatapoint
	DROP CONSTRAINT DF_tblDatapoint_BACnetDataType
GO
CREATE TABLE dbo.Tmp_tblDatapoint
	(
	Id int NOT NULL IDENTITY (1, 1),
	DeviceId int NOT NULL,
	[Order] int NOT NULL,
	Name dbo.en_name NOT NULL,
	Description dbo.en_desc NULL,
	MeasureUnit dbo.en_name NULL,
	DatapointType int NOT NULL,
	RegisterAddress int NOT NULL,
	RegisterType int NOT NULL,
	FunctionCode int NOT NULL,
	Alias dbo.en_name NULL,
	Multiplier decimal(18, 4) NOT NULL,
	Offset decimal(18, 4) NOT NULL,
	ReadWrite int NOT NULL,
	DatapointFormulaId int NULL,
	IntervalDatepart tinyint NOT NULL,
	LastFormulaCalcTime datetime NULL,
	AggregationDatepart tinyint NOT NULL,
	DeviceProtocol  AS ([dbo].[fncDeviceProtocol]([DeviceId])),
	_active bit NOT NULL,
	_deleted bit NOT NULL,
	_cdate datetime NOT NULL,
	_cuserId int NULL,
	_mdate datetime NULL,
	_muserId int NULL,
	_ddate datetime NULL,
	_duserId int NULL,
	_madate datetime NULL,
	_mauserId int NULL,
	Topic nvarchar(64) NULL,
	Path nvarchar(128) NULL,
	Theme varchar(128) NULL,
	ResourceUri varchar(128) NULL,
	Payload varchar(128) NULL,
	Instance int NOT NULL,
	BACnetObjectType int NOT NULL,
	BACnetPropertyId int NOT NULL,
	BACnetFunctionCode int NOT NULL,
	BACnetDataType int NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tblDatapoint SET (LOCK_ESCALATION = TABLE)
GO
DECLARE @v sql_variant 
SET @v = N'Virtual datapoint tblDatapointFormula FK'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_tblDatapoint', N'COLUMN', N'DatapointFormulaId'
GO
DECLARE @v sql_variant 
SET @v = N'Virtual datapoint calculation interval datepart numeric representation'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_tblDatapoint', N'COLUMN', N'IntervalDatepart'
GO
DECLARE @v sql_variant 
SET @v = N'Agreggation datepart numeric representation'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_tblDatapoint', N'COLUMN', N'AggregationDatepart'
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint_Order DEFAULT ((0)) FOR [Order]
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint__datapointTypeId DEFAULT ((1)) FOR DatapointType
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
	DF_tblDatapoint_IntervalDatepart DEFAULT ((0)) FOR IntervalDatepart
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint_LastFormulaCalcTime DEFAULT (getdate()) FOR LastFormulaCalcTime
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint_AggregationDatepart DEFAULT ((0)) FOR AggregationDatepart
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
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint_Instance DEFAULT ((1)) FOR Instance
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint_BACnetObjectType DEFAULT ((1)) FOR BACnetObjectType
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint_BACnetPropertyId DEFAULT ((1)) FOR BACnetPropertyId
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint_BACnetFunctionCode DEFAULT ((1)) FOR BACnetFunctionCode
GO
ALTER TABLE dbo.Tmp_tblDatapoint ADD CONSTRAINT
	DF_tblDatapoint_BACnetDataType DEFAULT ((1)) FOR BACnetDataType
GO
SET IDENTITY_INSERT dbo.Tmp_tblDatapoint ON
GO
IF EXISTS(SELECT * FROM dbo.tblDatapoint)
	 EXEC('INSERT INTO dbo.Tmp_tblDatapoint (Id, DeviceId, [Order], Name, Description, MeasureUnit, DatapointType, RegisterAddress, RegisterType, FunctionCode, Alias, Multiplier, Offset, ReadWrite, DatapointFormulaId, IntervalDatepart, LastFormulaCalcTime, AggregationDatepart, _active, _deleted, _cdate, _cuserId, _mdate, _muserId, _ddate, _duserId, _madate, _mauserId, Topic, Theme, ResourceUri, Payload, Instance, BACnetObjectType, BACnetPropertyId, BACnetFunctionCode, BACnetDataType)
		SELECT Id, DeviceId, [Order], Name, Description, MeasureUnit, DatapointType, RegisterAddress, RegisterType, FunctionCode, Alias, Multiplier, Offset, ReadWrite, DatapointFormulaId, IntervalDatepart, LastFormulaCalcTime, AggregationDatepart, _active, _deleted, _cdate, _cuserId, _mdate, _muserId, _ddate, _duserId, _madate, _mauserId, CONVERT(nvarchar(64), Topic), Theme, ResourceUri, Payload, Instance, BACnetObjectType, BACnetPropertyId, BACnetFunctionCode, BACnetDataType FROM dbo.tblDatapoint WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_tblDatapoint OFF
GO
ALTER TABLE dbo.tblDatapointFormulaChain
	DROP CONSTRAINT FK_tblDatapointFormulaChain_tblDatapoint_DatapointId
GO
ALTER TABLE dbo.tblDatapointFormulaChain
	DROP CONSTRAINT FK_tblDatapointFormulaChain_tblDatapoint_RelatedDatapointId
GO
ALTER TABLE dbo.tblGroupDatapoint
	DROP CONSTRAINT FK_tblGroupDatapoint_DatapointId
GO
ALTER TABLE dbo.tblDatapointValue
	DROP CONSTRAINT FK_tblDatapointValue_tblDatapoint
GO
ALTER TABLE dbo.tblDashboardDatapoint
	DROP CONSTRAINT FK_tblDashboardDatapoint_tblDatapoint
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
	FK_tblDatapoint_tblDatapointFormula FOREIGN KEY
	(
	DatapointFormulaId
	) REFERENCES dbo.tblDatapointFormula
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblDatapoint
	NOCHECK CONSTRAINT FK_tblDatapoint_tblDatapointFormula
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
ALTER TABLE dbo.tblDashboardDatapoint SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDashboardDatapoint', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDashboardDatapoint', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDashboardDatapoint', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
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
ALTER TABLE dbo.tblDatapointFormulaChain WITH NOCHECK ADD CONSTRAINT
	FK_tblDatapointFormulaChain_tblDatapoint_DatapointId FOREIGN KEY
	(
	DatapointId
	) REFERENCES dbo.tblDatapoint
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblDatapointFormulaChain
	NOCHECK CONSTRAINT FK_tblDatapointFormulaChain_tblDatapoint_DatapointId
GO
ALTER TABLE dbo.tblDatapointFormulaChain WITH NOCHECK ADD CONSTRAINT
	FK_tblDatapointFormulaChain_tblDatapoint_RelatedDatapointId FOREIGN KEY
	(
	RelatedDatapointId
	) REFERENCES dbo.tblDatapoint
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblDatapointFormulaChain
	NOCHECK CONSTRAINT FK_tblDatapointFormulaChain_tblDatapoint_RelatedDatapointId
GO
ALTER TABLE dbo.tblDatapointFormulaChain SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDatapointFormulaChain', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDatapointFormulaChain', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDatapointFormulaChain', 'Object', 'CONTROL') as Contr_Per 