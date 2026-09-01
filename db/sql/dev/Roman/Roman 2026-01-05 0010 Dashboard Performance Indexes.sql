-- Dashboard Performance Optimization Indexes
-- Created: 2026-01-05
-- Purpose: Improve dashboard loading performance by adding indexes for common queries

-- 1. Composite index on tblDatapointValue for date range queries with DatapointId filter
-- This is the most critical index for dashboard chart queries
-- The stored procedure prcDatapointValueList filters by both DatapointId and Date
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblDatapointValue_DatapointId_Date' AND object_id = OBJECT_ID('dbo.tblDatapointValue'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_tblDatapointValue_DatapointId_Date
	ON [dbo].[tblDatapointValue] ([DatapointId], [Date])
	INCLUDE ([Value])
	WITH (ONLINE = ON, FILLFACTOR = 90);
	
	PRINT 'Created index: IX_tblDatapointValue_DatapointId_Date';
END
ELSE
BEGIN
	PRINT 'Index IX_tblDatapointValue_DatapointId_Date already exists';
END
GO

-- 2. Index on Date column for date range queries (if not already covered by composite index)
-- This helps with queries that filter only by date range
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblDatapointValue_Date' AND object_id = OBJECT_ID('dbo.tblDatapointValue'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_tblDatapointValue_Date
	ON [dbo].[tblDatapointValue] ([Date])
	INCLUDE ([DatapointId], [Value])
	WITH (ONLINE = ON, FILLFACTOR = 90);
	
	PRINT 'Created index: IX_tblDatapointValue_Date';
END
ELSE
BEGIN
	PRINT 'Index IX_tblDatapointValue_Date already exists';
END
GO

-- 3. Index on tblDashboardDatapoint for faster user dashboard settings retrieval
-- Note: Table name is tblDashboardDatapoint (singular), not DashboardDatapoints
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblDashboardDatapoint_UserId_GraphId' AND object_id = OBJECT_ID('dbo.tblDashboardDatapoint'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_tblDashboardDatapoint_UserId_GraphId
	ON [dbo].[tblDashboardDatapoint] ([UserId], [GraphId])
	INCLUDE ([DatapointId])
	WITH (ONLINE = ON, FILLFACTOR = 90);
	
	PRINT 'Created index: IX_tblDashboardDatapoint_UserId_GraphId';
END
ELSE
BEGIN
	PRINT 'Index IX_tblDashboardDatapoint_UserId_GraphId already exists';
END
GO

-- 4. Verify existing index on DatapointId (should already exist from 2024-01-09)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblDatapointValue_DatapointId' AND object_id = OBJECT_ID('dbo.tblDatapointValue'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_tblDatapointValue_DatapointId
	ON [dbo].[tblDatapointValue] ([DatapointId])
	INCLUDE ([Date],[Value])
	WITH (ONLINE = ON, FILLFACTOR = 90);
	
	PRINT 'Created index: IX_tblDatapointValue_DatapointId';
END
ELSE
BEGIN
	PRINT 'Index IX_tblDatapointValue_DatapointId already exists';
END
GO

-- Update statistics for better query optimization
UPDATE STATISTICS [dbo].[tblDatapointValue] WITH FULLSCAN;
UPDATE STATISTICS [dbo].[tblDashboardDatapoint] WITH FULLSCAN;

PRINT 'Statistics updated';
GO
