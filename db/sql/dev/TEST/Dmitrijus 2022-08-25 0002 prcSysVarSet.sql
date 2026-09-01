USE [ExperimentDB]
GO
/****** Object:  StoredProcedure [dbo].[prcSysVarSet]    Script Date: 2022-08-25 10:36:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[prcSysVarSet] (
	@name en_sys_name,
	@value ntext,
	@module en_sys_name,
	@datatype en_name,
	@desc ntext
) AS BEGIN

	IF EXISTS (SELECT * FROM tblVars WHERE [name] = @name) BEGIN
		UPDATE
			tblVars
		SET 
			[value] = @value,	
			[module] = @module,	
			[datatype] = @datatype,		
			[desc] = @desc
		WHERE
			[name] = @name

	END ELSE BEGIN
		INSERT INTO 
			tblVars (
				[name],		[value],	[module],	[datatype],		[desc]
		) VALUES (
				@name,		@value,		@module,	@datatype,		@desc
		)
	END
END
