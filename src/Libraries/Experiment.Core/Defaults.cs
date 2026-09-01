#if DEBUG

// Add own precompiler defs for work
#define DEV_ARVYDAS
//#define DEV_ROMAN
//#define DEV_DMITRIJUS

#else

#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using Experiment.Core.Config;

namespace Experiment.Core{
	public class Defaults
	{
		#region Constants
		public const string DEFAULT_CULTURE = "lt-LT";

		/// <summary>
		/// Default language. Affects Xamarin Mobile App and Web Backend.
		/// </summary>
		public const string DEFAULT_LANGUAGE = "en";

		// Currency signs
		public const string CURRENCY_EUR_SIGN = "€";

		// Dates
		public const string DEFAULT_DATE_FORMAT = "yyyy-MM-dd";
		public const string DEFAULT_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
		public const string DEFAULT_DATETIME_FORMAT_FILE = "yyyy-MM-dd_HH-mm-ss";

		public const char FIELD_SEPARATOR = '|';
		public const bool IMAGE_CACHING = false;

		public const int DEFAULT_MQTT_PORT = 1883;

		// Graphs

		/// <summary>
		/// What is max length of datapoint name before trimming, eg. in case of datapoint comparisons
		/// </summary>
		public const int MAX_GRAPH_COMP_DP_NAME_LEN = 8;
		/// <summary>
		/// @Martynas told that 3 is max
		/// </summary>
		public const int MAX_GRAPH_COMP_YEARS = 3;
		public const int USER_ACCOUNT_DELETION_IN_DAYS = 30;

		/// <summary>
		/// Minimal length of username
		/// </summary>
		public const int MIN_USERNAME_LENGTH = 3;
		public const int MIN_PASSWORD_LENGTH = 6;

		public const string URL_ROOT = "/exp/";
		public const string URL_API_ADDON = "api/";
		public const string URL_API_ROOT = URL_ROOT + URL_API_ADDON;

		public const string URL_API_DEVICE = URL_API_ROOT + "Device";
		public const string URL_API_DATAPOINT = URL_API_ROOT + "Datapoint";

		// Paysera integration BASE64 encoded that was hard to find it via HEX editor
		// Paysera project Id: 235095
		private const string _PAYSERA_PROJECT_ID_B64 = "MjM1MDk1";
		// Paysera sign password: cb12ca193d39b38f33f7d09a2ce6d080
		private const string _PAYSERA_SIGN_PASSWORD_B64 = "Y2IxMmNhMTkzZDM5YjM4ZjMzZjdkMDlhMmNlNmQwODA=";
		public static int PAYSERA_PROJECT_ID { get => Convert.ToInt32(Encoding.UTF8.GetString(Convert.FromBase64String(_PAYSERA_PROJECT_ID_B64))); }
		public static string PAYSERA_SIGN_PASSWORD { get => Encoding.UTF8.GetString(Convert.FromBase64String(_PAYSERA_SIGN_PASSWORD_B64)); }


		// Back-end (ASP.net)

		/// <summary>
		/// Used generating URLs from back-end, eg. posts pictures of Ecosystem (Main menu)
		/// @deprecated
		/// </summary>
		//public const string BACKEND_URL_HOME = "http://terra" + URL_ROOT;

#warning @TODO: What is this? Deprecated?
		public const string NAME_DATE_FROM = "dateFrom";
		public const string NAME_DATE_TO = "dateTo";
		public const string NAME_DATE_IDS = "ids";
		public const string NAME_AGGREGATION_VARIANT = "aggregationVariant";
		public const string NAME_MEASURE_UNIT = "measureUnit";
		public const string NAME_CHART_TYPE = "chartType";


		public const string ENV_EXP_HOME = "EXP_HOME";

		public const string DEFAULT_CONFIG_PATH = @"\src\config\config.json";
		public const string DEFAULT_CONNNECTION_STRING_PATTERN = @"Server={0};Database={1};User Id={2};Password={3}";
		protected const bool DEBUG = true;

