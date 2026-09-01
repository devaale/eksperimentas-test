-- After [Name]  moved [Language]
-- After [Language] added [Tokens] int NOT NULL, default 0
-- Field Language 

/*
   2022 m. gruodžio 21 d., trečiadienis19:17:32
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
ALTER TABLE dbo.AspNetUsers
	DROP CONSTRAINT DF_AspNetUsers_Language
GO
CREATE TABLE dbo.Tmp_AspNetUsers
	(
	Id nvarchar(128) NOT NULL,
	Name nvarchar(128) NOT NULL,
	Language nvarchar(3) NOT NULL,
	Tokens int NOT NULL,
	Email nvarchar(256) NULL,
	EmailConfirmed bit NOT NULL,
	PasswordHash nvarchar(MAX) NULL,
	SecurityStamp nvarchar(MAX) NULL,
	PhoneNumber nvarchar(MAX) NULL,
	PhoneNumberConfirmed bit NOT NULL,
	TwoFactorEnabled bit NOT NULL,
	LockoutEndDateUtc datetime NULL,
	LockoutEnabled bit NOT NULL,
	AccessFailedCount int NOT NULL,
	UserName nvarchar(256) NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_AspNetUsers SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_AspNetUsers ADD CONSTRAINT
	DF_AspNetUsers_Language DEFAULT ('en') FOR Language
GO
ALTER TABLE dbo.Tmp_AspNetUsers ADD CONSTRAINT
	DF_AspNetUsers_Tokens DEFAULT 0 FOR Tokens
GO
IF EXISTS(SELECT * FROM dbo.AspNetUsers)
	 EXEC('INSERT INTO dbo.Tmp_AspNetUsers (Id, Name, Language, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName)
		SELECT Id, Name, Language, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName FROM dbo.AspNetUsers WITH (HOLDLOCK TABLOCKX)')
GO
ALTER TABLE dbo.tblPostReaction
	DROP CONSTRAINT FK_tblPostReaction_AspNetUsers
GO
ALTER TABLE dbo.tblObjectPermission
	DROP CONSTRAINT FK_tblObjectPermission_FriendUserId
GO
ALTER TABLE dbo.tblPost
	DROP CONSTRAINT FK_tblPost_AspNetUsers
GO
ALTER TABLE dbo.tblBlocked
	DROP CONSTRAINT FK_tblBlocked_AspNetUsers_UserId
GO
ALTER TABLE dbo.tblBlocked
	DROP CONSTRAINT FK_tblBlocked_AspNetUsers_BlockedUserId
GO
ALTER TABLE dbo.tblMessage
	DROP CONSTRAINT FK_tblMessage_AspNetUsers_SenderUserId
GO
ALTER TABLE dbo.tblFriend
	DROP CONSTRAINT FK_tblFriend_AspNetUsers_UserId
GO
ALTER TABLE dbo.tblMessage
	DROP CONSTRAINT FK_tblMessage_AspNetUsers_ReceiverUserId
GO
ALTER TABLE dbo.tblFriend
	DROP CONSTRAINT FK_tblFriend_AspNetUsers_FriendUserId
GO
ALTER TABLE dbo.tblObject
	DROP CONSTRAINT FK_tblObject_AspNetUsers
GO
ALTER TABLE dbo.AspNetUserLogins
	DROP CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO
ALTER TABLE dbo.AspNetUserClaims
	DROP CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO
ALTER TABLE dbo.AspNetUserRoles
	DROP CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO
DROP TABLE dbo.AspNetUsers
GO
EXECUTE sp_rename N'dbo.Tmp_AspNetUsers', N'AspNetUsers', 'OBJECT' 
GO
ALTER TABLE dbo.AspNetUsers ADD CONSTRAINT
	[PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE UNIQUE NONCLUSTERED INDEX UserNameIndex ON dbo.AspNetUsers
	(
	UserName
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
COMMIT
select Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.AspNetUserRoles ADD CONSTRAINT
	[FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.AspNetUserRoles SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.AspNetUserRoles', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.AspNetUserRoles', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.AspNetUserRoles', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.AspNetUserClaims ADD CONSTRAINT
	[FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.AspNetUserClaims SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.AspNetUserClaims', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.AspNetUserClaims', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.AspNetUserClaims', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.AspNetUserLogins ADD CONSTRAINT
	[FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.AspNetUserLogins SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.AspNetUserLogins', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.AspNetUserLogins', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.AspNetUserLogins', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblObject WITH NOCHECK ADD CONSTRAINT
	FK_tblObject_AspNetUsers FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblObject
	NOCHECK CONSTRAINT FK_tblObject_AspNetUsers
GO
ALTER TABLE dbo.tblObject SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblObject', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblObject', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblObject', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
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
ALTER TABLE dbo.tblFriend SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblFriend', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblFriend', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblFriend', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblMessage WITH NOCHECK ADD CONSTRAINT
	FK_tblMessage_AspNetUsers_SenderUserId FOREIGN KEY
	(
	SenderUserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblMessage
	NOCHECK CONSTRAINT FK_tblMessage_AspNetUsers_SenderUserId
GO
ALTER TABLE dbo.tblMessage WITH NOCHECK ADD CONSTRAINT
	FK_tblMessage_AspNetUsers_ReceiverUserId FOREIGN KEY
	(
	ReceiverUserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblMessage
	NOCHECK CONSTRAINT FK_tblMessage_AspNetUsers_ReceiverUserId
GO
ALTER TABLE dbo.tblMessage SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblMessage', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblMessage', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblMessage', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblBlocked WITH NOCHECK ADD CONSTRAINT
	FK_tblBlocked_AspNetUsers_UserId FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblBlocked
	NOCHECK CONSTRAINT FK_tblBlocked_AspNetUsers_UserId
GO
ALTER TABLE dbo.tblBlocked WITH NOCHECK ADD CONSTRAINT
	FK_tblBlocked_AspNetUsers_BlockedUserId FOREIGN KEY
	(
	RelatedUserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblBlocked
	NOCHECK CONSTRAINT FK_tblBlocked_AspNetUsers_BlockedUserId
GO
ALTER TABLE dbo.tblBlocked SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblBlocked', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblBlocked', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblBlocked', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblPost WITH NOCHECK ADD CONSTRAINT
	FK_tblPost_AspNetUsers FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblPost
	NOCHECK CONSTRAINT FK_tblPost_AspNetUsers
GO
ALTER TABLE dbo.tblPost SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblPost', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblPost', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblPost', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblObjectPermission WITH NOCHECK ADD CONSTRAINT
	FK_tblObjectPermission_FriendUserId FOREIGN KEY
	(
	FriendUserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblObjectPermission
	NOCHECK CONSTRAINT FK_tblObjectPermission_FriendUserId
GO
ALTER TABLE dbo.tblObjectPermission SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblObjectPermission', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblObjectPermission', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblObjectPermission', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblPostReaction WITH NOCHECK ADD CONSTRAINT
	FK_tblPostReaction_AspNetUsers FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblPostReaction
	NOCHECK CONSTRAINT FK_tblPostReaction_AspNetUsers
GO
ALTER TABLE dbo.tblPostReaction SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblPostReaction', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblPostReaction', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblPostReaction', 'Object', 'CONTROL') as Contr_Per 