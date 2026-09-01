DROP PROCEDURE IF EXISTS prcMainTree
GO

CREATE PROCEDURE prcMainTree (
	@userId nvarchar(128)
) AS BEGIN

	DECLARE @DEV_P varchar(3), @DTP_P varchar(3)
	SELECT @DEV_P = 'dev', @DTP_P = 'dtp'

 SELECT 
	@DEV_P + CAST(DEV.Id AS VARCHAR) [Id]
	,CAST('#' AS VARCHAR) [Parent]
	,DEV.Name [Text]
	,@DEV_P [Type]
 FROM	
	tblDevice DEV
 WHERE
	DEV.UserId = @userId

UNION ALL

SELECT
	@DTP_P + CAST(DTP.Id AS VARCHAR) Id
	,@DEV_P + CAST(DTP.DeviceId AS VARCHAR) Parent
	,DTP.Name [Text]
	,@DTP_P [Type]
FROM tblDatapoint DTP
WHERE DTP.DeviceId IN 
	(SELECT Id FROM tblDevice WHERE UserId = @userId)


END
GO

EXEC prcMainTree '26b33240-b13e-406d-a2a3-b4e90af3c459'