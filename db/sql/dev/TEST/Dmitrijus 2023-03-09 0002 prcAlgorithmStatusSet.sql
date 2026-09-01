USE [ExperimentDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[prcAlgorithmStatusSet] (
	@AlgorithmId int, 
	@Status decimal
) AS BEGIN

	UPDATE
		tblAlgorithm
	SET 
		[Status] = @Status
	WHERE
		Id = @AlgorithmId
END