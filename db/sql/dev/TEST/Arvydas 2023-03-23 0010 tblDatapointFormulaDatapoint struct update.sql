/*
   2023 m. kovo 23 d., ketvirtadienis16:25:10
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
ALTER TABLE dbo.tblDatapointFormulaDatapoint
	DROP CONSTRAINT FK_tblDatapointFormulaDatapoint_tblDatapoint_DatapointId
GO
ALTER TABLE dbo.tblDatapointFormulaDatapoint
	DROP CONSTRAINT FK_tblDatapointFormulaDatapoint_tblDatapoint_RelatedDatapointId
GO
ALTER TABLE dbo.tblDatapoint SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDatapoint', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDatapoint', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDatapoint', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblDatapointFormulaDatapoint
	DROP CONSTRAINT DF_tblDatapointFormulaDatapoint_Order
GO
CREATE TABLE dbo.Tmp_tblDatapointFormulaDatapoint
	(
	Id int NOT NULL IDENTITY (1, 1),
	DatapointId int NOT NULL,
	[Order] int NOT NULL,
	Name dbo.en_name NULL,
	RelatedDatapointId int NULL,
	Value decimal(18, 4) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tblDatapointFormulaDatapoint SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_tblDatapointFormulaDatapoint ADD CONSTRAINT
	DF_tblDatapointFormulaDatapoint_Order DEFAULT ((1000)) FOR [Order]
GO
SET IDENTITY_INSERT dbo.Tmp_tblDatapointFormulaDatapoint ON
GO
IF EXISTS(SELECT * FROM dbo.tblDatapointFormulaDatapoint)
	 EXEC('INSERT INTO dbo.Tmp_tblDatapointFormulaDatapoint (Id, DatapointId, [Order], Name, RelatedDatapointId)
		SELECT Id, DatapointId, [Order], Name, RelatedDatapointId FROM dbo.tblDatapointFormulaDatapoint WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_tblDatapointFormulaDatapoint OFF
GO
DROP TABLE dbo.tblDatapointFormulaDatapoint
GO
EXECUTE sp_rename N'dbo.Tmp_tblDatapointFormulaDatapoint', N'tblDatapointFormulaDatapoint', 'OBJECT' 
GO
ALTER TABLE dbo.tblDatapointFormulaDatapoint ADD CONSTRAINT
	PK_tblDatapointFormulaDatapoint PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

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
COMMIT
select Has_Perms_By_Name(N'dbo.tblDatapointFormulaDatapoint', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDatapointFormulaDatapoint', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDatapointFormulaDatapoint', 'Object', 'CONTROL') as Contr_Per 