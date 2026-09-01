/*
   2023 m. vasario 3 d., penktadienis00:40:57
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
CREATE TABLE dbo.tblOrder
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
	Completed datetime NULL
	)  ON [PRIMARY]
GO
DECLARE @v sql_variant 
SET @v = N'Order Unique Key Id'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'tblOrder', N'COLUMN', N'Id'
GO
DECLARE @v sql_variant 
SET @v = N'UserId'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'tblOrder', N'COLUMN', N'UserId'
GO
DECLARE @v sql_variant 
SET @v = N'Full price before discount calculated'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'tblOrder', N'COLUMN', N'FullPrice'
GO
DECLARE @v sql_variant 
SET @v = N'How many tokens for discount used, each token 2%'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'tblOrder', N'COLUMN', N'UsedTokens'
GO
DECLARE @v sql_variant 
SET @v = N'Discount sum'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'tblOrder', N'COLUMN', N'Discount'
GO
DECLARE @v sql_variant 
SET @v = N'Final price, what to pay for user'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'tblOrder', N'COLUMN', N'FinalPrice'
GO
DECLARE @v sql_variant 
SET @v = N'Payment method Id, which returned from Billing Back-end'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'tblOrder', N'COLUMN', N'PaymentMethodId'
GO
DECLARE @v sql_variant 
SET @v = N'Payment Method name'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'tblOrder', N'COLUMN', N'PaymentMethod'
GO
DECLARE @v sql_variant 
SET @v = N'Current state of the order'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'tblOrder', N'COLUMN', N'State'
GO
ALTER TABLE dbo.tblOrder ADD CONSTRAINT
	DF_tblOrder_FullPrice DEFAULT 0 FOR FullPrice
GO
ALTER TABLE dbo.tblOrder ADD CONSTRAINT
	DF_tblOrder_UsedTokens DEFAULT 0 FOR UsedTokens
GO
ALTER TABLE dbo.tblOrder ADD CONSTRAINT
	DF_tblOrder_Discount DEFAULT 0 FOR Discount
GO
ALTER TABLE dbo.tblOrder ADD CONSTRAINT
	DF_tblOrder_FinalPrice DEFAULT 0 FOR FinalPrice
GO
ALTER TABLE dbo.tblOrder ADD CONSTRAINT
	DF_tblOrder_State DEFAULT 0 FOR State
GO
ALTER TABLE dbo.tblOrder ADD CONSTRAINT
	PK_tblOrder PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblOrder SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblOrder', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblOrder', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblOrder', 'Object', 'CONTROL') as Contr_Per 