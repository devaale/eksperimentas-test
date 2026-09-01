/*
   2022 m. rugsėjo 14 d., trečiadienis17:18:21
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
CREATE TABLE dbo.tblFriend
	(
	Id int NOT NULL IDENTITY (1, 1),
	Name nvarchar(256) NOT NULL,
	Email nvarchar(512) NOT NULL,
	UserId nvarchar(128) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tblFriend ADD CONSTRAINT
	PK_tblFriend PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblFriend SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblFriend', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblFriend', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblFriend', 'Object', 'CONTROL') as Contr_Per 