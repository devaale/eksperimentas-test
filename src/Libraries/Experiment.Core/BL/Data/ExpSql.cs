#define WRITE_TO_DB

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

using Experiment.Core.BL.Data.SysVars;
using Experiment.Core.Data;
using Experiment.Core.Enums;
using Experiment.Core.Models;
using Experiment.Core.IO;
using Experiment.Core.Metadata;
using System.Collections.Specialized;

namespace Experiment.Core.BL.Data{
	public class ExpSql : Sql
	{
		#region Constants, eg. SQL queries
		private const string DEBUG_TYPE = "ExpSql";

		const string SQL_SCAN_BEGIN = "prcScanSessionBegin @_deviceId";
		const string SQL_SCAN_END = "prcScanSessionEnd @_scanSessionId, @_status";
		const string SQL_SCAN_STATUS_SET = "prcScanSessionStatusSet @_scanSessionId, @_status";

		const string SQL_SCAN_DATA_WRITE = "[prcScanDataWrite] @_scanSessionId, @_scanDataRangeId, @data";
		const string SQL_SCAN_DATA_IMPORT = "prcScanValueImport @_deviceId, @_systemId, @date, @value";
		const string SQL_SCAN_DATA_STATE_UPDATE = "prcScanSessionDataStateUpdate @_id, @_scanSessionDataStateId";
		//const string SQL_SCAN_VALUE_WRITE = "prcScanValueWrite @_scanSessionId, @_dataPointId, @value";
		//const string SQL_SCAN_VALUE_WRITE = "prcScanValueWrite @_dataPointId, @value";
		const string SQL_SCAN_VALUE_WRITE = "prcScanValueWrite @_deviceId, @_datapointId, @value";

		// API Scan
		const string SQL_SCAN_UPDATE_JSON = "prcApiDataUpdate @deviceId, @json";

		// @TODO check the following ones:
		const string SQL_SCAN_DATA_FOR_PARSING = "prcScanDataForParsing";
		const string SQL_DATAPOINT_SCAN_AND_PARSE_INFO = "prcDatapointScanAndParseInfo @_dataPointId";
		const string SQL_PHYSICAL_DATAPOINT_LIST_BY_DEVICE = "prcDatapointListByDeviceId @_deviceId";

		const string SQL_SYS_VAR_GET = "prcSysVarGet @name";
		const string SQL_SYS_VARS_GET = "prcSysVarsGet @module";

		//const string SQL_DATAPOINT_LIST = "prcDatapointList @_userId, @_customerId, @mode";
		const string SQL_OBJECT_ATTRIBUTE_LIST_ALL = "prcObjectAttributeListAll @_userId, @_customerId, @_deviceId, @mode, @lang";

		const string SQL_RPT_DATAPOINT_TABLE = "[prcDatapointChartTable] @datapointIds, @beginDate, @endDate, @aggregationType";
		const string SQL_RPT_DATAPOINT_CHARTS = "[prcReportDatapointCharts] @datapointIds, @beginDate, @endDate, @aggregationType";

		const string SQL_CREATE_ALERT_EVENT = "[prcCreateAlertEvent] @alertText, @alertGenerationId, @sessionId, @alertedValue, @setToOnOrOff";
		const string SQL_ALERT_EVENT_INFO = "[prcAlertEventInfo] @_userId, @_alertEventId";

		// formula
		const string SQL_FORMULA_DP_LIST = "prcFormulaDatapointsList";
		const string SQL_FORMULA_DP_LIST_FOR_RECALCULATION = "prcFormulaDatapointsListForRecalculation";
		const string SQL_FORMULA_CALC = "INSERT INTO [tblScanValues] ([_dataPointId], [value], [_scanSessionId]) VALUES ({0}, ({1}), NULL )";
		const string SQL_FORMULA_CALC_WITH_DATE = "INSERT INTO [tblScanValues] ([_dataPointId], [value], [date], [_scanSessionId]) VALUES ({0}, ({1}), '{2}', NULL)";
		const string SQL_FORMULA_CALC_DP_UPDATE = "prcDatapointFormulaUpdate @_id";

		// Import
		const string SQL_IMPORT_SOURCE_LIST = "prcImportSourceList @_activation";
		const string SQL_IMPORT_STREAM_LIST = "prcImportStreamList @_externalDataSourceId";
		const string SQL_IMPORT_DATAPOINT_LIST = "prcImportDatapointList @_externalDataStreamId";
		const string SQL_IMPORT_REGISTER_END = "prcImportRegisterEnd @_externalDataSourceId";

		// Import
		const int IMPORT_MAX_RECORDS_PER_REQUEST = 256; // Splitting of table in several requests, 0 - NO SPLITTING
		const int IMPORT_REST_MULTIPLIER = 4;

		// ENERSIS
		const string SQL_IMPORT_ENERSIS_DATAPOINT_VALUES = "prcImportEnersisDatapointValues";   // @_externalDataStreamId, @table
		const string SQL_IMPORT_DP_STATS_UPDATE = "prcImportDatapointStatUpdate";   // @_externalDataStreamId int, @table importDatapointStatData

		// Manual import
		const string SQL_IMPORT_DP_MANUAL_DATA = "prcImportDPManualData";   // @_userId, @_customerId, @table

		// UI
		const string SQL_UI_WORDS_ALL = "prcUiWordsAll";
		const string SQL_UI_WORD_REGISTER = "prcUiWordRegister @alias";

		// Mail
		const string SQL_SEND_MAIL = "prcSendMail";
		const string SQL_SEND_MAIL_LIST = "prcSendMailList";
		const string SQL_SEND_MAIL_STATE_UPDATE = "prcSendMailStateUpdate";
		const string SQL_USER_INFO_BY_OBJECT_ID = "prcUserInfoByObjectId @ObjectId";

		// ReportSub
		const string SQL_REPORTSUB_WRK = "prcReportSubSvcWork";

