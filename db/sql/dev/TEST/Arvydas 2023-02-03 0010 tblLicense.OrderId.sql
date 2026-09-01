/*
   2023 m. vasario 3 d., penktadienis00:29:55
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
ALTER TABLE dbo.tblLicense
	DROP CONSTRAINT FK_tblLicense_AspNetUsers
GO
ALTER TABLE dbo.AspNetUsers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblLicense
	DROP CONSTRAINT DF_tblLicense_Type
GO
ALTER TABLE dbo.tblLicense
	DROP CONSTRAINT DF_tblLicense_Active
GO
CREATE TABLE dbo.Tmp_tblLicense
	(
	Id int NOT NULL IDENTITY (1, 1),
	UserId nvarchar(128) NOT NULL,
	OrderId uniqueidentifier NULL,
	Type int NOT NULL,
	ValidFrom datetime NOT NULL,
	ValidUntil datetime NOT NULL,
	Active bit NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tblLicense SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_tblLicense ADD CONSTRAINT
	DF_tblLicense_Type DEFAULT ((0)) FOR Type
GO
ALTER TABLE dbo.Tmp_tblLicense ADD CONSTRAINT
	DF_tblLicense_Active DEFAULT ((0)) FOR Active
GO
SET IDENTITY_INSERT dbo.Tmp_tblLicense ON
GO
IF EXISTS(SELECT * FROM dbo.tblLicense)
	 EXEC('INSERT INTO dbo.Tmp_tblLicense (Id, UserId, Type, ValidFrom, ValidUntil, Active)
		SELECT Id, UserId, Type, ValidFrom, ValidUntil, Active FROM dbo.tblLicense WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_tblLicense OFF
GO
DROP TABLE dbo.tblLicense
GO
EXECUTE sp_rename N'dbo.Tmp_tblLicense', N'tblLicense', 'OBJECT' 
GO
ALTER TABLE dbo.tblLicense ADD CONSTRAINT
	PK_tblLicense PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblLicense WITH NOCHECK ADD CONSTRAINT
	FK_tblLicense_AspNetUsers FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblLicense
	NOCHECK CONSTRAINT FK_tblLicense_AspNetUsers
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblLicense', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblLicense', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblLicense', 'Object', 'CONTROL') as Contr_Per 