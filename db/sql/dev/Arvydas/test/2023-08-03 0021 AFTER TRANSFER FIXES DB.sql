UPDATE tblDatapoint
SET MeasureUnit = 'em'
WHERE MeasureUnit IS NULL

UPDATE tblDatapoint
SET RegisterType = 16000
WHERE RegisterType = 16

