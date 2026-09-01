DROP PROCEDURE IF EXISTS prcSituation
GO

CREATE PROCEDURE prcSituation ( 
	@userId nvarchar(128)
) AS BEGIN

	SELECT
		dbo.fncUnreadMessages(@userId) [NumOfUnreadMessages]
		

END
GO

EXEC prcSituation N'26b33240-b13e-406d-a2a3-b4e90af3c459'