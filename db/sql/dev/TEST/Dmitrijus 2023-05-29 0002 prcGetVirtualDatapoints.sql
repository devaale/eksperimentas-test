CREATE PROCEDURE [dbo].[prcGetVirtualDatapoints] AS BEGIN

	SELECT *
	FROM
		tblDatapoint dtp
	WHERE
		dtp.DatapointType = 2
		
END
