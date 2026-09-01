using Experiment.Core.BL.Data;
using Experiment.Core.IO;
using Experiment.Core.Metadata;

namespace Experiment.FormulaCalculation.Service
{
	internal sealed class ExpSqlFactory : IExpSqlFactory
	{
		readonly ILogger _logger;

		public ExpSqlFactory(ILogger logger)
		{
			_logger = logger;
		}

		public ExpSql Create()
		{
			return ExpSql.GenerateFromDefaults(_logger);
		}
	}
}
