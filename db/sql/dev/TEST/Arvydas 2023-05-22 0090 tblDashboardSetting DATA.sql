DELETE FROM tblDashboardDatapoint
DELETE FROM tblDashboardSetting
GO

INSERT INTO tblDashboardSetting (
	UserId, IntervalDatepart
) 
SELECT Id, 4 
FROM AspNetUsers 
GO