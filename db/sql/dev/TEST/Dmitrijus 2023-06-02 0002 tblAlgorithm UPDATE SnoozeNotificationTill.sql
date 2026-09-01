USE [ExperimentDB]
GO

UPDATE [dbo].[tblAlgorithm]
   SET SnoozeNotificationTill = GETDATE()
GO