		// Control
		const string SQL_ALGORITHM_LIST = "prcAlgorithmList";
		const string SQL_ALGORITHM_STATUS_SET = "prcAlgorithmStatusSet @AlgorithmId, @Status";
		const string SQL_ALGORITHM_SNOOZE_NOTIFICATION_TILL_UPDATE = "prcAlgorithmSnoozeNotificationTillUpdate @AlgorithmId, @SnoozeNotificationTill";
		const string SQL_GROUP_LIST = "prcGroupList @ReadWrite";
		const string SQL_GROUP_DATAPOINTS_BY_GROUP_ID = "prcGroupDatapointsByGroupId @GroupId";
		const string SQL_DATAPOINT_LIST = "prcDatapointList @ReadWrite";
		const string SQL_DATAPOINT_LAST_VALUE = "prcLastDatapointValue @DatapointId";

		// Formula Calculation
		const string SQL_GET_VIRTUAL_DATAPOINTS = "prcGetVirtualDatapoints";
		const string SQL_GET_DATAPOINT_INFO_WITH_FORMULA_CHAIN = "prcGetDatapointInfoWithFormulaChain @DatapointId";
		const string SQL_GET_DATAPOINT_FORMULA = "prcGetDatapointFormula @DatapointId";
		const string SQL_CALC_FORMULA_VALUE_WRITE = "prcCalcFormulaValueWrite @_deviceId, @_dataPointId, @value";
		const string SQL_LAST_FORMULA_CALC_TIME_UPDATE = "prcLastFormulaCalcTimeUpdate @DatapointId";
		const string SQL_GET_FANGER_PMV_VALUE = "prcGetFangerPmvValue @DatapointId";
		const string SQL_CALCULATE_ENVIROMENTAL_IMPACT = "SELECT [dbo].[fncEnvironmentalImpact](@Param1, @Param2, @Param3)";
		const string SQL_CALCULATE_THERMAL_COMFORT = "SELECT [dbo].[fncThermalComfort](@Param1, @Param2, @Param3)";
		const string SQL_CALCULATE_DEPRECIATION = "prcCalcDepreciation @DeviceId";
		const string SQL_UPDATE_DEPRECIATION_A = "prcUpdateDeprA @DeviceId, @DatePart";
		const string SQL_VDT_FUNCTIONS = "SELECT [dbo].[fncVdpFunctions](@datapointId)";

		// EXPERIMENT
		// Users purge
		const string SQL_EXP_USER_PURGE = "prcUserPurge";

		#endregion

		#region Init
		public ExpSql(IDbConnection cn, ILogger logger)
			: base(cn, logger)
		{

		}

		#endregion

		#region Static
		/// <summary>
		/// Create ExpSql class instance with default parameters (eg. connection string)
		/// 
		/// Warning! If you get an exception in your library that missing SqlClient,
		/// that means that it missing not on this DLL, but on specific project, where you are using it
		/// </summary>
		/// <returns></returns>
		public static ExpSql GenerateFromDefaults(ILogger logger)
		{
			// Modified for easier debug
			if (logger == null)
				logger = new DebugLogger(1);

			var cnStr = Defaults.ConnectionString;
			var cn = new SqlConnection(cnStr);
			var retVal = new ExpSql(cn, logger);
			return retVal;
		}

		#endregion

		#region Scan and Parse Data

		/// <summary>
		/// New scan start
		/// </summary>
		/// <param name="deviceId">Device's which will be scanned id</param>
		/// <returns>_rawDataId aka tblRawData._id</returns>
		public string ScanSessionBegin(
			string deviceId)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_SCAN_BEGIN;
			AddParameter(cmd, "@_deviceId", deviceId);
			return QueryScalar(cmd);
		}

		/// <summary>
		/// Scan end
		/// </summary>
		/// <param name="scanSessionId">the string return value which was received from <b>RawDataStart</b></param>
		/// <param name="status">Status enumeration</param>
		/// <param name="data">binary data</param>
		/// <returns></returns>
		public bool ScanSessionEnd(
			string scanSessionId,
			ScanSessionStatus status)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_SCAN_END;
			AddParameter(cmd, "@_scanSessionId", scanSessionId);
			AddParameter(cmd, "@_status", (int)status);
			int affected = Execute(cmd);
			return !IsError && affected > 0;
		}

		public bool ScanDataWrite(
			string scanSessionId,
			string scanDataRangeId,
			byte[] data)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_SCAN_DATA_WRITE;
			AddParameter(cmd, "@_scanSessionId", scanSessionId);
			AddParameter(cmd, "@_scanDataRangeId", scanDataRangeId);
			AddParameter(cmd, "@data", data);
			int affected = Execute(cmd);
			return !IsError && affected > 0;
		}

		/// <summary>
		/// Updating of tblScanSessionData state
		/// </summary>
		/// <param name="scanSessionDataId">id of the record</param>
		/// <param name="state">new state value</param>
		/// <returns></returns>
		public bool ScanDataStateUpdate(
			string scanSessionDataId,
			ScanSessionDataStatus state)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_SCAN_DATA_STATE_UPDATE;
			AddParameter(cmd, "@_id", scanSessionDataId);
			AddParameter(cmd, "@_scanSessionDataStateId", (int)state);
			int affected = Execute(cmd);
			return !IsError && affected > 0;
		}

		/// <summary>
		/// Write parsed scan value, from parsed raw bytes array from tblScanSessionData table
		/// </summary>
		/// <param name="scanSessionId"></param>
		/// <param name="dataPointId"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool ParsedScanValueWrite(
			string scanSessionId,
			string dataPointId,
			decimal value)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_SCAN_VALUE_WRITE;

			AddParameter(cmd, "@_scanSessionId", int.Parse(scanSessionId));
			AddParameter(cmd, "@_dataPointId", int.Parse(dataPointId));
			AddParameter(cmd, "@value", value);

			DebugOut("prcScanValueWrite " + scanSessionId + ", " + dataPointId + ", " + value);

#if WRITE_TO_DB
			int affected = Execute(cmd);
