/*
   2023 m. kovo 14 d., antradienis14:21:45
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
ALTER TABLE dbo.tblDatapoint SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDatapoint', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDatapoint', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDatapoint', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblDatapointFormulaDatapoint ADD CONSTRAINT
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
ALTER TABLE dbo.tblDatapointFormulaDatapoint ADD CONSTRAINT
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