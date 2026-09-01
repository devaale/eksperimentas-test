DROP PROCEDURE IF EXISTS prcDatapointFormulaList
GO
CREATE PROCEDURE prcDatapointFormulaList (@lang varchar(3)) AS BEGIN

	SELECT 
		DF.*
		,UW.[text] [Name]
	FROM
		tblDatapointFormula DF
	INNER JOIN
		tblUiWord UW ON DF.Alias = UW.Alias AND UW.Code = @lang
END
GO