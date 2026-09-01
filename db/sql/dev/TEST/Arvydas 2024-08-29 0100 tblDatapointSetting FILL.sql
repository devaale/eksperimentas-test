BEGIN TRAN

DELETE FROM tblDatapointSetting WHERE Protocol = 100

INSERT INTO tblDatapointSetting (
	[Protocol], [Direction], [ValueType], [Mandatory], [Name], [Description]
) VALUES 
	(100, 2, 0, 1, 'temp_inside', 'Temperature inside' ),
	(100, 2, 0, 0, 'temp_outside', 'Temperature outside' ),
	(100, 2, 0, 1, 'temp_target_now', 'Currently desired temperature' ),
	(100, 2, 0, 0, 'temp_target_user_in_building', 'The desired temperature when the user is in the building' ),
	(100, 2, 0, 0, 'temp_target_user_away', 'The desired temperature when an user is away' ),
	(100, 2, 0, 0, 'user_in_building_from', 'The hour (0-24) from which the user will be in the building' ),
	(100, 2, 0, 0, 'user_in_building_till', 'The hour from which the user will not be in the building' ),
	(100, 2, 3, 1, 'time_now', 'Current date time' ),
	(100, 1, 0, 1, 'decision', 'Decision: 0 - DoNothing, 1 - TurnOnHeating, 2 - TurnOnCooling.' )

COMMIT TRAN

SELECT * FROM tblDatapointSetting