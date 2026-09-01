/*
   2022 m. lapkričio 10 d., ketvirtadienis17:42:35
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
CREATE TABLE dbo.tblPostReaction
	(
	Id int NOT NULL IDENTITY (1, 1),
	PostId int NOT NULL,
	UserId nvarchar(128) NOT NULL,
	Reaction int NOT NULL
	)  ON [PRIMARY]
GO
DECLARE @v sql_variant 
SET @v = N'Reaction type: 0 - None, 1 - Like.'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'tblPostReaction', N'COLUMN', N'Reaction'
GO
ALTER TABLE dbo.tblPostReaction ADD CONSTRAINT
	DF_tblPostReaction_Reaction DEFAULT 0 FOR Reaction
GO
ALTER TABLE dbo.tblPostReaction ADD CONSTRAINT
	PK_tblPostReaction PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblPostReaction SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblPostReaction', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblPostReaction', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblPostReaction', 'Object', 'CONTROL') as Contr_Per 