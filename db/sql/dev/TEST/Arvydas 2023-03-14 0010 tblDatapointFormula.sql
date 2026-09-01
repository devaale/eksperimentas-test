/*
   2023 m. kovo 14 d., antradienis13:51:46
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
CREATE TABLE dbo.tblDatapointFormula
	(
	Id int NOT NULL,
	Alias dbo.en_sys_name NOT NULL,
	NumDatapoints int NOT NULL
	)  ON [PRIMARY]
GO
DECLARE @v sql_variant 
SET @v = N'Developer assigned, non auto-number PK field.'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'tblDatapointFormula', N'COLUMN', N'Id'
GO
DECLARE @v sql_variant 
SET @v = N'Formula'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'tblDatapointFormula', N'COLUMN', N'Alias'
GO
DECLARE @v sql_variant 
SET @v = N'Fixed number of formula datapoints or 0 if amount is not fixed.'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'tblDatapointFormula', N'COLUMN', N'NumDatapoints'
GO
ALTER TABLE dbo.tblDatapointFormula ADD CONSTRAINT
	DF_tblDatapointFormula_NumDatapoints DEFAULT 0 FOR NumDatapoints
GO
ALTER TABLE dbo.tblDatapointFormula ADD CONSTRAINT
	PK_tblDatapointFormula PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblDatapointFormula SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDatapointFormula', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDatapointFormula', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDatapointFormula', 'Object', 'CONTROL') as Contr_Per 