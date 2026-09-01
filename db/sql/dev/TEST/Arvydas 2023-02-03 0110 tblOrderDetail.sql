/*
   2023 m. vasario 3 d., penktadienis00:46:23
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
CREATE TABLE dbo.tblOrderDetail
	(
	Id int NOT NULL IDENTITY (1, 1),
	OrderId uniqueidentifier NOT NULL,
	LicenseType int NOT NULL,
	NumMonths int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tblOrderDetail ADD CONSTRAINT
	DF_tblOrderDetail_LicenseType DEFAULT 0 FOR LicenseType
GO
ALTER TABLE dbo.tblOrderDetail ADD CONSTRAINT
	DF_tblOrderDetail_NumMonths DEFAULT 0 FOR NumMonths
GO
ALTER TABLE dbo.tblOrderDetail ADD CONSTRAINT
	PK_tblOrderDetail PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblOrderDetail SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblOrderDetail', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblOrderDetail', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblOrderDetail', 'Object', 'CONTROL') as Contr_Per 