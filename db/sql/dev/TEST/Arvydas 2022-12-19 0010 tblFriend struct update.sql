/*
   2022 m. gruodžio 19 d., pirmadienis15:22:18
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
ALTER TABLE dbo.tblFriend
	DROP CONSTRAINT FK_tblFriend_AspNetUsers_UserId
GO
ALTER TABLE dbo.tblFriend
	DROP CONSTRAINT FK_tblFriend_AspNetUsers_FriendUserId
GO
ALTER TABLE dbo.AspNetUsers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_tblFriend
	(
	Id int NOT NULL IDENTITY (1, 1),
	UserId nvarchar(128) NOT NULL,
	RelatedUserId nvarchar(128) NOT NULL,
	Name  AS ([dbo].[fncUsernameById]([RelatedUserId]))
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tblFriend SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_tblFriend ON
GO
IF EXISTS(SELECT * FROM dbo.tblFriend)
	 EXEC('INSERT INTO dbo.Tmp_tblFriend (Id, UserId, RelatedUserId)
		SELECT Id, UserId, FriendUserId FROM dbo.tblFriend WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_tblFriend OFF
GO
DROP TABLE dbo.tblFriend
GO
EXECUTE sp_rename N'dbo.Tmp_tblFriend', N'tblFriend', 'OBJECT' 
GO
ALTER TABLE dbo.tblFriend ADD CONSTRAINT
	PK_tblFriend PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblFriend WITH NOCHECK ADD CONSTRAINT
	FK_tblFriend_AspNetUsers_UserId FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblFriend
	NOCHECK CONSTRAINT FK_tblFriend_AspNetUsers_UserId
GO
ALTER TABLE dbo.tblFriend WITH NOCHECK ADD CONSTRAINT
	FK_tblFriend_AspNetUsers_FriendUserId FOREIGN KEY
	(
	RelatedUserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblFriend
	NOCHECK CONSTRAINT FK_tblFriend_AspNetUsers_FriendUserId
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblFriend', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblFriend', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblFriend', 'Object', 'CONTROL') as Contr_Per 