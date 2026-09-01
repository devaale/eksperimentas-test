/*
   2024 m. rugpjūčio 29 d., ketvirtadienis21:25:51
   User: 
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
ALTER TABLE dbo.tblDatapointSetting
	DROP CONSTRAINT DF_tblDatapointSetting_Direction
GO
ALTER TABLE dbo.tblDatapointSetting
	DROP CONSTRAINT DF_tblDatapointSetting_ValueType
GO
ALTER TABLE dbo.tblDatapointSetting
	DROP CONSTRAINT DF_tblDatapointSetting_Mandatory
GO
CREATE TABLE dbo.Tmp_tblDatapointSetting
	(
	Id int NOT NULL IDENTITY (1, 1),
	Protocol int NOT NULL,
	Name dbo.en_name NOT NULL,
	Description dbo.en_desc NULL,
	Direction tinyint NOT NULL,
	ValueType tinyint NOT NULL,
	Mandatory bit NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tblDatapointSetting SET (LOCK_ESCALATION = TABLE)
GO
DECLARE @v sql_variant 
SET @v = N'0 - None, 1 - In, 2 - Out, 3 - Both'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_tblDatapointSetting', N'COLUMN', N'Direction'
GO
DECLARE @v sql_variant 
SET @v = N'0 - Normal, 1 - Boolean, 3 - Current time'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_tblDatapointSetting', N'COLUMN', N'ValueType'
GO
ALTER TABLE dbo.Tmp_tblDatapointSetting ADD CONSTRAINT
	DF_tblDatapointSetting_Direction DEFAULT ((0)) FOR Direction
GO
ALTER TABLE dbo.Tmp_tblDatapointSetting ADD CONSTRAINT
	DF_tblDatapointSetting_ValueType DEFAULT ((0)) FOR ValueType
GO
ALTER TABLE dbo.Tmp_tblDatapointSetting ADD CONSTRAINT
	DF_tblDatapointSetting_Mandatory DEFAULT ((0)) FOR Mandatory
GO
SET IDENTITY_INSERT dbo.Tmp_tblDatapointSetting ON
GO
IF EXISTS(SELECT * FROM dbo.tblDatapointSetting)
	 EXEC('INSERT INTO dbo.Tmp_tblDatapointSetting (Id, Name, Description, Direction, ValueType, Mandatory)
		SELECT Id, Name, Description, Direction, ValueType, Mandatory FROM dbo.tblDatapointSetting WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_tblDatapointSetting OFF
GO
DROP TABLE dbo.tblDatapointSetting
GO
EXECUTE sp_rename N'dbo.Tmp_tblDatapointSetting', N'tblDatapointSetting', 'OBJECT' 
GO
ALTER TABLE dbo.tblDatapointSetting ADD CONSTRAINT
	PK_tblDatapointSetting PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDatapointSetting', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDatapointSetting', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDatapointSetting', 'Object', 'CONTROL') as Contr_Per 