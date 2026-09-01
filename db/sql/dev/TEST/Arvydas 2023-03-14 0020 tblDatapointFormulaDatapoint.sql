/*
   2023 m. kovo 14 d., antradienis14:19:28
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
CREATE TABLE dbo.tblDatapointFormulaDatapoint
	(
	Id int NOT NULL IDENTITY (1, 1),
	DatapointId int NOT NULL,
	RelatedDatapointId int NOT NULL,
	[Order] int NOT NULL,
	Name dbo.en_name NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tblDatapointFormulaDatapoint ADD CONSTRAINT
	DF_tblDatapointFormulaDatapoint_Order DEFAULT 1000 FOR [Order]
GO
ALTER TABLE dbo.tblDatapointFormulaDatapoint ADD CONSTRAINT
	PK_tblDatapointFormulaDatapoint PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblDatapointFormulaDatapoint SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDatapointFormulaDatapoint', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDatapointFormulaDatapoint', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDatapointFormulaDatapoint', 'Object', 'CONTROL') as Contr_Per 