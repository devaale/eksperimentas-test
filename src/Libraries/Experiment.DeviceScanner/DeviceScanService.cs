using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Experiment.Core;
using Experiment.Core.IO;
using Experiment.Data.Enums;
using Experiment.Core.BL.Data.SysVars;
using Experiment.Core.BL.Data;
using Experiment.Core.Metadata;

using Experiment.DeviceScanner.Data;
using System.Runtime.InteropServices;

namespace Experiment.DeviceScanner{
	public partial class DeviceScanService : ServiceBase
	{
		#region Constants

		const string TYPE_NAME = nameof(DeviceScanService);
		/// <summary>
		/// Default log level
		/// </summary>
		public const int DEFAULT_LOG_LEVEL = 5;
		/// <summary>
		/// Thread Id pattern, which shown in logs
		/// </summary>
		const string THREAD_ID_PATTERN = "Thread:{0}, DeviceId:{1}";
		/// <summary>
		/// Main scan delay
		/// </summary>
		const int DEFAULT_LOOP_DELAY = 15; // secs
		/// <summary>
		/// After which time period retry to scan the device in case of unknown failure
		/// </summary>
		const int FAILURE_DELAY = -10; // mins
		/// <summary>
		/// Second of time in miliseconds
		/// </summary>
		const int SECOND_DELAY = 1000; // ms

		/// <summary>
		/// How many seconds is one waiting loop, to prevent checking every second
		/// </summary>
		const int LOOP_SECONDS = 3;

		#endregion

		#region Attributes
		bool _ServiceStarted;
		EventLog _EventLog;
		ILogger _Logger;
		Thread _Thread;

		ExpSql _Db;
		Dictionary<string, DateTime> _CurrentlyScanningDevices;
		int _LoopDelay;
		IDictionary<SysVarName, object> _Vars = new Dictionary<SysVarName, object>();

		#endregion

		#region Init 
		public DeviceScanService()
		{
			InitializeComponent();

			_ServiceStarted = false;
			if (Environment.UserInteractive)
			{
				_Logger = new ConsoleLogger(
					DEFAULT_LOG_LEVEL,
					TYPE_NAME);
			}
			else
			{
				_Logger = new FileLogger(
					DEFAULT_LOG_LEVEL,
					Defaults.DEFAULT_LOG_FOLDER,
					TYPE_NAME);
			}

			InitEventLog();
			_Db = ExpSql.GenerateFromDefaults(_Logger);
			_CurrentlyScanningDevices = new Dictionary<string, DateTime>();

			UpdateSysVars();
		}

		#endregion

		#region Helpers
		void InitEventLog()
		{
			try
			{
				_EventLog = new EventLog();// "Application");
				_EventLog.Source = TYPE_NAME;
			}
			catch (Exception ex)
			{
				_Logger.WriteLine(0, TYPE_NAME + ": Initializing of event log went wrong: " + ex.Message);
			}
		}

		void WriteToEventLog(string msg)
		{
			if (_EventLog != null)
			{
				_EventLog.WriteEntry(msg);
			}
		}

		void UpdateSysVars()
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(UpdateSysVars));

			try
			{
				_Vars = _Db.SysVarsGet(SysVarModule.Scan);

				if (_Vars.ContainsKey(SysVarName.SCAN_LOOP_DELAY))
				{
					_LoopDelay = Convert.ToInt32(_Vars[SysVarName.SCAN_LOOP_DELAY]);
				}

				if (_Vars.ContainsKey(SysVarName.SCAN_LOG_LEVEL))
				{
					_Logger.LogLevel = Convert.ToInt32(_Vars[SysVarName.SCAN_LOG_LEVEL]);
				}

				if (_Logger is FileLogger)
				{
					FileLogger logger = (FileLogger)_Logger;
					if (_Vars.ContainsKey(SysVarName.SCAN_LOG_LOCATION))
					{
						logger.LogFolder = _Vars[SysVarName.SCAN_LOG_LOCATION].ToString();
					}

					if (_Vars.ContainsKey(SysVarName.SCAN_LOG_USE_DATES))
					{
						logger.UseDatesInLogFileNames = _Vars[SysVarName.SCAN_LOG_USE_DATES].ToString().Equals("1");
					}
				}
			}
			catch (Exception ex)
			{
				_Logger.WriteLine(0, string.Format("{0}, {1}.", vLoc, ex.Message));
			}
		}

		List<ScanDeviceInfo> LoadDataFromDb()
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(LoadDataFromDb));

			List<ScanDeviceInfo> retVal = new List<ScanDeviceInfo>();

			try
			{
				DataTable devices = _Db.Query("prcDeviceListForScanning");

				if (devices.Rows.Count > 0)
				{
					foreach (DataRow row in devices.Rows)
					{
						retVal.Add(new ScanDeviceInfo(row));
					}
				}
				else
				{
					_Logger.WriteLine(3, string.Format("{0}, Found 0 or NO RECORDS, returning...", vLoc));
				}
			}
			catch (Exception ex)
			{
				_Logger.WriteLine(0, string.Format("{0}, {1}.", vLoc, ex.Message));
			}

			return retVal;
		}

		void Heartbeat()
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(Heartbeat));
			_Logger.WriteLine(4, string.Format("{0}, Start..", vLoc));

			while (_ServiceStarted)
			{
				try
				{
					// Updating system variables
					_Logger.WriteLine(5, string.Format("{0}, {1}, Log level is: {2}..", vLoc, nameof(UpdateSysVars), _Logger.LogLevel));
					UpdateSysVars();

					// Retrieving device data from db
					_Logger.WriteLine(5, string.Format("{0}, {1}..", vLoc, nameof(LoadDataFromDb)));
					List<ScanDeviceInfo> devices = LoadDataFromDb();

					// Processing each device
					foreach (ScanDeviceInfo device in devices)
					{
						_Logger.WriteLine(4, string.Format("{0}, ForEach Device, Id={1}, Protocol={2}.",
							vLoc, device.Id, device.Protocol));

						if (device != null)
						{
							ProcessDevice(device);

							if (!_ServiceStarted)
								break;
						}
					}

					// Making delay before next cycle
					for (int i = _LoopDelay; i > 0; i -= LOOP_SECONDS)
					{
						Thread.Sleep(SECOND_DELAY * LOOP_SECONDS);
						if (!_ServiceStarted)
						{
							_Logger.WriteLine(5, string.Format("{0}, STOP RECEIVED!", vLoc));
							break; // exit if stopped
						}
					}
				}
				catch (Exception ex)
				{
					_Logger.WriteLine(0, string.Format("{0}, {1}.", vLoc, ex.Message));
				}
			}
			_Logger.WriteLine(5, string.Format("{0}, {1}={2}", vLoc, nameof(_ServiceStarted), _ServiceStarted));
		}

		/// <summary>
		/// Heartbeat >> device processing
		/// </summary>
		/// <param name="device"></param>
		/// <returns>
		/// Unused but could be utilized
		/// </returns>
		bool ProcessDevice(ScanDeviceInfo device)
		{
			var vLoc = string.Format("{0}::{1}(device.Id={2})", TYPE_NAME, nameof(ProcessDevice), device.Id);
			var retVal = false;

			try
			{
				_Logger.WriteLine(4, string.Format("{0}, Start..", vLoc));

				// Normal start in case if device not under scan yet
				if (!_CurrentlyScanningDevices.ContainsKey(device.Id))
				{
					_CurrentlyScanningDevices.Add(device.Id, DateTime.Now);
					StartScan(device);
				}

				// Start after 10 minutes of failure
				else if (_CurrentlyScanningDevices[device.Id] < DateTime.Now.AddMinutes(FAILURE_DELAY))
				{
					_Logger.WriteLine(3, string.Format(
						"{0}, Device Id: {1} already under scanning, but it took too long [projected:{2}]...",
						vLoc, device.Id, _CurrentlyScanningDevices[device.Id].ToString(Defaults.DEFAULT_DATETIME_FORMAT)));

					StartScan(device);
				}

				// Information about that device already under scanning (redundant?)
				else
				{
					_Logger.WriteLine(4, string.Format(
						"{0}, Device Id: {1} already under scanning..",
						vLoc, device.Id));
				}

				retVal = true;
			}
			catch (Exception ex)
			{
				_Logger.WriteLine(0, string.Format("{0}, {1}.", vLoc, ex.Message));
			}

			return retVal;
		}

		/// <summary>
		/// Scan starting, threat starting method
		/// </summary>
		/// <param name="deviceId"></param>
		void StartScan(ScanDeviceInfo device)
		{
			var vLoc = string.Format("{0}::{1}(deviceId={2})", TYPE_NAME, nameof(StartScan), device.Id);

			try
			{
				_Logger.WriteLine(5, string.Format("{0}", vLoc));

				ScanThreadStateObject state = new ScanThreadStateObject()
				{
					Device = device,
					Db = ExpSql.GenerateFromDefaults(_Logger),
					State = ScanThreadState.Started,
					Logger = _Logger,
					CurrentlyScanningDevices = _CurrentlyScanningDevices,
				};

				// Starting the thread
				Thread thread = new Thread(new ParameterizedThreadStart(DoWork));
				GenerateThreadId(state, thread);
				thread.Start(state);
				_Logger.WriteLine(5, string.Format("{0}, End!", vLoc));
			}
			catch (Exception ex)
			{
				_Logger.WriteLine(0, string.Format("{0}, {1}.", vLoc, ex.Message));
			}
		}

		#endregion

		#region Static

		/// <summary>
		/// After thread.Start is done, this static method is executed which will instantiate device scanning class and start it
		/// </summary>
		/// <param name="obj"></param>
		public void DoWork(object obj)
		{
			ScanThreadStateObject state = (ScanThreadStateObject)obj;
			var vLoc = string.Format("{0}/{1}::{2}", state.DebugThreadId, TYPE_NAME, nameof(DoWork));

			try
			{
				state.Logger.WriteLine(4, string.Format("{0}, Protocol: {1}", vLoc, state.Device.Protocol));

				switch (state.Device.Protocol)
				{
					case DeviceProtocol.Experiment.Modbus:
						new ModbusScanner(state, state.Logger).Start();
						break;

					case DeviceProtocol.API:
						new ApiScanner(state, state.Logger).Start();
						break;

					default:

						break;

				}

				// End up
				state.Logger.WriteLine(5, string.Format("{0}, ended! Clean up in progress...", vLoc));

				// Clean up of the mess
				if (state.CurrentlyScanningDevices.ContainsKey(state.Device.Id))
					state.CurrentlyScanningDevices.Remove(state.Device.Id);

				state.Logger.WriteLine(5, string.Format("{0}, ended! Clean up in progress. Finish", vLoc));
			}
			catch (Exception ex)
			{
				state.Logger.WriteLine(0, string.Format("{0}, {1}.", vLoc, ex.Message));
			}
		}

		internal static string GenerateThreadId(
			ScanThreadStateObject state,
			Thread thread)
		{
			state.DebugThreadId = string.Format(
				THREAD_ID_PATTERN,
				thread.GetHashCode(),
				state.Device.Id);
			return state.DebugThreadId;
		}

		#endregion

		#region Methods
		public void Start()
		{
			OnStart(new string[] { });
		}

		protected override void OnStart(string[] args)
		{
			var vLoc = string.Format("{0}::{1}..", TYPE_NAME, nameof(OnStart));
			_Logger.WriteLine(3, vLoc);
			WriteToEventLog(vLoc);

			_ServiceStarted = true;

			_Thread = new Thread(Heartbeat);
			_Thread.Start();

		}

		protected override void OnStop()
		{
			var msg = string.Format("{0}::{1}\r\n\r\n\r\n", TYPE_NAME, nameof(OnStop));
			_Logger.WriteLine(3, msg);
			WriteToEventLog(msg);

			_ServiceStarted = false;
		}

		#endregion
	}
}
