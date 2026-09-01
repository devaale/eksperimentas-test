/*
   2023 m. gruodžio 14 d., ketvirtadienis15:32:12
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
ALTER TABLE dbo.tblDatapoint ADD
	Topic varchar(128) NULL,
	Theme varchar(128) NULL,
	ResourceUri varchar(128) NULL,
	Payload varchar(128) NULL,
	Instance int NOT NULL CONSTRAINT DF_tblDatapoint_Instance DEFAULT 1,
	BACnetObjectType int NOT NULL CONSTRAINT DF_tblDatapoint_BACnetObjectType DEFAULT 1,
	BACnetPropertyId int NOT NULL CONSTRAINT DF_tblDatapoint_BACnetPropertyId DEFAULT 1,
	BACnetFunctionCode int NOT NULL CONSTRAINT DF_tblDatapoint_BACnetFunctionCode DEFAULT 1,
	BACnetDataType int NOT NULL CONSTRAINT DF_tblDatapoint_BACnetDataType DEFAULT 1
GO
ALTER TABLE dbo.tblDatapoint SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDatapoint', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDatapoint', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDatapoint', 'Object', 'CONTROL') as Contr_Per 