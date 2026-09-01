USE [ExperimentDB]
GO
/****** Object:  StoredProcedure [dbo].[prcSysInstall]    Script Date: 2022-08-25 10:35:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
	-- tblVars
	EXEC prcSysVarSet 'SCAN_LOG_LEVEL',			'5',					'Scan',		'int',			'Įrenginių skenavimo tarnybos log lygis, kokio lygio pranešimus loginti.'
	EXEC prcSysVarSet 'SCAN_LOG_LOCATION',		'c:\temp\exp_logs\',	'Scan',		'text',			'Katalogas, kur saugoti skenavimo tarnybos log bylą.'
	EXEC prcSysVarSet 'SCAN_LOG_USE_DATES',		'1',					'Scan',		'bit (0 or 1)',	'Ar naudoti datas skenavimo tarnybos log bylų pavadinimuose.'
	EXEC prcSysVarSet 'SCAN_LOOP_DELAY',		'15',					'Scan',		'int',			'Kiek laiko laukti po vieno apklausos ciklo.'