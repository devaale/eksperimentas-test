using System;
using System.Data;

using Experiment.Core;
using Experiment.Core.IO;
using Experiment.Core.Metadata;

namespace Experiment.FormulaCalculation.Service
{
	internal sealed class FormulaOperandValueResolver : IFormulaOperandValueResolver
	{
		readonly IExpSqlFactory _dbFactory;
		readonly ILogger _logger;

		public FormulaOperandValueResolver(IExpSqlFactory dbFactory, ILogger logger)
		{
			_dbFactory = dbFactory;
			_logger = logger;
		}

		public decimal Resolve(DataRow operand)
		{
			decimal result = 0;

			var vLoc = string.Format("{0}::{1}", FormulaCalculationLogContext.TypeName, nameof(Resolve));
			var vStep = "Start";

			var db = _dbFactory.Create();

			// This will prevent from service crash in unplanned cases
			try
			{
				if (operand[Defaults.DB_RELATED_DATAPOINT_ID] == DBNull.Value && operand[Defaults.DB_VALUE] != DBNull.Value)
				{
					result = decimal.Parse(operand[Defaults.DB_VALUE].ToString());
				}
				else if (operand[Defaults.DB_RELATED_DATAPOINT_ID] != DBNull.Value && operand[Defaults.DB_VALUE] == DBNull.Value)
				{
					DataTable datapointValue = db.GetLastDatapointValue(Int32.Parse(operand[Defaults.DB_RELATED_DATAPOINT_ID].ToString()));
					result = (decimal)datapointValue.Rows[0][Defaults.DB_VALUE];
				}
				else if (operand[Defaults.DB_RELATED_DATAPOINT_ID] == DBNull.Value && operand[Defaults.DB_VALUE] == DBNull.Value)
				{
					result = 0;
				}
			}
			catch (Exception ex)
			{
				var errorMsg = string.Format(
					"{0}, Failed at: {1}, with: {2},\r\n in: {3}",
					vLoc, vStep, ex.Message, ex.StackTrace);
				_logger.WriteLine(0, errorMsg);
			}

			return result;
		}
	}
}
