CREATE NONCLUSTERED INDEX IX_tblDatapointValue_DatapointId
ON [dbo].[tblDatapointValue] ([DatapointId])
INCLUDE ([Date],[Value])