		public const string DEFAULT_LOG_FOLDER = @"C:\temp\exp_logs\";
		public const int DEFAULT_LOG_LEVEL = 6;

		// MQTT
		public const string DB_CLIENT_ID = "clientId";
		public const string DB_TOPIC = "topic";
		public const string DB_FIELD = "field";
		public const string DB_PATH = "path";
		public const string DB_PAYLOAD = "payload";
		public const string DB_DEVICE_TOPIC_ID = "_deviceTopicId";
		// Devices
		//public const string DB_DEVICE_ID = "_deviceId";
		public const string DB_DEVICE_ID = "DeviceId";
		public const string DB_SYSTEM_ID = "_systemId";
		public const string DB_SCAN_SESSION_ID = "_scanSessionId";
		public const string DB_CUSTOMER_ID = "_customerId";

		public const string DB_DATE = "date";
		//public const string DB_INTERVAL = "interval";
		public const string DB_INTERVAL = "Interval";
		public const string DB_DATA_START = "dataStart";
		public const string DB_DATA_LENGTH = "dataLength";
		//public const string DB_DATA_END = "dataEnd";
		public const string DB_BITSIZE = "bitsize";
		//public const string DB_REGISTER_ADDRESS = "registerAddress";
		public const string DB_REGISTER_ADDRESS = "RegisterAddress";
		//public const string DB_FUNCTION_CODE = "functionCode";
		public const string DB_FUNCTION_CODE = "FunctionCode";
		public const string DB_WRITE_FUNCTION_CODE = "writeFunctionCode";
		//public const string DB_REGISTER_TYPE = "registerType";
		public const string DB_REGISTER_TYPE = "RegisterType";
		//public const string DB_MULTIPLIER = "multiplier";
		public const string DB_MULTIPLIER = "Multiplier";
		//public const string DB_OFFSET = "offset";
		public const string DB_OFFSET = "Offset";
		public const string DB_BIT_POSITION = "bitPosition";
		public const string DB_DATA = "data";
		//public const string DB_UNIT_ID = "unitId";
		public const string DB_UNIT_ID = "UnitId";
		public const string DB_READ_WRITE = "ReadWrite";

		public const string DB_HOST = "host";
		public const string DB_DATABASE = "db";
		public const string DB_USERNAME = "username";
		public const string DB_PASSWORD = "password";
		public const string DB_OPTIONS = "options";

		public const string DB_EXTERNAL_DATA_SOURCE_ID = "_externalDataSourceId";
		public const string DB_EXTERNAL_DATA_STREAM_ID = "_externalDataStreamId";
		public const string DB_EXTERNAL_DATAPOINT_ID = "_externalDatapointId";

		public const string DB_DP_VIRTUAL = "_virtual";
		public const string DB_DP_FORMULA = "datapointFormula";
		public const string DB_DP_INTERVAL = "interval";
		public const string DB_DP_LAST_ACTIVATION_TIME = "lastActivationTime";
		public const string DB_DP_LAST_SCAN_TIME = "lastScanTime";
		public const string DB_DP_PROJECTED_SCAN_TIME = "projectedScanTime";

		// Essential db tables fields
		public const string DB_ID = "Id";
		//public const string DB_NAME = "name";
		public const string DB_NAME = "Name";
		public const string DB_CODE = "code";    // Used in few import tables, at least, as well as in JSON service return structure
		//public const string DB_VALUE = "value";
		public const string DB_VALUE = "Value";
		public const string DB_TEXT = "text";
		//public const string DB_DESC = "description";
		public const string DB_DESC = "Description";
		public const string DB_ALIAS = "alias";
		public const string DB_LANGUAGE_ID = "languageId";

		// Common credentials
		//public const string DB_EMAIL = "email";
		public const string DB_EMAIL = "Email";
		public const string DB_ADDRESS = "address";
		public const string DB_PHONE = "phone";
		//public const string DB_URL = "url";
		public const string DB_URL = "Url";

