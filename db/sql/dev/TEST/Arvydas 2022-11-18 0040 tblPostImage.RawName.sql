/*
   2022 m. lapkričio 18 d., penktadienis21:56:12
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
ALTER TABLE dbo.tblPostImage
	DROP CONSTRAINT FK_tblPostImage_tblPost
GO
ALTER TABLE dbo.tblPost SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblPost', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblPost', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblPost', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblPostImage
	DROP CONSTRAINT DF_tblPostImage_rawId
GO
CREATE TABLE dbo.Tmp_tblPostImage
	(
	Id uniqueidentifier NOT NULL,
	PostId int NOT NULL,
	ContentType nvarchar(128) NULL,
	Name nvarchar(256) NOT NULL,
	RawName dbo.en_url NULL,
	ImageUrl dbo.en_url NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tblPostImage SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_tblPostImage ADD CONSTRAINT
	DF_tblPostImage_rawId DEFAULT (newid()) FOR Id
GO
IF EXISTS(SELECT * FROM dbo.tblPostImage)
	 EXEC('INSERT INTO dbo.Tmp_tblPostImage (Id, PostId, ContentType, Name, ImageUrl)
		SELECT Id, PostId, ContentType, Name, ImageUrl FROM dbo.tblPostImage WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.tblPostImage
GO
EXECUTE sp_rename N'dbo.Tmp_tblPostImage', N'tblPostImage', 'OBJECT' 
GO
ALTER TABLE dbo.tblPostImage ADD CONSTRAINT
	PK_tblPostImage_1 PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblPostImage WITH NOCHECK ADD CONSTRAINT
	FK_tblPostImage_tblPost FOREIGN KEY
	(
	PostId
	) REFERENCES dbo.tblPost
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblPostImage
	NOCHECK CONSTRAINT FK_tblPostImage_tblPost
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblPostImage', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblPostImage', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblPostImage', 'Object', 'CONTROL') as Contr_Per 