#else
			int affected = 1;
#endif

			if (IsError)
			{
				DebugOut(cmd.CommandText);
			}
			return !IsError && affected > 0;
		}

		/// <summary>
		/// Write scaned value, from parsed raw bytes array from tblDatapoint table
		/// </summary>
		/// <param name="deviceId">
		/// int or string
		/// </param>
		/// <param name="datapointId">
		/// int or string</param>
		/// <param name="value">
		/// </param>
		/// <returns></returns>
		public bool ScanValueWrite(
			object deviceId,
			object datapointId,
			decimal value)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_SCAN_VALUE_WRITE;

			AddParameter(cmd, "@_deviceId", deviceId);
			AddParameter(cmd, "@_datapointId", datapointId);
			AddParameter(cmd, "@value", value);

			DebugOut($"prcScanValueWrite {datapointId}, {value}");

#if WRITE_TO_DB
			int affected = Execute(cmd);
#else
			int affected = 1;
#endif

			if (IsError)
			{
				DebugOut(cmd.CommandText);
			}
			return !IsError && affected > 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="scanSessionId"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		public bool ScanSessionStatusSet(
			string scanSessionId,
			ScanSessionStatus status)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_SCAN_STATUS_SET;

			AddParameter(cmd, "@_scanSessionId", int.Parse(scanSessionId));
			AddParameter(cmd, "@_status", (int)status);

			DebugOut("prcScanSessionStatusSet " + scanSessionId + ", " + status);
#if WRITE_TO_DB
			int affected = Execute(cmd);
#else
			int affected = 1;
#endif

			if (IsError)
			{
				DebugOut(cmd.CommandText);
			}
			return !IsError && affected > 0;
		}


		/// <summary>
		/// Import scan data value (parsed), creates as well scan session for it
		/// Used mostly for development or testing, but could be reused for data import if needed
		///
		/// Possibly could be required some changes if to use it for an import
		/// </summary>
		/// <param name="deviceId"></param>
		/// <param name="systemId"></param>
		/// <param name="date"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool ScanDataImport(string deviceId, string systemId, DateTime date, decimal value)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_SCAN_DATA_IMPORT;

			AddParameter(cmd, "@_deviceId", deviceId);
			AddParameter(cmd, "@_systemId", systemId);
			AddParameter(cmd, "@date", date);
			AddParameter(cmd, "@value", value);
#if WRITE_TO_DB
			int affected = Execute(cmd);
#else
			int affected = 1;
#endif

			if (IsError)
			{
				DebugOut(cmd.CommandText);
			}
			return !IsError && affected > 0;
		}

		/// <summary>
		/// Returns data for parsing service
		/// </summary>
		/// <returns></returns>
		public DataTable ScanDataForParsing()
		{
			return Query(SQL_SCAN_DATA_FOR_PARSING);
		}

		/// <summary>
		/// Get system variable from the database
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string SysVarGet(SysVarName var)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_SYS_VAR_GET;
			AddParameter(cmd, "@name", ExpSql.Magic(var.ToString()));

			return QueryScalar(cmd);
		}

		/// <summary>
		/// Get system variable from the database
		/// </summary>
		/// <param name="module">module name, IF NULL returns all variables</param>
		/// <returns></returns>
		public Dictionary<SysVarName, object> SysVarsGet(SysVarModule module)
		{
			Dictionary<SysVarName, object> retVal = new Dictionary<SysVarName, object>();

			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_SYS_VARS_GET;
			if (module == SysVarModule.Any)
				AddParameter(cmd, "@module", DBNull.Value);
			else
				AddParameter(cmd, "@module", module.ToString());

			DataTable table = Query(cmd);
			foreach (DataRow r in table.Rows)
			{
				SysVarName key;
				if (Enum.TryParse(r[Defaults.DB_NAME].ToString(), out key))
				{
					retVal.Add(key, r[Defaults.DB_VALUE]);
				}
			}

			return retVal;
		}

		public DataTable GetDataPointScanAndParseInfo(string dataPointId)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_DATAPOINT_SCAN_AND_PARSE_INFO;
			AddParameter(cmd, "@_dataPointId", dataPointId);
			return Query(cmd);
		}

		/// <summary>
		/// Returns datapoint list
		/// Used in formula defaults
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public DataTable DatapointList(string userId, string customerId)
		{
			return DatapointList(userId, customerId, DBNull.Value);
		}

		/// <summary>
		/// Returns datapoint list
		/// Used in formula defaults
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public DataTable DatapointList(string userId, string customerId, object mode)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_DATAPOINT_LIST;
			AddParameter(cmd, "@_userId", userId);
			AddParameter(cmd, "@_customerId", customerId);
			AddParameter(cmd, "@mode", mode);
			return Query(cmd);
		}

		public DataTable ObjectAttributeListAll(string userId, string customerId, string deviceId, string mode, string languageId)
		{
			DebugOut("ObjectAttributeListAll inside ");
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_OBJECT_ATTRIBUTE_LIST_ALL;
			AddParameter(cmd, "@_userId", userId);
			AddParameter(cmd, "@_customerId", customerId);
			AddParameter(cmd, "@_deviceId", deviceId);
			AddParameter(cmd, "@mode", mode);
			AddParameter(cmd, "@lang", languageId);
			return Query(cmd);
		}

		/// <summary>
		/// Untested, I hope that it works (A.G.)
		/// </summary>
		/// <param name="deviceId"></param>
		/// <returns></returns>
		public DataTable PhysicalDatapointListByDeviceId(int deviceId)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_PHYSICAL_DATAPOINT_LIST_BY_DEVICE;
			AddParameter(cmd, "@_deviceId", deviceId);
			return Query(cmd);
		}

		/// <summary>
		/// Save scanned device datapoints data in json format.
		/// Used in Api Scanner, which is part of modbus scanner, which scanns SuperHow AI service 
		/// </summary>
		/// <param name="deviceId"></param>
		/// <param name="json"></param>
		/// <returns></returns>
		public int ScanDataJsonUpdate(int deviceId, string json)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_SCAN_UPDATE_JSON; //prcApiDataUpdate @deviceId, @json

			AddParameter(cmd, "@deviceId", deviceId);
			AddParameter(cmd, "@json", json);
			return Execute(cmd);
		}

		#endregion

		#region Reports
		public DataTable ReportDatapoints(
			string datapointIds,
			string beginDate,
			string endDate,
			string aggregationType)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_RPT_DATAPOINT_TABLE;

			AddParameter(cmd, "@datapointIds", datapointIds);
			AddParameter(cmd, "@beginDate", beginDate);
			AddParameter(cmd, "@endDate", endDate);
			AddParameter(cmd, "@aggregationType", aggregationType);

			DataTable retVal = Query(cmd, "prcDatapointChartTable");    // This is datatable's name
			if (IsError)
			{
				DebugOut(cmd.CommandText);
			}
			return retVal;
		}

		public DataSet ReportDatapointCharts(
			string datapointIds,
			string beginDate,
			string endDate,
			string aggregationType)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_RPT_DATAPOINT_CHARTS;

			AddParameter(cmd, "@datapointIds", datapointIds);
			AddParameter(cmd, "@beginDate", beginDate);
			AddParameter(cmd, "@endDate", endDate);
			AddParameter(cmd, "@aggregationType", aggregationType);

			DataSet retVal = QueryDs(cmd);
			if (IsError)
			{
				DebugOut(cmd.CommandText);
			}

			return retVal;
		}


		#endregion

		#region Alerts

		public bool CreateAlertEvent(
			string alertText,
			int alertGenerationId,
			string sessionId = null,
			Nullable<decimal> alertedValue = null,
			bool setToOnOrOff = true
			)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_CREATE_ALERT_EVENT;

			AddParameter(cmd, "@alertText", ExpSql.Magic(alertText));
			AddParameter(cmd, "@alertGenerationId", alertGenerationId);
			AddParameter(cmd, "@sessionId", DbNullOrString(sessionId));
			AddParameter(cmd, "@alertedValue", DbNullOrValue(alertedValue));
			AddParameter(cmd, "@setToOnOrOff", setToOnOrOff);

			DebugOut("prcCreateAlertEvent  " + alertText);
