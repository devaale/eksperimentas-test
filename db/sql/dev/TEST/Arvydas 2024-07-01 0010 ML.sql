-- ML
EXEC prcUiWordUpdate 'topics', N'Topics', N'Temos', N'Темы'
EXEC prcUiWordUpdate 'newTopic', N'New topic', N'Nauja tema', N'Новая тема'
EXEC prcUiWordUpdate 'topicNotEmpty', N'Topic can''t be empty', N'Tema negali būti tuščia.', N'Тема не может быть пустой'
EXEC prcUiWordUpdate 'nDeviceTopics', N'Topics of {0}', N'Temos: {0}', N'Темы: {0}'

-- SYS VARS

IF NOT EXISTS(SELECT * FROM tblVars WHERE [name] = 'MQTT_LOG_LEVEL') BEGIN
	INSERT INTO [tblVars] (
		[name],				[value],	[module],	[datatype], [desc]
	) VALUES (
		'MQTT_LOG_LEVEL',	'5',		'MQTT',		'int',		'Log level of MQTT service'
	)
END


select * from tblUiWord where alias LIKE '%empty%'