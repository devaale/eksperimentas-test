using System.Data;

using Experiment.Core.BL.Data;
using Experiment.Data.Models;

namespace Experiment.FormulaCalculation.Service
{
	internal sealed class FormulaCalculationContext
	{
		public FormulaCalculationContext(
			Datapoint datapoint,
			DataTable formulaTable,
			ExpSql db,
			string calculationLocation)
		{
			Datapoint = datapoint;
			FormulaTable = formulaTable;
			Db = db;
			CalculationLocation = calculationLocation;
		}

		public Datapoint Datapoint { get; }
		public DataTable FormulaTable { get; }
		public ExpSql Db { get; }
		public string CalculationLocation { get; }
	}
}