#if WRITE_TO_DB
			int affected = Execute(cmd);
#else
            int affected = 1;
#endif
			if (IsError)
			{
				DebugOut(cmd.CommandText);
			}
			return !IsError && affected > 0;
		}

		public DataTable AlertEventInfo(
			string userId,
			string alertEventId
			)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_ALERT_EVENT_INFO;
			AddParameter(cmd, "@_userId", userId);
			AddParameter(cmd, "@_alertEventId", alertEventId);
			return Query(cmd);
		}

		#endregion

		#region Formula calculation

		/// <summary>
		/// Getting datapoints for calculation
		/// </summary>
		/// <returns></returns>
		public DataTable FormulaDatapointList()
		{
			return this.Query(SQL_FORMULA_DP_LIST);
		}

		public DataTable FormulaDatapointsListForRecalculation()
		{
			return this.Query(SQL_FORMULA_DP_LIST_FOR_RECALCULATION);
		}

		public bool FormulaCalculation(string datapointId, string formula, DateTime insertDatetime)
		{
			///RomKuc 2021-03-05 modification so during insert we specificly fix the date value (in case the calculation physicly will execute some time later (maybe next day)
			//Execute(string.Format(SQL_FORMULA_CALC, datapointId, formula, insertDatetime));
			Execute(string.Format(SQL_FORMULA_CALC_WITH_DATE, datapointId, formula, insertDatetime));
			return !IsError;
		}

		public DateTime FormulaCalculation(int datapointId, string formula, DateTime selectedDatetime, int interval, string intervalDatepart, DateTime recalculateToDatetime, bool dontCloseConnection)
		{
			ILogger logger = this.Logger;
			DateTime nextDateTime = DateTime.MinValue;
			DataTable testResult;
			DataTable nextDateTimeResult;
			DataTable insertSuccessfulResult;
			bool insertSuccessful = false;
			DataTable scanValueExistsResult;
			bool scanValueExists = false;
			String sql = "";
			//logger.WriteLine(5, string.Format("formula: {0}", formula));
			//formula = formula.Replace("fncDtp", "fncDtpWithDate");
			//logger.WriteLine(5, string.Format(SQL_FORMULA_CALC_WITH_DATE, datapointId, formula, selectedDatetime));
			Execute("sys.sp_set_session_context 'currentDateForCalculations', '" + selectedDatetime.ToString() + "';", dontCloseConnection);
			testResult = this.Query("select SESSION_CONTEXT(N'currentDateForCalculations') as currentDateForCalculations", dontCloseConnection);
			
			/*if (selectedDatetime > recalculateToDatetime)
            {
				Execute(string.Format("update [tblDataPointRecalculation] set [_active]=0 where [_id]={0} and [_active]=1", datapointId));
			}*/
			scanValueExistsResult = this.Query(string.Format("select _id from dbo.tblScanValues where [_dataPointId]={0} and [date]='{1}'", datapointId, selectedDatetime), dontCloseConnection);
			if (scanValueExistsResult != null)
			{
				if (scanValueExistsResult.Rows.Count > 0)
				{
					scanValueExists = (scanValueExistsResult.Rows[0]["_id"] == DBNull.Value ? false : true);
				}
			}
			/*if (scanValueExists)
			{
				Execute(string.Format("update [tblDataPointRecalculation] set [recalculatedUntillDatetime]='{0}' where [_id]={1} and [_active]=1", selectedDatetime, datapointId), dontCloseConnection);
			}*/
			//else
			if (!scanValueExists)
			{
				Execute(string.Format(SQL_FORMULA_CALC_WITH_DATE, datapointId, formula, selectedDatetime), dontCloseConnection);	
				insertSuccessfulResult = this.Query(string.Format("select _id from dbo.tblScanValues where [_dataPointId]={0} and [date]='{1}'", datapointId, selectedDatetime), dontCloseConnection);
				if (insertSuccessfulResult != null)
				{
					if (insertSuccessfulResult.Rows.Count > 0)
					{
						insertSuccessful = (insertSuccessfulResult.Rows[0]["_id"] == DBNull.Value ? false : true);
					}
				}
				else  ///RomKuc 20211014 in case some error appears during formula calculation
                {
					Execute(string.Format("update [tblDataPointRecalculation] set [_active]=0, _mdate=GETDATE(), _muserId=0  where [_id]={0} and [_active]=1", datapointId));
					return selectedDatetime;
				}
				//if (insertSuccessful)
				//	Execute(string.Format("update [tblDataPointRecalculation] set [recalculatedUntillDatetime]='{0}' where [_id]={1} and [_active]=1", selectedDatetime, datapointId), dontCloseConnection);
			}
			if (scanValueExists || insertSuccessful)
            {				
				Execute(string.Format("update [tblDataPointRecalculation] set [recalculatedUntillDatetime]='{0}' where [_id]={1} and [_active]=1", selectedDatetime, datapointId), dontCloseConnection);
			}
			nextDateTimeResult = this.Query(string.Format("select dbo.fncDATEADD('{0}', {1}, '{2}') as nextDateTime", intervalDatepart, interval, selectedDatetime), dontCloseConnection);
			if (nextDateTimeResult != null)
			{
				if (nextDateTimeResult.Rows.Count > 0)
				{
					nextDateTime = (nextDateTimeResult.Rows[0]["nextDateTime"] == DBNull.Value ? DateTime.MinValue : (DateTime)(nextDateTimeResult.Rows[0]["nextDateTime"]));
				}
			}
			if (nextDateTime > recalculateToDatetime)
			{
				Execute(string.Format("update [tblDataPointRecalculation] set [_active]=0, _mdate=GETDATE(), _muserId=0  where [_id]={0} and [_active]=1", datapointId));
				sql = string.Format("update [tblDataPoint] set [calculationReferenceDate]='{0}' where _id={1} AND [calculationReferenceDate]<'{2}'", selectedDatetime, datapointId, recalculateToDatetime);
				logger.WriteLine(5, string.Format("ExpSql.cs::FormulaCalculation, sql={0}", sql));
				Execute(sql);
			}

			if (insertSuccessful || scanValueExists)
				return nextDateTime;
			else
				return selectedDatetime;
		}

		public bool FormulaDpUpdate(string datapointId)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_FORMULA_CALC_DP_UPDATE;
			AddParameter(cmd, "@_id", datapointId);
			Execute(cmd);
			return !IsError;
		}

		#endregion

		#region Import

		/// <summary>
		/// External data sources list
		/// </summary>
		/// <returns></returns>
		public DataTable ImportSourceList(bool activation = false)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_IMPORT_SOURCE_LIST;
			AddParameter(cmd, "@_activation", activation);
			return Query(cmd);
		}

		/// <summary>
		/// External data stream list
		/// </summary>
		/// <param name="externalDataSourceId"></param>
		/// <returns></returns>
		public DataTable ImportStreamList(string externalDataSourceId)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_IMPORT_STREAM_LIST;
			AddParameter(cmd, "@_externalDataSourceId", externalDataSourceId);
			return Query(cmd);
		}

		/// <summary>
		/// Returns external datapoints list, regarding external data stream
		/// </summary>
		/// <param name="externalDataStreamId"></param>
		/// <returns></returns>
		public DataTable ImportDatapointList(string externalDataStreamId)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_IMPORT_DATAPOINT_LIST;
			AddParameter(cmd, "@_externalDataStreamId", externalDataStreamId);
			return Query(cmd);
		}

		/// <summary>
		/// Import ENERSIS datapoint data in bulk
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public int ImportEnerissBulkDatapointData(string externalDataStreamId, DataTable table)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_IMPORT_ENERSIS_DATAPOINT_VALUES;
			cmd.CommandType = CommandType.StoredProcedure;

			AddParameter(cmd, "@_externalDataStreamId", externalDataStreamId);
			AddParameter(cmd, "@table", table, SqlDbType.Structured);
			return Execute(cmd);
		}

		/// <summary>
		/// This method updates datapoint [lastAvailableDataTime] before import happened.
		/// 
		/// Later used to check how correctly works import, do it really works and so on.
		/// Every user can check it via UI after this, if he'll compare [lastAvailableDataTime] with [lastScanTime]
		/// </summary>
		/// <param name="externalDataStreamId"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		public int ImportDatapointStatUpdate(string externalDataStreamId, DataTable table)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_IMPORT_DP_STATS_UPDATE;
			cmd.CommandType = CommandType.StoredProcedure;

			AddParameter(cmd, "@_externalDataStreamId", externalDataStreamId);
			AddParameter(cmd, "@table", table, SqlDbType.Structured);
			return Execute(cmd);
		}

		/// <summary>
		/// Registers import end, updating [tblExternalDataSource].[lastScanDate] field.
		/// </summary>
		/// <param name="externalDataSourceId"></param>
		public void ImportRegisterEnd(string externalDataSourceId)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_IMPORT_REGISTER_END;
			AddParameter(cmd, "@_externalDataSourceId", externalDataSourceId);
			Execute(cmd);
		}

		public int ImportDPManualData(ILogger logger, string userId, string customerId, DataTable table)
		{
			var wLocation = string.Format("{0}:{1}", DEBUG_TYPE, "ImportDPManualData");
			int affected = 0;

			// Some code could be unrearchable at the moment, but constant values might change in future
			// Don't do anything with it!
			if(IMPORT_MAX_RECORDS_PER_REQUEST > 0)
			{
				int requestNo = 1;

				logger.WriteLine(5, string.Format("{0}, Splitting table in {1}|+1 different requests...",
					wLocation, table.Rows.Count / IMPORT_MAX_RECORDS_PER_REQUEST));

				foreach (DataTable splitTable in Utils.SplitDataTablePerRecords(table, IMPORT_MAX_RECORDS_PER_REQUEST))
				{
					IDbCommand cmd = Connection.CreateCommand();
					cmd.CommandText = SQL_IMPORT_DP_MANUAL_DATA;
					cmd.CommandType = CommandType.StoredProcedure;

					AddParameter(cmd, "@_userId", userId);
					AddParameter(cmd, "@_customerId", customerId);
					AddParameter(cmd, "@table", splitTable, SqlDbType.Structured);

					logger.WriteLine(5, string.Format("{0}, Executing request no: {1}...", wLocation, requestNo));
					affected += Execute(cmd);

					if (IsError)
					{
						logger.WriteLine(0, string.Format("{0}, Request {1} failed with: {2}, abandoning...", wLocation, requestNo, ErrorMsg));
						return affected;
					} 
					else
					{
						logger.WriteLine(5, string.Format("{0}, Request successful!", wLocation));
					}

					logger.WriteLine(5, string.Format("{0}, Sleeping for: {1}*{2}={3}ms...",
						wLocation,
						splitTable.Rows.Count,
						IMPORT_REST_MULTIPLIER,
						IMPORT_REST_MULTIPLIER * splitTable.Rows.Count));

					Thread.Sleep(IMPORT_REST_MULTIPLIER * splitTable.Rows.Count);
					requestNo++;
				}
			} else
			{
				IDbCommand cmd = Connection.CreateCommand();
				cmd.CommandText = SQL_IMPORT_DP_MANUAL_DATA;
				cmd.CommandType = CommandType.StoredProcedure;

				AddParameter(cmd, "@_userId", userId);
				AddParameter(cmd, "@_customerId", customerId);
				AddParameter(cmd, "@table", table, SqlDbType.Structured);

				logger.WriteLine(5, string.Format("{0}, Executing import...", wLocation));
				affected += Execute(cmd);

				if (IsError)
				{
					logger.WriteLine(0, string.Format("{0}, Failed with: {1}", wLocation, ErrorMsg));
					return affected;
				} else
				{
					logger.WriteLine(3, string.Format("{0}, It seems that import was sucessful!", wLocation));
				}
			}

			return affected;
		}

		#endregion

		#region Multilanguage
		public DataTable MultilanguageWordsAll()
		{
			return Query(SQL_UI_WORDS_ALL);
		}

		public void MultilanguageUpdateAlias(string alias)
		{
			Debug.WriteLine("ExpSql::MultilanguageUpdateAlias, Trying to add multilanguage alias [" + alias + "]");
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_UI_WORD_REGISTER;
			AddParameter(cmd, "@alias", alias);
			Execute(cmd);
		}

		#endregion

		#region Send Mail

		void sendMail(string to, string subject, string body, string from)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_SEND_MAIL;
			cmd.CommandType = CommandType.StoredProcedure;

			AddParameter(cmd, "@to", to);
			AddParameter(cmd, "@subject", subject);
			AddParameter(cmd, "@body", body);
			AddParameter(cmd, "@from", from);
			Execute(cmd);
		}

		public DataTable sendMailList(int state)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_SEND_MAIL_LIST;
			cmd.CommandType = CommandType.StoredProcedure;

			AddParameter(cmd, "@_state", state);
			return Query(cmd);
		}

		public bool sendMailStateUpdate(
			string sendMailId,
			SendMailState state)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_SEND_MAIL_STATE_UPDATE;
			cmd.CommandType = CommandType.StoredProcedure;

			AddParameter(cmd, "@_id", sendMailId);
			AddParameter(cmd, "@_state", (int)state);
			int affected = Execute(cmd);
			return !IsError && affected > 0;
		}

		#endregion

		#region ReportSub service
		public void ReportSubUpdate()
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_REPORTSUB_WRK;
			cmd.CommandType = CommandType.StoredProcedure;

			int affected = Execute(cmd);
		}

		#endregion

		#region Control

		/// <summary>
		/// Returns algorithm list
		/// </summary>
		/// <returns></returns>
		public DataTable AlgorithmList()
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_ALGORITHM_LIST;
			return Query(cmd);
		}

		/// <summary>
		/// Returns group list
		/// </summary>
		/// <param name="readWrite"></param>
		/// <returns></returns>
		public DataTable GroupList(int readWrite)
		{
			IDbCommand cmd = Connection.CreateCommand();
			AddParameter(cmd, "@ReadWrite", readWrite);
			cmd.CommandText = SQL_GROUP_LIST;
			return Query(cmd);
		}

		/// <summary>
		/// Returns datapoint list
		/// </summary>
		/// <param name="readWrite"></param>
		/// <returns></returns>
		public DataTable DatapointList(int readWrite)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_DATAPOINT_LIST;
			AddParameter(cmd, "@ReadWrite", readWrite);
			return Query(cmd);
		}

		public DataTable GetLastDatapointValue(int DataPointId)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_DATAPOINT_LAST_VALUE;
			AddParameter(cmd, "@DataPointId", DataPointId);
			return Query(cmd);
		}

		/// <summary>
		/// Returns group datapoint list
		/// </summary>
		/// <param name="groupId"></param>
		/// <returns></returns>
		public DataTable GetGroupDatapointsList(string GroupId)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_GROUP_DATAPOINTS_BY_GROUP_ID;
			AddParameter(cmd, "@GroupId", GroupId);
			return Query(cmd);
		}

		/// <summary>
		/// Returns user info by objectId
		/// </summary>
		/// <param name="objectId"></param>
		/// <returns></returns>
		public DataTable UserInfo(int objectId)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_USER_INFO_BY_OBJECT_ID;
			AddParameter(cmd, "@ObjectId", objectId);
			return Query(cmd);
		}

		/// <summary>
		/// Update Algorithm Status
		/// </summary>
		/// <param name="AlgorithmId">Status int</param>
		/// <param name="Status">Status decimal</param>
		/// <returns></returns>
		public bool AlgorithmStatusUpdate(
			int AlgorithmId,
			decimal Status)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_ALGORITHM_STATUS_SET;
			AddParameter(cmd, "@AlgorithmId", AlgorithmId);
			AddParameter(cmd, "@Status", (decimal)Status);
			int affected = Execute(cmd);
			return !IsError && affected > 0;
		}

		/// <summary>
		/// Update time of Snooze Notification Till
		/// </summary>
		/// <param name="AlgorithmId">AlgorithmId int</param>
		/// <param name="snoozeNotificationTill">SnoozeNotificationTill datetime</param>
		/// <returns></returns>
		public bool SnoozeNotificationTillTimeUpdate(
			int AlgorithmId, DateTime? snoozeNotificationTill)
		{
			object snoozeTill = DBNull.Value;
			if (snoozeNotificationTill.HasValue)
				snoozeTill = snoozeNotificationTill.Value;

			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_ALGORITHM_SNOOZE_NOTIFICATION_TILL_UPDATE;
			AddParameter(cmd, "@AlgorithmId", AlgorithmId);
			AddParameter(cmd, "@SnoozeNotificationTill", snoozeTill);
			int affected = Execute(cmd);
			return !IsError && affected > 0;
		}

		#endregion

		#region Formula Calculation

		/// <summary>
		/// @deprecated 
		/// 
		/// Returns virtual datapoints table
		/// </summary>
		/// <returns></returns>
		public DataTable GetVirtualDatapoints()
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_GET_VIRTUAL_DATAPOINTS;
			return Query(cmd);
		}

		/// <summary>
		/// Returns virtual datapoints table
		/// </summary>
		/// <returns></returns>
		public DataSet GetVirtualDatapointsDs()
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_GET_VIRTUAL_DATAPOINTS;
			return QueryDs(cmd);
		}

		/// <summary>
		/// @deprecated
		/// 
		/// Returns datapoint info with formula chain
		/// </summary>
		/// <param name="DatapointId">DatapointId int</param>
		/// <returns></returns>
		public DataTable GetDatapointInfoWithFormulaChain(string datapointId)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_GET_DATAPOINT_INFO_WITH_FORMULA_CHAIN;
			AddParameter(cmd, "@DatapointId", datapointId);
			return Query(cmd);
		}

		/// <summary>
		/// Returns datapoint formula
		/// </summary>
		/// <param name="DatapointId">DatapointId int</param>
		/// <returns></returns>
		public DataTable GetDatapointFormula(string datapointId)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_GET_DATAPOINT_FORMULA;
			AddParameter(cmd, "@DatapointId", datapointId);
			return Query(cmd);
		}

		/// <summary>
		/// Write formula calculated value to database
		/// </summary>
		/// <param name="deviceId"></param>
		/// <param name="dataPointId"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool CalcFormulaValueWrite(
			string deviceId,
			string dataPointId,
			decimal value)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_CALC_FORMULA_VALUE_WRITE;

			AddParameter(cmd, "@_deviceId", int.Parse(deviceId));
			AddParameter(cmd, "@_dataPointId", int.Parse(dataPointId));
			AddParameter(cmd, "@value", value);

			DebugOut("prcCalcFormulaValueWrite " + deviceId+ ", " + dataPointId + ", " + value);

