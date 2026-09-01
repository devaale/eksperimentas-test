-- =============================================
-- Migration: Add ObjectId to DashboardSetting
-- Date: 2026-02-06
-- Author: Evaldas
-- Description: Changes DashboardSetting from per-user to per-user-per-object
--              Adds Id as new PK, ObjectId as nullable (no FK constraint)
-- =============================================

BEGIN TRANSACTION;

BEGIN TRY

    -- =============================================
    -- tblDashboardSetting changes
    -- =============================================
    
    -- Drop existing primary key constraint (currently on UserId only)
    IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_tblDashboardSetting_1' AND parent_object_id = OBJECT_ID('tblDashboardSetting'))
    BEGIN
        ALTER TABLE [dbo].[tblDashboardSetting] DROP CONSTRAINT [PK_tblDashboardSetting_1];
    END

    -- Add Id column as new identity primary key (if not exists)
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tblDashboardSetting') AND name = 'Id')
    BEGIN
        ALTER TABLE [dbo].[tblDashboardSetting] ADD [Id] INT IDENTITY(1,1) NOT NULL;
    END

    -- Create new primary key on Id
    IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_tblDashboardSetting' AND parent_object_id = OBJECT_ID('tblDashboardSetting'))
    BEGIN
        ALTER TABLE [dbo].[tblDashboardSetting] 
        ADD CONSTRAINT [PK_tblDashboardSetting] PRIMARY KEY CLUSTERED ([Id]);
    END

    -- Add ObjectId column if it doesn't exist (NULLABLE, no FK)
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tblDashboardSetting') AND name = 'ObjectId')
    BEGIN
        ALTER TABLE [dbo].[tblDashboardSetting] ADD [ObjectId] INT NULL;
    END

    -- Create unique index on UserId + ObjectId for fast lookups
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblDashboardSetting_UserId_ObjectId' AND object_id = OBJECT_ID('tblDashboardSetting'))
    BEGIN
        CREATE UNIQUE NONCLUSTERED INDEX [IX_tblDashboardSetting_UserId_ObjectId] 
        ON [dbo].[tblDashboardSetting] ([UserId], [ObjectId]);
    END

    -- =============================================
    -- tblDashboardDatapoint changes
    -- =============================================

    -- Add ObjectId column if it doesn't exist (NULLABLE, no FK)
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('tblDashboardDatapoint') AND name = 'ObjectId')
    BEGIN
        ALTER TABLE [dbo].[tblDashboardDatapoint] ADD [ObjectId] INT NULL;
    END

    -- Create index for faster lookups by UserId + ObjectId
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblDashboardDatapoint_UserId_ObjectId' AND object_id = OBJECT_ID('tblDashboardDatapoint'))
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_tblDashboardDatapoint_UserId_ObjectId] 
        ON [dbo].[tblDashboardDatapoint] ([UserId], [ObjectId]);
    END

    COMMIT TRANSACTION;
    PRINT 'Migration completed successfully!';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Migration failed: ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO