#define SEND_MAIL // Enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Experiment.Modbus;

using Experiment.Core;
using Experiment.Core.BL.Data;
using Experiment.Core.BL.Data.SysVars;
using Experiment.Core.IO;
using Experiment.Core.Metadata;
using Experiment.Core.Web;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;
using Experiment.Data.Models;

using Experiment.Control.Service.Data;

namespace Experiment.Control.Service{
	partial class ControlService : ServiceBase
	{
		#region Constants
		/// <summary>
		/// Debug
		/// </summary>
		const string TYPE_NAME = nameof(ControlService);

		/// <summary>
		/// Default log level
		/// </summary>
		public const int DEFAULT_LOG_LEVEL = 5;

		/// <summary>
		/// Sleep time by minutes
		/// </summary>
		const int SLEEP_TIME_MINS = 1;
		/// <summary>
		/// How many ms to sleep in total
		/// </summary>
		const int SLEEP_TIME = SLEEP_TIME_MINS * 60 * 1000;

		/// <summary>
		/// Single sleep moment, between which we will check do service started
		/// </summary>
		const int SLEEP_SINGLE = 500;

		/// <summary>
		/// Amount of single sleep times at the end of the single heartbeat
		/// </summary>
		const int SLEEP_TILES = SLEEP_TIME / SLEEP_SINGLE;

		#endregion

		#region Attributes
		static bool _ServiceStarted;

		static ILogger _Logger;
		static EventLog _EventLog;
		static Mail _Mail;

		Thread _Thread;

		#endregion

		#region Properties
		internal static bool ConsoleMode
		{
			get { return Environment.UserInteractive; }
		}

		#endregion

		#region Ctor

		public ControlService()
		{
			InitializeComponent();

			_ServiceStarted = false;
			if (ConsoleMode)
			{
				_Logger = new ConsoleLogger(
					DEFAULT_LOG_LEVEL,
					Program.ServiceName);
			}
			else
			{
				_Logger = new FileLogger(
					DEFAULT_LOG_LEVEL,
					Defaults.DEFAULT_LOG_FOLDER,
				Program.ServiceName);
			}

			InitEventLog();
			//_Db = ExpSql.GenerateFromDefaults(_Logger);
			_Mail = new Mail(_Logger);
		}

		#endregion

		#region Helpers
		void InitEventLog()
		{
			try
			{
				_EventLog = new EventLog();// "Application");
				_EventLog.Source = Program.ServiceName;

			}
			catch (Exception ex)
			{
				_Logger.WriteLine(0, Program.ServiceName + ": Initializing of event log went wrong: " + ex.Message);
			}
		}

		static void WriteToEventLog(string msg)
		{
			if (_EventLog != null)
			{
				_EventLog.WriteEntry(msg);
			}
		}

		void UpdateSysVars(ExpSql db)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(UpdateSysVars));

			try
			{
				var _Vars = db.SysVarsGet(SysVarModule.Scan);

				if (_Vars.ContainsKey(SysVarName.SCAN_LOG_LEVEL))
				{
					_Logger.LogLevel = Convert.ToInt32(_Vars[SysVarName.SCAN_LOG_LEVEL]);
				}
			}
			catch (Exception ex)
			{
				_Logger.WriteLine(5, string.Format("{0}, {1}.", vLoc, ex.Message));
			}
		}
		#endregion

		#region Methods
		protected override void OnStart(string[] args = null)
		{
			var vLoc = string.Format("{0}::{1}", Program.ServiceName, nameof(OnStart));
			_Logger.WriteLine(3, vLoc);
			WriteToEventLog(vLoc);

			_ServiceStarted = true;

			_Thread = new Thread(Heartbeat);
			_Thread.Start();
		}

		protected override void OnStop()
		{
			var vLoc = string.Format("{0}::{1}", Program.ServiceName, nameof(OnStop));
			_Logger.WriteLine(3, vLoc);
			WriteToEventLog(vLoc);

			_ServiceStarted = false;
		}

		internal void Start()
		{
			OnStart();
		}

		#endregion

		#region Service logic
		internal void Heartbeat()
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(Heartbeat));
			var vStep = "Start";
			_Logger.WriteLine(4, string.Format("{0}, {1}", vLoc, vStep));

			while (_ServiceStarted)
			{
				vStep = "While";
				_Logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));

				var sleepCount = SLEEP_TILES;

				// This will prevent from service crash in unplanned cases
				try
				{
					vStep = "Generate DB defaults..";
					_Logger.WriteLine(5, string.Format("{0}, {1}..", vLoc, vStep));
					var db = ExpSql.GenerateFromDefaults(_Logger);

					vStep = "Updating sys vars..";
					_Logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));
					UpdateSysVars(db);

					vStep = "Reading algorithm list..";
					_Logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));
					var algorithmTable = db.AlgorithmList();

					vStep = "Reading group list..";
					_Logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));
					var groupTable = db.GroupList(1);

					vStep = "Reading datapoint list..";
					_Logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));
					var datapointTable = db.DatapointList(1);

					//if (!db.IsError && algorithmTable.Columns.Contains(Defaults.DB_ID))
					if (!db.IsError && algorithmTable.Columns.Contains(nameof(IAlgorithm.Id)))
					{
						// Run algorithm
						ParseAlgorithm(algorithmTable, groupTable, datapointTable);
					}
					else
					{
						//if (!algorithmTable.Columns.Contains(Defaults.DB_ID))
						if (!algorithmTable.Columns.Contains(nameof(IAlgorithm.Id)))
						{
							_Logger.WriteLine(0, string.Format("{0}, Returned data sources table has no data!", vLoc));
						}
					}
				}
				catch (Exception ex)
				{
					var errorMsg = string.Format(
						"{0}, Failed at: {1}, with: {2},\r\n in: {3}",
						vLoc, vStep, ex.Message, ex.StackTrace);
					_Logger.WriteLine(0, string.Format("{0}, {1}", vLoc, errorMsg));
				}


				_Logger.WriteLine(5, string.Format("{0}, Sleeping...", vLoc));
				while (--sleepCount > 0)
				{
					Thread.Sleep(SLEEP_SINGLE);

					if (!_ServiceStarted)
						break;
				}
			}
		}

		static void ParseAlgorithm(DataTable algorithmTable, DataTable groupTable, DataTable datapointTable)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(ParseAlgorithm));
			_Logger.WriteLine(5, string.Format("{0}..", vLoc));

			foreach (DataRow row in algorithmTable.Rows)
			{
				int algorithmId = (int)row[nameof(IAlgorithm.Id)];
				var algorithmName = row[nameof(IAlgorithm.Name)].ToString();

				//var wProcessingAlgorithmsStr = string.Format("Algorithm: {0}, Name: {1}",
				//	row[Defaults.DB_ID].ToString(),
				//	row[Defaults.DB_NAME].ToString());

				_Logger.WriteLine(5, string.Format("{0} => {1} for {2}",
					vLoc, algorithmId, algorithmName));

				RunAlgorithm(row, algorithmTable);
			}
		}

		static void RunAlgorithm(DataRow algorithm, DataTable algorithmTable)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(RunAlgorithm));

			int algorithmId = (int)algorithm[nameof(IAlgorithm.Id)];
			AlgorithmType type = (AlgorithmType)algorithm[nameof(IAlgorithm.Type)];

			_Logger.WriteLine(5, string.Format("{0} => {1} => {2}", vLoc, algorithmId, type));

			//switch (algorithm["Type"])
			switch (type)
			{
				//case 10:
				case AlgorithmType.TimeTrigger:
					TimeTrigger(algorithm);
					break;

				//case 20:
				case AlgorithmType.PeriodicTimeTrigger:
					PeriodicTimeTrigger(algorithm);
					break;

				//case 30:
				case AlgorithmType.Alarm:
					Alarm(algorithm);
					break;

				//case 40:
				case AlgorithmType.AlarmTrigger:
					AlarmTrigger(algorithm, algorithmTable);
					break;

				default:
					break;
			}
		}

		/// <summary>
		/// During exact date (from - to) and time (from - to) sets specified On state. 
		/// And at other times, sets specified status Off.
		/// 
		/// Does not send emails.
		/// </summary>
		/// <param name="algorithm"></param>
		static void TimeTrigger(DataRow algorithm)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(TimeTrigger));
			var vStep = "Getting data";

			var db = ExpSql.GenerateFromDefaults(_Logger);

			// algorithmId
			int algorithmId = (int)algorithm[nameof(IAlgorithm.Id)];
			vLoc += string.Format("\t=> {0}", algorithmId);

			// Get Date and Time
			DateTime dateStart = (DateTime)algorithm[nameof(IAlgorithm.DateStart)];
			DateTime dateEnd = (DateTime)algorithm[nameof(IAlgorithm.DateEnd)];

			TimeSpan timeStart = (TimeSpan)algorithm[nameof(IAlgorithm.TimeStart)];
			TimeSpan timeEnd = (TimeSpan)algorithm[nameof(IAlgorithm.TimeEnd)];

			DateTime dateTimeStart = dateStart.Add(timeStart);
			DateTime dateTimeEnd = dateEnd.Add(timeEnd);

			// Get Status
			decimal status = (decimal)algorithm[nameof(IAlgorithm.Status)];

			// Get the GroupId for the data points it contains to which the data will be written
			int groupId = (int)algorithm[nameof(IAlgorithm.GroupId)];

			// Get the DatapointId to which the value will be written
			int datapointId = (int)algorithm[nameof(IAlgorithm.DatapointId)];

			// Get the value to be written
			decimal valueOff = (decimal)algorithm[nameof(IAlgorithm.ValueOff)];
			decimal valueOn = (decimal)algorithm[nameof(IAlgorithm.ValueOn)];

			// Need to write a value or not
			bool needToWrite = false;
			decimal writeValue = valueOff;

			// Updated status
			bool statusUpdated = false;

			// Check Date and Time
			if (DateTime.Now >= dateTimeStart &&
				DateTime.Now <= dateTimeEnd)
			{
				// Check that the new value does not match the last written value.
				if (status != valueOn)
				{
					needToWrite = true;
					writeValue = valueOn;
				}
			}
			else
			{
				// Check that the new value does not match the last written value.
				if (status != valueOff)
				{
					needToWrite = true;
					writeValue = valueOff;
				}
			}

			// Check need to write a value or not
			if (needToWrite)
			{
				// Check if GroupId/DatapointId is set"
				switch (WhereWriteValue(datapointId, groupId))
				{
					case 1:
						// DatapointId is set
						WriteValueByDatapointId(datapointId.ToString(), writeValue.ToString());

						// Update algorithm status
						//StatusUpdated = db.AlgorithmStatusUpdate((int)algorithm[Defaults.DB_ID], writeValue);
						statusUpdated = db.AlgorithmStatusUpdate(algorithmId, writeValue);
						break;

					case 2:
						// GroupId is set"
						WriteValueByGroupId(groupId.ToString(), writeValue.ToString());

						// Update algorithm status
						//StatusUpdated = db.AlgorithmStatusUpdate((int)algorithm[Defaults.DB_ID], writeValue);
						statusUpdated = db.AlgorithmStatusUpdate(algorithmId, writeValue);
						break;

					case 0:
						// GroupId and DatapointId are not set
						_Logger.WriteLine(5, string.Format("{0}, [GroupId] and [DatapointId] are not set", vLoc));
						break;

					default:
						// GroupId and DatapointId are not set
						_Logger.WriteLine(5, string.Format("{0}, [GroupId] and [DatapointId] are not set", vLoc));
						break;
				}
			}
		}


		/// <summary>
		/// Periodically, not during specific date (from - to), 
		/// but during specific day of week and time (not a date) sets specified On state.
		/// And at other times, sets specified status Off.
		/// 
		/// Does not send emails.
		/// </summary>
		/// <param name="row"></param>
		static void PeriodicTimeTrigger(DataRow row)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(PeriodicTimeTrigger));
			var vStep = "Getting data";

			var db = ExpSql.GenerateFromDefaults(_Logger);

			// algorithmId
			int algorithmId = (int)row[nameof(IAlgorithm.Id)];
			vLoc += string.Format("\t=> {0}", algorithmId);

			// Get Time and DayOfWeek
			TimeSpan timeStart = (TimeSpan)row[nameof(IAlgorithm.TimeStart)];
			TimeSpan timeEnd = (TimeSpan)row[nameof(IAlgorithm.TimeEnd)];

			bool onMonday = (bool)(row[nameof(IAlgorithm.OnMonday)]);
			bool onTuesday = (bool)(row[nameof(IAlgorithm.OnTuesday)]);
			bool onWednesday = (bool)(row[nameof(IAlgorithm.OnWednesday)]);
			bool onThursday = (bool)(row[nameof(IAlgorithm.OnThursday)]);
			bool onFriday = (bool)(row[nameof(IAlgorithm.OnFriday)]);
			bool onSaturday = (bool)(row[nameof(IAlgorithm.OnSaturday)]);
			bool onSunday = (bool)(row[nameof(IAlgorithm.OnSunday)]);

			// Get Status
			decimal status = (decimal)(row[nameof(IAlgorithm.Status)]);

			// Get the GroupId for the data points it contains to which the data will be written
			int groupId = (int)row[nameof(IAlgorithm.GroupId)];

			// Get the DatapointId to which the value will be written
			int datapointId = (int)row[nameof(IAlgorithm.DatapointId)];

			// Get the value to be written
			decimal valueOff = (decimal)row[nameof(IAlgorithm.ValueOff)];
			decimal valueOn = (decimal)row[nameof(IAlgorithm.ValueOn)];

			// Need to write a value or not
			bool needToWrite = false;
			decimal writeValue = valueOff;

			// Updated status
			bool statusUpdated = false;

			// Check Time and DayOfWeek
			if (
				System.DateTime.Now.TimeOfDay >= timeStart &&
				System.DateTime.Now.TimeOfDay <= timeEnd &&

				(DateTime.Today.DayOfWeek == DayOfWeek.Monday && onMonday ||
				DateTime.Today.DayOfWeek == DayOfWeek.Tuesday && onTuesday ||
				DateTime.Today.DayOfWeek == DayOfWeek.Wednesday && onWednesday ||
				DateTime.Today.DayOfWeek == DayOfWeek.Thursday && onThursday ||
				DateTime.Today.DayOfWeek == DayOfWeek.Friday && onFriday ||
				DateTime.Today.DayOfWeek == DayOfWeek.Saturday && onSaturday ||
				DateTime.Today.DayOfWeek == DayOfWeek.Sunday && onSunday)
				)
			{
				// Check that the new value does not match the last written value.
				if (status != valueOn)
				{
					needToWrite = true;
					writeValue = valueOn;
				}
			}
			else
			{
				// Check that the new value does not match the last written value.
				if (status != valueOff)
				{
					needToWrite = true;
					writeValue = valueOff;
				}
			}

			// Check need to write a value or not
			if (needToWrite)
			{
				// Check if GroupId/DatapointId is set"
				switch (WhereWriteValue(datapointId, groupId))
				{
					case 1:
						// DatapointId is set
						WriteValueByDatapointId(datapointId.ToString(), writeValue.ToString());

						// Update algorithm status
						//StatusUpdated = db.AlgorithmStatusUpdate((int)algorithm[Defaults.DB_ID], writeValue);
						statusUpdated = db.AlgorithmStatusUpdate(algorithmId, writeValue);
						break;

					case 2:
						// GroupId is set"
						WriteValueByGroupId(groupId.ToString(), writeValue.ToString());

						// Update algorithm status
						//StatusUpdated = db.AlgorithmStatusUpdate((int)algorithm[Defaults.DB_ID], writeValue);
						statusUpdated = db.AlgorithmStatusUpdate(algorithmId, writeValue);
						break;

					case 0:
						// GroupId and DatapointId are not set
						vStep = string.Format("{0}, [GroupId] and [DatapointId] are not set", vLoc);
						_Logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));
						break;

					default:
						// GroupId and DatapointId are not set
						vStep = string.Format("{0}, [GroupId] and [DatapointId] are not set", vLoc);
						_Logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));
						break;
				}
			}
		}

		static void Alarm(DataRow algorithm)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(Alarm));
			var vStep = "Getting data";

			// Generate DB defaults
			var db = ExpSql.GenerateFromDefaults(_Logger);
			var alg = Algorithm.From<Algorithm>(algorithm);

			// Merging date with time to get datetime
			DateTime dateTimeStart = alg.DateStart.Value.Add(alg.TimeStart.Value); // dateStart.Add(timeStart);
			DateTime dateTimeEnd = alg.DateEnd.Value.Add(alg.TimeEnd.Value); // dateEnd.Add(timeEnd);

			DataTable datapointValueById = db.GetLastDatapointValue(alg.DatapointId);

			// Updated status
			bool statusUpdated = false;

			// Snooze notification till time updated status
			bool snoozeNotification = false;

			// ==========================================================
			//
			// 2024-01-09 New Algorithm
			//
			// ==========================================================

			// Check for DatapointId validity
			_Logger.WriteLine(5, string.Format("{0}, Checking DatapointId={1} validity..", vLoc, alg.DatapointId));
			if (alg.DatapointId == 0 || datapointValueById.Rows.Count == 0)
			{
				// DatapointId is not set
				_Logger.WriteLine(4, string.Format("{0}, [DatapointId] is not set", vLoc));
				return;
			}


			// Get datapoint value
			decimal datapointValue = (decimal)datapointValueById.Rows[0][nameof(IDatapointValue.Value)];

			// Value buffer (paklaida?)
			decimal valueBuffer = 0.5m;

			// Set possible alarm statuses
			decimal alarmStatusOff = 0;
			decimal alarmStatusOn = 1;

			// Need to change alarm status or not
			bool needToChange = false;
			decimal changeValue = alarmStatusOff;

			// Check Date and Time
			if (
				DateTime.Now >= dateTimeStart &&
				DateTime.Now <= dateTimeEnd &&
				(datapointValue >= (alg.ValueFrom - valueBuffer) &&
				datapointValue <= (alg.ValueTo + valueBuffer))
				)
			{
				_Logger.WriteLine(5, string.Format("{0}, Discovered Status: On", vLoc));

				// If status already was On and still is On, after the verification
				if (alg.Status == alarmStatusOn)
				{
					_Logger.WriteLine(5, string.Format("{0}, Previously it was: On, [{1}]: {2}",
						vLoc, nameof(IAlgorithm.ReminderAfterHours), alg.ReminderAfterHours));

					// Re-reminder enabled => ReminderAfterHours > 0
					// Re-reminder disabled => ReminderAfterHours = 0
					if (alg.ReminderAfterHours > 0)
					{
						// If snooze date is NULL, assign current date,
						// that it was valid for re-reminding
						if(!alg.SnoozeNotificationTill.HasValue)
						{
							alg.SnoozeNotificationTill = DateTime.Now;
						}

						if(DateTime.Now >= alg.SnoozeNotificationTill)
						{

							// Update Snooze Notification Till time before send mail
							DateTime timeNow = DateTime.Now;
							//DateTime SnoozeNotificationTillTime = TimeNow.AddHours((int)algorithm[Defaults.DB_REMINDER_AFTER_HOURS]);
							//SnoozeNotification = db.SnoozeNotificationTillTimeUpdate((int)algorithm[Defaults.DB_ID], SnoozeNotificationTillTime);

							DateTime snoozeNotificationTillTime = timeNow.AddHours(alg.ReminderAfterHours); //  (int)algorithm[nameof(IAlgorithm.ReminderAfterHours)]

							_Logger.WriteLine(5, string.Format("{0}, [{1}]: {2}",
								vLoc, nameof(IAlgorithm.SnoozeNotificationTill),
								snoozeNotificationTillTime.ToString(Defaults.DEFAULT_DATE_FORMAT))
							);

							snoozeNotification = db.SnoozeNotificationTillTimeUpdate(alg.Id, snoozeNotificationTillTime);

#if SEND_MAIL
							// Send mail
							SendMail(alg, datapointValue, 1, true);
#endif
						}

					}
				}
				else // Check that the new value does not match the last changed value.
					 //if (alg.Status != alarmStatusOn)
				{
					_Logger.WriteLine(5, string.Format("{0}, Previously it was: Off", vLoc));

					needToChange = true;
					changeValue = alarmStatusOn;
				}
			}
			else
			{
				_Logger.WriteLine(5, string.Format("{0}, Discovered Status: Off", vLoc));

				// Check that the new value does not match the last changed value.
				if (alg.Status != alarmStatusOff)
				{
					_Logger.WriteLine(5, string.Format("{0}, Previously it was: On", vLoc));

					needToChange = true;
					changeValue = alarmStatusOff;
				}
			}

			// Need to change alarm status or not
			_Logger.WriteLine(5, string.Format("{0}, [{1}]: {2}", vLoc, nameof(needToChange), needToChange));
			if (needToChange)
			{
				// Update algorithm status
				statusUpdated = db.AlgorithmStatusUpdate(alg.Id, changeValue);  // db.AlgorithmStatusUpdate((int)algorithm[Defaults.DB_ID], changeValue);

				// Update Snooze Notification Till time
				if (alg.Status == alarmStatusOff)
				{
					// 2024-01-09
					//DateTime timeNow = System.DateTime.Now;
					//DateTime snoozeNotificationTillTime = timeNow.AddHours(alg.ReminderAfterHours);   // (int)algorithm[nameof(IAlgorithm.ReminderAfterHours)]
					//snoozeNotification = db.SnoozeNotificationTillTimeUpdate(alg.Id, snoozeNotificationTillTime);

					snoozeNotification = db.SnoozeNotificationTillTimeUpdate(alg.Id, null);
				}

#if SEND_MAIL
				// Send mail
				SendMail(alg, datapointValue, changeValue, false);
#endif
			}


			/*
			// ==========================================================
			//
			// 2024-01-09 WARNING! An old algorithm! Deprecated!
			//
			// ==========================================================

			// Check for DatapointId validity
			if (alg.DatapointId == 0 || datapointValueById.Rows.Count == 0)
			{
				// DatapointId is not set
				_Logger.WriteLine(4, string.Format("{0}, [DatapointId] is not set", vLoc));
			}
			else
			{
				// Get datapoint value
				//decimal DatapointValue = (decimal)DatapointValueById.Rows[0][Defaults.DB_VALUE];
				decimal datapointValue = (decimal)datapointValueById.Rows[0][nameof(IDatapointValue.Value)];

				// If need to send the reminder
				if (DateTime.Now >= alg.SnoozeNotificationTill &&
					alg.Status == 1)
				{
					// Update Snooze Notification Till time before send mail
					DateTime timeNow = DateTime.Now;
					//DateTime SnoozeNotificationTillTime = TimeNow.AddHours((int)algorithm[Defaults.DB_REMINDER_AFTER_HOURS]);
					//SnoozeNotification = db.SnoozeNotificationTillTimeUpdate((int)algorithm[Defaults.DB_ID], SnoozeNotificationTillTime);

					DateTime snoozeNotificationTillTime = timeNow.AddHours((int)algorithm[nameof(IAlgorithm.ReminderAfterHours)]);
					snoozeNotification = db.SnoozeNotificationTillTimeUpdate(alg.Id, snoozeNotificationTillTime);

#if SEND_MAIL
					// Send mail
					SendMail(algorithm, datapointValue, 1, true);
#endif
				}

				// Set the value from and the value to
				//decimal ValueFrom = (decimal)algorithm[Defaults.DB_VALUE_FROM];
				//decimal ValueTo = (decimal)algorithm[Defaults.DB_VALUE_TO];
				//decimal valueBuffer = 0.5m;

				// Set the value from and the value to
				//decimal valueFrom = (decimal)algorithm[nameof(IAlgorithm.ValueFrom)];
				//decimal valueTo = (decimal)algorithm[nameof(IAlgorithm.ValueTo)];
				decimal valueBuffer = 0.5m;

				// Set possible alarm statuses
				decimal alarmStatusOff = 0;
				decimal alarmStatusOn = 1;

				// Need to change alarm status or not
				bool needToChange = false;
				decimal changeValue = alarmStatusOff;

				// Check Date and Time
				if (System.DateTime.Now >= dateTimeStart &&
					System.DateTime.Now <= dateTimeEnd &&
					(datapointValue >= (alg.ValueFrom - valueBuffer) &&
					datapointValue <= (alg.ValueTo + valueBuffer))
					)
				{
					// Check that the new value does not match the last changed value.
					if (alg.Status != alarmStatusOn)
					{
						needToChange = true;
						changeValue = alarmStatusOn;
					}
				}
				else
				{
					// Check that the new value does not match the last changed value.
					if (alg.Status != alarmStatusOff)
					{
						needToChange = true;
						changeValue = alarmStatusOff;
					}
				}

				// Need to change alarm status or not
				if (needToChange)
				{
					// Update algorithm status
					statusUpdated = db.AlgorithmStatusUpdate((int)algorithm[Defaults.DB_ID], changeValue);

					// Update Snooze Notification Till time
					if (alg.Status == alarmStatusOff)
					{
						DateTime timeNow = System.DateTime.Now;
						//DateTime SnoozeNotificationTillTime = TimeNow.AddHours((int)algorithm[Defaults.DB_REMINDER_AFTER_HOURS]);
						//SnoozeNotification = db.SnoozeNotificationTillTimeUpdate((int)algorithm[Defaults.DB_ID], SnoozeNotificationTillTime);

						DateTime snoozeNotificationTillTime = timeNow.AddHours((int)algorithm[nameof(IAlgorithm.ReminderAfterHours)]);
						snoozeNotification = db.SnoozeNotificationTillTimeUpdate(alg.Id, snoozeNotificationTillTime);
					}

#if SEND_MAIL
					// Send mail
					SendMail(algorithm, datapointValue, changeValue, false);
#endif
				}
			}
			*/
		}

		static void AlarmTrigger(DataRow algorithm, DataTable algorithmTable)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(AlarmTrigger));
			var vStep = "Getting data";

			var db = ExpSql.GenerateFromDefaults(_Logger);

			// algorithmId
			int algorithmId = (int)algorithm[nameof(IAlgorithm.Id)];
			vLoc += string.Format("\t=> {0}", algorithmId);

			// status
			decimal status = (decimal)algorithm[nameof(IAlgorithm.Status)]; // Defaults.DB_STATUS

			// alarmId according to which status the alarm will be generated
			int alarmId = (int)algorithm[nameof(IAlgorithm.AlarmId)];   // Defaults.DB_ALARM_ID

			// alarms list (Algorithm Type = 30)
			DataView alarms = new DataView(algorithmTable);
			alarms.RowFilter = "Type = 30";

			// Get alarm algorithm information by AlarmId
			DataView alarmInfo = alarms;
			alarmInfo.RowFilter = string.Concat("Id = ", alarmId.ToString());

			// Updated status
			bool statusUpdated = false;

			// Check or AlarmId is set
			if (alarmId == 0 || alarmInfo.Count == 0)
			{
				// AlarmId is not set
				_Logger.WriteLine(4, string.Format("{0}, [AlarmId] is not set or invalid", vLoc));
			}
			else
			{
				decimal alarmStatus = (decimal)alarmInfo[0][nameof(IAlgorithm.Status)];     // Defaults.DB_STATUS

				// Get the GroupId for the data points it contains to which the data will be written
				int groupId = (int)algorithm[nameof(IAlgorithm.GroupId)];                   // Defaults.DB_GROUP_ID

				// Get the DatapointId to which the value will be written
				int datapointId = (int)algorithm[nameof(IAlgorithm.DatapointId)];           // Defaults.DB_DATAPOINT_ID

				// Set possible alarm statuses
				decimal alarmStatusOff = 0;
				decimal alarmStatusOn = 1;

				// Get the value to be written
				decimal valueOff = (decimal)algorithm[nameof(IAlgorithm.ValueOff)];         // Defaults.DB_VALUE_OFF
				decimal valueOn = (decimal)algorithm[nameof(IAlgorithm.ValueOn)];           // Defaults.DB_VALUE_ON

				// Need to write a value or not
				bool needToWrite = false;
				decimal writeValue = valueOff;

				// Check or alarm is triggered
				if (alarmStatus == alarmStatusOn)
				{
					// Check that the new value does not match the last written value.
					if (status != valueOn)
					{
						needToWrite = true;
						writeValue = valueOn;
					}
				}
				else if (alarmStatus == alarmStatusOff)
				{
					// Check that the new value does not match the last written value.
					if (status != valueOff)
					{
						needToWrite = true;
						writeValue = valueOff;
					}
				}

				// Check need to write a value or not
				if (needToWrite)
				{
					// Check if GroupId/DatapointId is set"
					switch (WhereWriteValue(datapointId, groupId))
					{
						case 1:
							// DatapointId is set
							WriteValueByDatapointId(datapointId.ToString(), writeValue.ToString());

							// Update algorithm status
							statusUpdated = db.AlgorithmStatusUpdate(algorithmId, writeValue);  // (int)algorithm[Defaults.DB_ID]
							break;

						case 2:
							// GroupId is set"
							WriteValueByGroupId(groupId.ToString(), writeValue.ToString());

							// Update algorithm status
							statusUpdated = db.AlgorithmStatusUpdate(algorithmId, writeValue);  // (int)algorithm[Defaults.DB_ID]
							break;

						case 0:
							// GroupId and DatapointId are not set
							vStep = string.Format("{0}, [GroupId] and [DatapointId] are not set", vLoc);
							_Logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));
							break;

						default:
							// GroupId and DatapointId are not set
							vStep = string.Format("{0}, [GroupId] and [DatapointId] are not set", vLoc);
							_Logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));
							break;
					}
				}
			}
		}

		static void WriteValueByDatapointId(string datapointId, string value)
		{
			ExpControlResponse retVal = new ExpControlResponse();
			retVal.ControlResponse = new List<ExpControlStatus>();

			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(WriteValueByDatapointId));
			var vStep = "Writing value";

			try
			{
				retVal = StandAloneOperations.WriteDataPoint(datapointId, value, _Logger);

				vStep = string.Format("{0} MODBUS_WRITE, Successfully executed: datapointId: {1}, Value: {2}",
					vLoc, datapointId, value);
				_Logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));
			}
			catch (Exception ex)
			{
				retVal.ControlResponse.Add(new ExpControlStatus { Id = datapointId, Value = value, Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ErrorStatus = ExpErrorStatus.STATUS_ERROR, ErrorMsg = "Failed at step: [" + vStep + "] < br /> Cause: " + ex.Message });
				vStep = string.Format("{0} MODBUS_WRITE, Error msg: {1}",
					vLoc, "Failed at step: [" + vStep + "] < br /> Cause: " + ex.Message);
				_Logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));
			};
		}

		static void WriteValueByGroupId(string groupId, string value)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(WriteValueByGroupId));
			var db = ExpSql.GenerateFromDefaults(_Logger);

			// Get group datapoints list
			DataTable groupDatapointsTable = db.GetGroupDatapointsList(groupId);

			if (!db.IsError && groupDatapointsTable.Columns.Contains(Defaults.DB_ID))
			{
				foreach (DataRow row in groupDatapointsTable.Rows)
				{
					WriteValueByDatapointId(row[Defaults.DB_DATAPOINT_ID].ToString(), value);
				}
			}
			else
			{
				if (!groupDatapointsTable.Columns.Contains(Defaults.DB_ID))
				{
					_Logger.WriteLine(0, string.Format("{0}, Returned data sources table has no data!", vLoc));
				}
			}
		}

		static int WhereWriteValue(int datapointId, int groupId)
		{
			int result = 0;

			if (datapointId != 0)
			{
				// DatapointId is set
				result = 1;
			}
			else if (datapointId == 0 && groupId != 0)
			{
				// GroupId is set
				result = 2;
			}
			else
			{
				// GroupId and DatapointId are not set
				result = 0;
			}

			return result;
		}

		static void SendMail(Algorithm algorithm, decimal datapointValueById, decimal status, bool reminder)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(SendMail));
			var vStep = "Loading mail settings";

			// Loading mail settings
			Dictionary<string, string> mailSettings = _Mail.MailSettings();
			string subject = reminder ? "REMINDER: Alarm info" : "Alarm info";
			string textAlarmStatus = (status == 1) ? "STARTED" : "FINISHED";

			try
			{
				var db = ExpSql.GenerateFromDefaults(_Logger);

				vStep = "Getting user info by ObjectId";
				//DataTable userInfo = db.UserInfo((int)algorithm[Defaults.DB_OBJECT_ID]);
				DataTable userInfo = db.UserInfo(algorithm.ObjectId);   // (int)algorithm[nameof(IAlgorithm.ObjectId)]

				string emailBody = string.Format(
					"ALARM {0}, Id: {1}, Name: {2}, Description: {3}, Status: {4}. " +
					"DatapointId: {5}, Alarm range: {6} - {7}, Value: {8}",
					textAlarmStatus,
					algorithm.Id,			// algorithm[nameof(IAlgorithm.Id)],			// algorithm[Defaults.DB_ID],
					algorithm.Name,			// algorithm[nameof(IAlgorithm.Name)],			// algorithm[Defaults.DB_NAME],
					algorithm.Description,	// algorithm[nameof(IAlgorithm.Description)],	// algorithm[Defaults.DB_DESC],
					status,
					algorithm.DatapointId,	// algorithm[nameof(IAlgorithm.DatapointId)],	// algorithm[Defaults.DB_DATAPOINT_ID],
					algorithm.ValueFrom,	// algorithm[nameof(IAlgorithm.ValueFrom)],		// algorithm[Defaults.DB_VALUE_FROM],
					algorithm.ValueTo,		// algorithm[nameof(IAlgorithm.ValueTo)],		// algorithm[Defaults.DB_VALUE_TO],
					datapointValueById);

				DataTable table = new DataTable();
				DataRow row = table.NewRow();

				table.Columns.Add(Defaults.DB_TO, typeof(string));      // Defaults.DB_TO
				table.Columns.Add(Defaults.DB_FROM, typeof(string));    // Defaults.DB_FROM
				table.Columns.Add(Defaults.DB_SUBJECT, typeof(string)); // Defaults.DB_SUBJECT
				table.Columns.Add(Defaults.DB_BODY, typeof(string));    // Defaults.DB_BODY

				table.Rows.Add(userInfo.Rows[0][Defaults.DB_EMAIL],
					mailSettings[Mail.Username],
					subject,
					emailBody);

				var wProcessingSendMailStr = string.Format(
					"to: {0}, from: {1}, subject: {2}, body: {3}",
					table.Rows[0][Defaults.DB_TO].ToString(),
					table.Rows[0][Defaults.DB_FROM].ToString(),
					table.Rows[0][Defaults.DB_SUBJECT].ToString(),
					table.Rows[0][Defaults.DB_BODY].ToString()
				   );

				var mail = Mail.Send(mailSettings, table.Rows[0]);

				if (mail == SendMailState.Sent)
				{
					vStep = string.Format("{0}, Mail successfully sent: {1}",
						vLoc,
						wProcessingSendMailStr);
					_Logger.WriteLine(4, string.Format("{0}, {1}", vLoc, vStep));
				}
				else
				{
					vStep = string.Format("{0}, Error. Email not sent: {1}",
						vLoc,
						wProcessingSendMailStr);
					_Logger.WriteLine(3, string.Format("{0}, {1}", vLoc, vStep));
				}
			}
			catch (Exception ex)
			{
				var errorMsg = string.Format(
					"{0}, Failed at: {1}, with: {2},\r\n in: {3}",
					vLoc, vStep, ex.Message, ex.StackTrace);
				_Logger.WriteLine(0, string.Format("{0}, {1}", vLoc, errorMsg));
			}
		}



#warning TODO: Switch all functions on ControlService::SendMail(Algorithm algorithm.. (see below)
		/// <summary>
		/// @DEPRECATED
		/// </summary>
		/// <param name="algorithm"></param>
		/// <param name="datapointValueById"></param>
		/// <param name="status"></param>
		/// <param name="reminder"></param>
		static void SendMail(DataRow algorithm, decimal datapointValueById, decimal status, bool reminder)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(SendMail));
			var vStep = "Loading mail settings";

			// Loading mail settings
			Dictionary<string, string> mailSettings = _Mail.MailSettings();
			string subject = reminder ? "REMINDER: Alarm info" : "Alarm info";
			string textAlarmStatus = (status == 1) ? "STARTED" : "FINISHED";

			try
			{
				var db = ExpSql.GenerateFromDefaults(_Logger);

				vStep = "Getting user info by ObjectId";
				//DataTable userInfo = db.UserInfo((int)algorithm[Defaults.DB_OBJECT_ID]);
				DataTable userInfo = db.UserInfo((int)algorithm[nameof(IAlgorithm.ObjectId)]);

				string emailBody = string.Format(
					"ALARM {0}, Id: {1}, Name: {2}, Description: {3}, Status: {4}. " +
					"DatapointId: {5}, Alarm range: {6} - {7}, Value: {8}",
					textAlarmStatus,
					algorithm[nameof(IAlgorithm.Id)],                   // algorithm[Defaults.DB_ID],
					algorithm[nameof(IAlgorithm.Name)],                 // algorithm[Defaults.DB_NAME],
					algorithm[nameof(IAlgorithm.Description)],          // algorithm[Defaults.DB_DESC],
					status,
					algorithm[nameof(IAlgorithm.DatapointId)],          // algorithm[Defaults.DB_DATAPOINT_ID],
					algorithm[nameof(IAlgorithm.ValueFrom)],            // algorithm[Defaults.DB_VALUE_FROM],
					algorithm[nameof(IAlgorithm.ValueTo)],              // algorithm[Defaults.DB_VALUE_TO],
					datapointValueById);

				DataTable table = new DataTable();
				DataRow row = table.NewRow();

				table.Columns.Add(Defaults.DB_TO, typeof(string));      // Defaults.DB_TO
				table.Columns.Add(Defaults.DB_FROM, typeof(string));    // Defaults.DB_FROM
				table.Columns.Add(Defaults.DB_SUBJECT, typeof(string)); // Defaults.DB_SUBJECT
				table.Columns.Add(Defaults.DB_BODY, typeof(string));    // Defaults.DB_BODY

				table.Rows.Add(userInfo.Rows[0][Defaults.DB_EMAIL],
					mailSettings[Mail.Username],
					subject,
					emailBody);

				var wProcessingSendMailStr = string.Format(
					"to: {0}, from: {1}, subject: {2}, body: {3}",
					table.Rows[0][Defaults.DB_TO].ToString(),
					table.Rows[0][Defaults.DB_FROM].ToString(),
					table.Rows[0][Defaults.DB_SUBJECT].ToString(),
					table.Rows[0][Defaults.DB_BODY].ToString()
				   );

				var mail = Mail.Send(mailSettings, table.Rows[0]);

				if (mail == SendMailState.Sent)
				{
					vStep = string.Format("{0}, Mail successfully sent: {1}",
						vLoc,
						wProcessingSendMailStr);
					_Logger.WriteLine(4, string.Format("{0}, {1}", vLoc, vStep));
				}
				else
				{
					vStep = string.Format("{0}, Error. Email not sent: {1}",
						vLoc,
						wProcessingSendMailStr);
					_Logger.WriteLine(3, string.Format("{0}, {1}", vLoc, vStep));
				}
			}
			catch (Exception ex)
			{
				var errorMsg = string.Format(
					"{0}, Failed at: {1}, with: {2},\r\n in: {3}",
					vLoc, vStep, ex.Message, ex.StackTrace);
				_Logger.WriteLine(0, string.Format("{0}, {1}", vLoc, errorMsg));
			}
		}

		#endregion
	}
}
