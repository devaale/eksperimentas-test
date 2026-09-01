using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Core.BL.Data.SysVars{
	public enum SysVarModule
	{
		Any,
		Parsing,
		Scan,
		AlertNotification,
		FormulaCalculation,
		DataImport,
		ReportSub,
		SendMail,
	}
}
