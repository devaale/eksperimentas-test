/*
   2023 m. vasario 3 d., penktadienis00:47:00
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
ALTER TABLE dbo.tblOrder SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblOrder', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblOrder', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblOrder', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblOrderDetail ADD CONSTRAINT
	FK_tblOrderDetail_tblOrder FOREIGN KEY
	(
	OrderId
	) REFERENCES dbo.tblOrder
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblOrderDetail
	NOCHECK CONSTRAINT FK_tblOrderDetail_tblOrder
GO
ALTER TABLE dbo.tblOrderDetail SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblOrderDetail', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblOrderDetail', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblOrderDetail', 'Object', 'CONTROL') as Contr_Per 