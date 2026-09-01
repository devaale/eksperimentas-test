/*
   2023 m. vasario 15 d., trečiadienis01:18:01
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
ALTER TABLE dbo.tblOrder
	DROP CONSTRAINT FK_tblOrder_AspNetUsers
GO
ALTER TABLE dbo.AspNetUsers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblOrder
	DROP CONSTRAINT DF_tblOrder_Id
GO
ALTER TABLE dbo.tblOrder
	DROP CONSTRAINT DF_tblOrder_FullPrice
GO
ALTER TABLE dbo.tblOrder
	DROP CONSTRAINT DF_tblOrder_UsedTokens
GO
ALTER TABLE dbo.tblOrder
	DROP CONSTRAINT DF_tblOrder_Discount
GO
ALTER TABLE dbo.tblOrder
	DROP CONSTRAINT DF_tblOrder_FinalPrice
GO
ALTER TABLE dbo.tblOrder
	DROP CONSTRAINT DF_tblOrder_State
GO
CREATE TABLE dbo.Tmp_tblOrder
	(
	Id uniqueidentifier NOT NULL,
	UserId nvarchar(128) NOT NULL,
	FullPrice money NOT NULL,
	UsedTokens int NOT NULL,
	Discount money NOT NULL,
	FinalPrice money NOT NULL,
	PaymentMethodId nvarchar(64) NULL,
	PaymentMethod nvarchar(128) NULL,
	State tinyint NOT NULL,
	Posted datetime NOT NULL,
	PostedIp nvarchar(32) NULL,
	Completed datetime NULL,
	CompletedIp nvarchar(32) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tblOrder SET (LOCK_ESCALATION = TABLE)
GO
DECLARE @v sql_variant 
SET @v = N'Order Unique Key Id'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_tblOrder', N'COLUMN', N'Id'
GO
DECLARE @v sql_variant 
SET @v = N'UserId'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_tblOrder', N'COLUMN', N'UserId'
GO
DECLARE @v sql_variant 
SET @v = N'Full price before discount calculated'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_tblOrder', N'COLUMN', N'FullPrice'
GO
DECLARE @v sql_variant 
SET @v = N'How many tokens for discount used, each token 2%'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_tblOrder', N'COLUMN', N'UsedTokens'
GO
DECLARE @v sql_variant 
SET @v = N'Discount sum'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_tblOrder', N'COLUMN', N'Discount'
GO
DECLARE @v sql_variant 
SET @v = N'Final price, what to pay for user'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_tblOrder', N'COLUMN', N'FinalPrice'
GO
DECLARE @v sql_variant 
SET @v = N'Payment method Id, which returned from Billing Back-end'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_tblOrder', N'COLUMN', N'PaymentMethodId'
GO
DECLARE @v sql_variant 
SET @v = N'Payment Method name'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_tblOrder', N'COLUMN', N'PaymentMethod'
GO
DECLARE @v sql_variant 
SET @v = N'Current state of the order'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_tblOrder', N'COLUMN', N'State'
GO
ALTER TABLE dbo.Tmp_tblOrder ADD CONSTRAINT
	DF_tblOrder_Id DEFAULT (newid()) FOR Id
GO
ALTER TABLE dbo.Tmp_tblOrder ADD CONSTRAINT
	DF_tblOrder_FullPrice DEFAULT ((0)) FOR FullPrice
GO
ALTER TABLE dbo.Tmp_tblOrder ADD CONSTRAINT
	DF_tblOrder_UsedTokens DEFAULT ((0)) FOR UsedTokens
GO
ALTER TABLE dbo.Tmp_tblOrder ADD CONSTRAINT
	DF_tblOrder_Discount DEFAULT ((0)) FOR Discount
GO
ALTER TABLE dbo.Tmp_tblOrder ADD CONSTRAINT
	DF_tblOrder_FinalPrice DEFAULT ((0)) FOR FinalPrice
GO
ALTER TABLE dbo.Tmp_tblOrder ADD CONSTRAINT
	DF_tblOrder_State DEFAULT ((0)) FOR State
GO
IF EXISTS(SELECT * FROM dbo.tblOrder)
	 EXEC('INSERT INTO dbo.Tmp_tblOrder (Id, UserId, FullPrice, UsedTokens, Discount, FinalPrice, PaymentMethodId, PaymentMethod, State, Posted, Completed)
		SELECT Id, UserId, FullPrice, UsedTokens, Discount, FinalPrice, PaymentMethodId, PaymentMethod, State, Posted, Completed FROM dbo.tblOrder WITH (HOLDLOCK TABLOCKX)')
GO
ALTER TABLE dbo.tblLicense
	DROP CONSTRAINT FK_tblLicense_tblOrder
GO
ALTER TABLE dbo.tblOrderDetail
	DROP CONSTRAINT FK_tblOrderDetail_tblOrder
GO
DROP TABLE dbo.tblOrder
GO
EXECUTE sp_rename N'dbo.Tmp_tblOrder', N'tblOrder', 'OBJECT' 
GO
ALTER TABLE dbo.tblOrder ADD CONSTRAINT
	PK_tblOrder PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblOrder WITH NOCHECK ADD CONSTRAINT
	FK_tblOrder_AspNetUsers FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblOrder
	NOCHECK CONSTRAINT FK_tblOrder_AspNetUsers
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblOrder', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblOrder', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblOrder', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblOrderDetail WITH NOCHECK ADD CONSTRAINT
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
select Has_Perms_By_Name(N'dbo.tblOrderDetail', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblOrderDetail', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblOrderDetail', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblLicense WITH NOCHECK ADD CONSTRAINT
	FK_tblLicense_tblOrder FOREIGN KEY
	(
	OrderId
	) REFERENCES dbo.tblOrder
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.tblLicense
	NOCHECK CONSTRAINT FK_tblLicense_tblOrder
GO
ALTER TABLE dbo.tblLicense SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblLicense', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblLicense', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblLicense', 'Object', 'CONTROL') as Contr_Per 