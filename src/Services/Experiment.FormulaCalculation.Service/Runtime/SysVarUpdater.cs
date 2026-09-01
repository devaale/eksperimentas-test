using System;

using Experiment.Core.BL.Data;
using Experiment.Core.BL.Data.SysVars;
using Experiment.Core.IO;
using Experiment.Core.Metadata;

namespace Experiment.FormulaCalculation.Service
{
	internal sealed class SysVarUpdater : ISysVarUpdater
	{
		readonly ExpSql _db;
		readonly ILogger _logger;

		public SysVarUpdater(ExpSql db, ILogger logger)
		{
			_db = db;
			_logger = logger;
		}

		public void UpdateSysVars()
		{
			var vLoc = string.Format("{0}::{1}", FormulaCalculationLogContext.TypeName, nameof(UpdateSysVars));

			try
			{
				var vars = _db.SysVarsGet(SysVarModule.Scan);

				if (vars.ContainsKey(SysVarName.SCAN_LOG_LEVEL))
				{
					_logger.LogLevel = Convert.ToInt32(vars[SysVarName.SCAN_LOG_LEVEL]);
				}
			}
			catch (Exception ex)
			{
				_logger.WriteLine(5, string.Format("{0}, {1}.", vLoc, ex.Message));
			}
		}
	}
}
