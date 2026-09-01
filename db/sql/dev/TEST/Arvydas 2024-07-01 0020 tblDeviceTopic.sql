/*
   2024 m. liepos 1 d., pirmadienis15:50:20
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
CREATE TABLE dbo.tblDeviceTopic
	(
	Id int NOT NULL IDENTITY (1, 1),
	DeviceId int NOT NULL,
	Topic nvarchar(64) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tblDeviceTopic ADD CONSTRAINT
	DF_tblDeviceTopic_DeviceId DEFAULT 0 FOR DeviceId
GO
ALTER TABLE dbo.tblDeviceTopic ADD CONSTRAINT
	PK_tblDeviceTopic PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblDeviceTopic SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDeviceTopic', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDeviceTopic', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDeviceTopic', 'Object', 'CONTROL') as Contr_Per 