		public const string DB_LAST_AVAILABLE_DATA_TIME = "lastAvailableDataTime";
		public const string DB_LAST_SCAN_TIME = "lastScanTime";
		public const string DB_PROJECTED_SCAN_TIME = "projectedScanTime";
		public const string DB_PROTOCOL = "Protocol";

		// Send Mail
		public const string DB_TO = "To";
		public const string DB_SUBJECT = "Subject";
		public const string DB_BODY = "Body";
		public const string DB_FROM = "From";

		// Algorithm and Alarm
		public const string DB_DATE_START = "DateStart";
		public const string DB_DATE_END = "DateEnd";
		public const string DB_TIME_START = "TimeStart";
		public const string DB_TIME_END = "TimeEnd";
		public const string DB_STATUS = "Status";
		public const string DB_GROUP_ID = "GroupId";
		public const string DB_DATAPOINT_ID = "DatapointId";
		public const string DB_VALUE_OFF = "ValueOff";
		public const string DB_VALUE_ON = "ValueOn";

		public const string DB_ON_MONDAY = "OnMonday";
		public const string DB_ON_TUESDAY = "OnTuesday";
		public const string DB_ON_WEDNESDAY = "OnWednesday";
		public const string DB_ON_THURSDAY = "OnThursday";
		public const string DB_ON_FRIDAY = "OnFriday";
		public const string DB_ON_SATURDAY = "OnSaturday";
		public const string DB_ON_SUNDAY = "OnSunday";

		public const string DB_VALUE_FROM = "ValueFrom";
		public const string DB_VALUE_TO = "ValueTo";
		public const string DB_ALARM_ID = "AlarmId";
		public const string DB_OBJECT_ID = "ObjectId";

		public const string DB_REMINDER_AFTER_HOURS = "ReminderAfterHours";
		public const string DB_SNOOZE_NOTIFICATION_TILL = "SnoozeNotificationTill";

		// Formula Calculation
		public const string DB_INTERVAL_DATE_PART = "IntervalDatepart";
		public const string DB_LAST_FORMULA_CALC_TIME = "LastFormulaCalcTime";
		public const string DB_DATAPOINT_FORMULA_ID = "DatapointFormulaId";
		public const string DB_RELATED_DATAPOINT_ID = "RelatedDatapointId";
		public const string DB_AGGREGATION_DATE_PART = "AggregationDatepart";
		
		/// <summary>
		/// Http META header for .txt files
		/// </summary>
		public const string HTTP_HEADER_TXT = "text/plain";

		// ML ALIASES
		public const string MA_APP_NAME = "app-name";

		#endregion

		protected static string _ExpHome = string.Empty;
		public static string ExpHome
		{
			get
			{
				if (string.IsNullOrEmpty(_ExpHome))
				{
					_ExpHome = Environment.GetEnvironmentVariable(ENV_EXP_HOME);
				}
				return _ExpHome;
			}
		}

		protected static string _ConnectionString = String.Empty;
		public static string ConnectionString
		{
			get
			{

				if (String.IsNullOrEmpty(_ConnectionString))
				{
					InitSettings();
				}
				return _ConnectionString;
			}
		}

		static void InitSettings()
		{
			if (string.IsNullOrEmpty(ExpHome))
				throw new Exception("EXP_HOME was not found!");

			var fullConfigPath = ExpHome + DEFAULT_CONFIG_PATH;
			LoadAndParseSettings(fullConfigPath);
		}

		static void LoadAndParseSettings(string path)
		{
			using (StreamReader file = File.OpenText(path))
			{
				JsonSerializer serializer = new JsonSerializer();
				ConfigObject o = (ConfigObject)serializer.Deserialize(file, typeof(ConfigObject));

				_ConnectionString = String.Format(
					DEFAULT_CONNNECTION_STRING_PATTERN,
					o.Settings.Host,
					o.Settings.Database,
					o.Settings.Username,
					o.Settings.Password);
				file.Close();
			}
		}
	}
}
