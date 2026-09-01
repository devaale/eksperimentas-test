ALTER PROCEDURE [dbo].[prcSituation] ( 
	@userId nvarchar(128)
) AS BEGIN

	SELECT
		dbo.fncUnreadMessages(@userId, NULL) [NumOfUnreadMessages]
END
GO
