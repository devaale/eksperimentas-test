using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Core.BL.Data.SysVars{
	/// <summary>
	/// System variables (tblVars) names or keys
	/// 
	/// Enum names in this case are direct their names representation in DB.
	/// That's why used SNAKE naming style, which is used for those var names in tblVars table.
	/// </summary>
	public enum SysVarName
	{
		/// <summary>
		/// Owner company id, which owns EXP
		/// </summary>
		SYS_OWNER_ID,

		/// <summary>
		/// Scan service log level
		/// </summary>
		SCAN_LOG_LEVEL,
		/// <summary>
		/// Scan service log file location
		/// </summary>
		SCAN_LOG_LOCATION,
		/// <summary>
		/// Do scan service need to use dates in log file names (returns 0 or 1 as text)
		/// </summary>
		SCAN_LOG_USE_DATES,
		/// <summary>
		/// Scan service main loop delay
		/// </summary>
		SCAN_LOOP_DELAY,

		/// <summary>
		/// Parsing service log level
		/// </summary>
		PARSE_LOG_LEVEL,
		/// <summary>
		/// Parsing service log location
		/// </summary>
		PARSE_LOG_LOCATION,
		/// <summary>
		/// Do parsing service need to use dates in log file names (returns 0 or 1 as text)
		/// </summary>
		PARSE_LOG_USE_DATES,
		/// <summary>
		/// Parsing service main loop delay
		/// </summary>
		PARSE_LOOP_DELAY,

		//RomKuc to use with other modules also
		ALERTNOTIFICATION_LOG_LEVEL,
		ALERTNOTIFICATION_LOG_LOCATION,
		ALERTNOTIFICATION_LOG_USE_DATES,
		ALERTNOTIFICATION_LOOP_DELAY,

		MQTT_LOG_LEVEL,
	}
}