#if WRITE_TO_DB
			int affected = Execute(cmd);
#else
			int affected = 1;
#endif

			if (IsError)
			{
				DebugOut(cmd.CommandText);
			}
			return !IsError && affected > 0;
		}

		/// <summary>
		/// Update last formula calc time
		/// </summary>
		/// <param name="Status">Status decimal</param>
		/// <returns></returns>
		public bool LastFormulaCalcTimeUpdate(
			int datapointId)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_LAST_FORMULA_CALC_TIME_UPDATE;
			AddParameter(cmd, "@DatapointId", datapointId);
			int affected = Execute(cmd);
			return !IsError && affected > 0;
		}

		/// <summary>
		/// Get FangerPMV value
		/// </summary>
		/// <param name="DatapointId">DatapointId int</param>
		/// <returns></returns>
		public DataTable GetFangerPmvValue(int DatapointId)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_GET_FANGER_PMV_VALUE;
			AddParameter(cmd, "@DatapointId", DatapointId);
			return Query(cmd);
		}

		/// <summary>
		/// Calculate Enviromental Impact
		/// </summary>
		/// <param name="Param1">Param1 decimal</param>
		/// <param name="Param2">Param2 decimal</param>
		/// <param name="Param3">Param3 decimal</param>
		/// <returns></returns>
		public string CalculateEnvironmentalImpact(decimal Param1, decimal Param2, decimal Param3)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_CALCULATE_ENVIROMENTAL_IMPACT;
			AddParameter(cmd, "@Param1", Param1);
			AddParameter(cmd, "@Param2", Param2);
			AddParameter(cmd, "@Param3", Param3);
			return QueryScalar(cmd);
		}

		/// <summary>
		/// Calculate Thermal Comfort
		/// </summary>
		/// <param name="Param1">Param1 decimal</param>
		/// <param name="Param2">Param2 decimal</param>
		/// <param name="Param3">Param3 decimal</param>
		/// <returns></returns>
		public string CalculateThermalComfort(decimal Param1, decimal Param2, decimal Param3)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_CALCULATE_THERMAL_COMFORT;
			AddParameter(cmd, "@Param1", Param1);
			AddParameter(cmd, "@Param2", Param2);
			AddParameter(cmd, "@Param3", Param3);
			return QueryScalar(cmd);
		}

		/// <summary>
		/// Calculate Depreciation
		/// </summary>
		/// <param name="DeviceId">Device Id</param>
		/// <returns></returns>
		public string CalculateDepreciation(int DeviceId)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_CALCULATE_DEPRECIATION;
			AddParameter(cmd, "@DeviceId", DeviceId);
			return QueryScalar(cmd);
		}

		/// <summary>
		/// Update depreciation device age
		/// </summary>
		/// <param name="DeviceId">Device Id</param>
		/// <param name="DatePart">Date part</param>
		/// <returns></returns>
		public DataTable UpdateDeprA(int DeviceId, string DatePart)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_UPDATE_DEPRECIATION_A;
			AddParameter(cmd, "@DeviceId", DeviceId);
			AddParameter(cmd, "@DatePart", DatePart);
			return Query(cmd);
		}

		/// <summary>
		/// Virtual datapoint functions
		/// </summary>
		/// <param name="DateTime">Date and Time</param>
		/// <param name="DatePart">Date part</param>
		/// <param name="IntervalDatePart">IntervalDatePart</param>
		/// <returns></returns>
		public string VdpFunctions(int datapointId)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_VDT_FUNCTIONS;
			AddParameter(cmd, "@datapointId", datapointId);
			return QueryScalar(cmd);
		}

		#endregion

		#region Experiment: Purge user and its data
		/// <summary>
		/// Using PurgeUserService
		/// </summary>
		public int ExpPurgeUsersData()
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_EXP_USER_PURGE;
			return Execute(cmd);
		}

		#endregion

		#region MQTT

		/// <summary>
		/// Returns recordset data for MQTT service
		/// [prcMqttServiceData]
		/// 
		/// [ ] Updated
		/// </summary>
		/// <returns></returns>
		public DataSet MqttServiceData()
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = "[prcMqttServiceData]";
			return QueryDs(cmd);
		}

		/// <summary>
		/// Saves the specific MQTT topic value
		/// [prcMqttValueSave] @_deviceId, @topic, @value, @date
		/// 
		/// [X] Updated 2024-07-10
		/// </summary>
		/// <param name="deviceId"></param>
		/// <param name="topic"></param>
		/// <param name="value"></param>
		/// <param name="date"></param>
		public void MqttValueSave(
			int deviceId,
			string topic,
			string path,
			decimal? value,
			DateTime? date)
		{
			object parsedTopic = DBNull.Value;
			if (!string.IsNullOrEmpty(topic))
				parsedTopic = topic;

			object parsedPath = DBNull.Value;
			if (!string.IsNullOrEmpty(path))
				parsedPath = path;

			object parsedValue = DBNull.Value;
			if (value.HasValue)
				parsedValue = value.Value;

			object parsedDate = DBNull.Value;
			if (date.HasValue)
				parsedDate = date.Value;

			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = "[prcMqttValueSave] @_deviceId, @topic, @path, @value, @date";
			AddParameter(cmd, "@_deviceId", deviceId);
			AddParameter(cmd, "@topic", parsedTopic);
			AddParameter(cmd, "@path", parsedPath);
			AddParameter(cmd, "@value", parsedValue);
			AddParameter(cmd, "@date", parsedDate);
			Execute(cmd);
		}

		/// <summary>
		/// Returns still not processed MQTT messages (tblMqttMessage)
		/// 
		/// [X] Updated 2024-07-09
		/// </summary>
		/// <returns></returns>
		public List<MessageMqtt> MqttMessagesUnprocessed()
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = "[prcMqttMessagesUnprocessed]";
			var table = Query(cmd);
			var retVal = (from row in table.AsEnumerable()
						  select new MessageMqtt()
						  {
							  Id = row.Field<int>(nameof(MessageMqtt.Id)),
							  DeviceId = row.Field<int>(nameof(MessageMqtt.DeviceId)),
							  DeviceTopicId = row.Field<int>(nameof(MessageMqtt.DeviceTopicId)),
							  Topic = row.Field<string>(nameof(MessageMqtt.Topic)),
							  Payload = row.Field<string>(nameof(MessageMqtt.Payload)),
						  }).ToList();
			return retVal;
		}

		/// <summary>
		/// Sets specific MQTT message (tblMqttMessage) as processed with specific state
		/// 
		/// [X] Updated 2024-07-09
		/// </summary>
		/// <param name="mqttMessageId"></param>
		/// <param name="state"></param>
		public void MqttMessageProcessed(int mqttMessageId, MqttMessageState state)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = "[prcMqttMessageProcessed] @id, @state";
			AddParameter(cmd, "@id", mqttMessageId);
			AddParameter(cmd, "@state", state);
			Execute(cmd);
		}

		/// <summary>
		/// Informs about received any MQTT message from device's broker
		/// 
		/// [X] Updated 2024-07-09
		/// </summary>
		/// <param name="url"></param>
		/// <param name="topic"></param>
		/// <param name="payload"></param>
		public void MqttMessageReceived(string url, string topic, string payload)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = "[prcMqttMessageReceived] @url, @topic, @payload";
			AddParameter(cmd, "@url", url);
			AddParameter(cmd, "@topic", topic);
			AddParameter(cmd, "@payload", payload);
			Execute(cmd);
		}

		/// <summary>
		/// [prcDeviceLastScanUpdate] @DeviceId, @Date
		/// 
		/// [x] Updated 2024-07-09
		/// </summary>
		/// <param name="deviceId"></param>
		/// <param name="date"></param>
		public void DeviceLastScanUpdate(int deviceId, DateTime? date)
		{
			object parsedDate = DBNull.Value;
			if (date.HasValue)
				parsedDate = date.Value;

			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = "[prcDeviceLastScanUpdate] @deviceId, @date";
			AddParameter(cmd, "@deviceId", deviceId);
			AddParameter(cmd, "@date", parsedDate);
			Execute(cmd);
		}
		#endregion



		#region Static
		static object DbNullOrString(string str)
		{
			object retVal = DBNull.Value;					   
			if (!string.IsNullOrEmpty(str))
			{
				retVal = str;
			}
			return retVal;
		}

		
		static object DbNullOrValue<T>(Nullable<T> param) where T : struct
		{
			object retVal = DBNull.Value;
			if (param.HasValue)
			{
				retVal = param.Value;
			}
			return retVal;
		}

		#endregion


	}
}

