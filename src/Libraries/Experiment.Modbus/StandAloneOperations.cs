#define FileLogger

using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Experiment.Core;
using Experiment.Core.IO;
using Experiment.Core.BL.Data;
using Experiment.Core.Metadata;
using Experiment.Core.Web;

namespace Experiment.Modbus{
	public class StandAloneOperations
	{
		#region Constants
		public const int DEFAULT_LOG_LEVEL = 5;

		#endregion

		static private ILogger _Logger;
		//static EventLog _EventLog;
		static private Experiment.Modbus.Converters _Converter;

		internal StandAloneOperations()
		{

		}

		public static ExpResponse ScanDataPoint (string dataPointId, ILogger logger)
		{
			String step = "Initialization";
			ExpResponse retVal = new ExpResponse();
			try
			{
				step = "Retrieving data from database";
				ExpSql db = ExpSql.GenerateFromDefaults(logger);
				DataTable info = db.GetDataPointScanAndParseInfo(dataPointId);
				if(info.Rows.Count<1)
				{
					// device not found
					retVal.ResponseStatus.ErrorMsg = "Such datapoint not found";
					return retVal;
				}
				retVal.ResponseData = info;

				step = "Initializing scanning";
				ExpDevice device = new ExpDevice(info.Rows[0]);

				step = "Trying to scan the device";
				decimal result = ScanTheDevice(device);

				retVal.ResponseData = new { id = dataPointId, value = result, date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
			}
			catch (Exception ex)
			{
				retVal.ResponseStatus.ErrorMsg = "Failed at step: [" + step + "]<br />Cause: " + ex.Message;// + ", \nat: " + ex.StackTrace;
			}
			return retVal;
		}

		public static decimal ScanTheDevice(ExpDevice device)
		{
			// extra datapoint's info
			int start = int.Parse(device[Defaults.DB_DATA_START].ToString());
			//int bitsize = int.Parse(device[Defaults.DB_BITSIZE].ToString());
			int registerType = int.Parse(device[Defaults.DB_REGISTER_TYPE].ToString());
			int functionCode = int.Parse(device[Defaults.DB_FUNCTION_CODE].ToString());
			decimal multiplier = decimal.Parse(device[Defaults.DB_MULTIPLIER].ToString());
			decimal offset = decimal.Parse(device[Defaults.DB_OFFSET].ToString());

			byte[] _SyncScanResult = { };
			decimal result = 0;

#if FileLogger
			_Logger = new FileLogger(
			DEFAULT_LOG_LEVEL,
			Defaults.DEFAULT_LOG_FOLDER,
			"StandAloneScan");

			ModbusWrapper _Modbus = new ModbusWrapper(_Logger);

			_Logger.WriteLine(3, "-------------------------------------------------------------------------------------------------------------------------");
			_Logger.WriteLine(3, $"Device. IP Address: {device.Host}, Port: {device.Port}, Unit ID: {device.UnitID}");

			_Modbus.Connect(device.Host, device.Port, device.UnitID);

			_Converter = new Experiment.Modbus.Converters(_Logger);
#else
			_Converter = new Experiment.Modbus.Converters(new DebugLogger(0));
#endif

			if (_Modbus.IsConnected)
			{
				// Connected
				// Reading from device
				_Logger.WriteLine(3, "Connected.");

				switch (functionCode)
				{
					// Read Coils from Server device (FC=01).
					case 1:
						_SyncScanResult = _Modbus.ReadCoils(
							start, _Converter.ParseRegisterType_Dict(registerType).BitSize);

						result = _Converter.ParseData(
							_SyncScanResult, 0, registerType, multiplier, offset);
						break;

					// Read Discrete Inputs from Server device(FC=02).
					case 2:
						_SyncScanResult = _Modbus.ReadDiscreteInputs(
							start, _Converter.ParseRegisterType_Dict(registerType).BitSize);

						result = _Converter.ParseData(
							_SyncScanResult, 0, registerType, multiplier, offset);
						break;

					// Read Holding Registers from Master device (FC=03).
					case 3:
						_SyncScanResult = Experiment.Core.Utils.Convert(
							_Modbus.ReadHoldingRegisters(
								start, _Converter.ParseRegisterType_Dict(registerType).BitSize / 16));

						result = _Converter.ParseData(
							_SyncScanResult, 0, registerType, multiplier, offset);
						break;

					// Read Input Registers from Master device (FC=04).
					case 4:
						_SyncScanResult = Experiment.Core.Utils.Convert(
							_Modbus.ReadInputRegisters(
								start, _Converter.ParseRegisterType_Dict(registerType).BitSize / 16));

						result = _Converter.ParseData(
							_SyncScanResult, 0, registerType, multiplier, offset);
						break;

					default:
						//throw new NotImplementedException();
						break;
				}

				_Logger.WriteLine(3, $"Function code: {functionCode}, " +
					$"Start: {start}, " +
					$"Register Type: {registerType}, " +
					$"BitSize: {_Converter.ParseRegisterType_Dict(registerType).BitSize}, " +
					$"Multiplier: {multiplier}, " +
					$"Offset: {offset} ");

				_Logger.WriteLine(3, $"Result: {result}");

				_Modbus.Disconnect();

				_Logger.WriteLine(3, "Disconnected.");
			}
			else
			{
				_Logger.WriteLine(3, "Unable to connect to modbus device.");

				throw new Exception("Unable to connect to modbus device.");
			}

			return result;
		}

		public static ExpControlResponse WriteDataPoint(string datapointId, string value, ILogger logger)
		{
			String step = "Initialization";
			ExpControlResponse retVal = new ExpControlResponse();

			retVal.ControlResponse = new List<ExpControlStatus>();

			try
			{
				step = "Retrieving data from database";
				ExpSql db = ExpSql.GenerateFromDefaults(logger) ;
				DataTable info = db.GetDataPointScanAndParseInfo(datapointId);
				if (info.Rows.Count < 1)
				{
					// device not found
					//retVal.ResponseStatus.ErrorMsg = "Such datapoint not found";
					retVal.ControlResponse.Add(new ExpControlStatus { Id = datapointId, Value = value, Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ErrorStatus = ExpErrorStatus.STATUS_ERROR, ErrorMsg = "Such datapoint not found" });

					return retVal;
				}
				//retVal.ResponseData = info;

				step = "Initializing data writing";
				ExpDevice device = new ExpDevice(info.Rows[0]);

				step = "Trying to write to device";
				decimal result = WriteToDevice(device, datapointId, value);

				//retVal.ResponseData = new { id = dataPointId, value = value, date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
				retVal.ControlResponse.Add(new ExpControlStatus { Id = datapointId, Value = value, Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ErrorStatus = ExpErrorStatus.STATUS_OK, ErrorMsg = "" });
			}
			catch (Exception ex)
			{
				//retVal.ResponseStatus.ErrorMsg = "Failed at step: [" + step + "]<br />Cause: " + ex.Message;// + ", \nat: " + ex.StackTrace;
				//retVal.ControlResponse.Add(new EseControlStatus { Id = datapointId, Value = value, Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ErrorStatus = EseErrorStatus.STATUS_ERROR, ErrorMsg = "Failed at step: [" + step + "] < br /> Cause: " + ex.Message });
				retVal.ControlResponse.Add(new ExpControlStatus { Id = datapointId, Value = value, Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ErrorStatus = ExpErrorStatus.STATUS_ERROR, ErrorMsg = ex.Message });
			}
			return retVal;
		}

		public static ExpControlResponse WriteDataPoints(string dataPointIds, string registerType, string value, ILogger logger)
		{
			String step = "Initialization";
			ExpControlResponse retVal = new ExpControlResponse();

			retVal.ControlResponse = new List<ExpControlStatus>();

			string[] dataPointIdsArray = dataPointIds.Split('|');

			foreach (string datapointId in dataPointIdsArray)
			{
				try
				{
					step = "Retrieving data from database";
					ExpSql db = ExpSql.GenerateFromDefaults(logger);
					DataTable info = db.GetDataPointScanAndParseInfo(datapointId);

					// Checking if a data point exists
					if (info.Rows.Count < 1)
					{
						// device not found
						//retVal.ResponseStatus.ErrorMsg = "Such datapoint not found";
						retVal.ControlResponse.Add(new ExpControlStatus { Id = datapointId, Value = value, Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ErrorStatus = ExpErrorStatus.STATUS_ERROR, ErrorMsg = "Such datapoint not found" });

						return retVal;
					}
					//retVal.ResponseData = info;

					step = "Initializing data writing";
					ExpDevice device = new ExpDevice(info.Rows[0]);

					int datapointRegisterType = int.Parse(device[Defaults.DB_REGISTER_TYPE].ToString());

					// Checking if a datapoint type match writing data type
					if (datapointRegisterType == int.Parse(registerType))
					{
						step = "Trying to write to device";
						decimal result = WriteToDevice(device, datapointId, value);

						//retVal.ResponseData = new { id = dataPointId, value = value, date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
						retVal.ControlResponse.Add(new ExpControlStatus { Id = datapointId, Value = value, Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ErrorStatus = ExpErrorStatus.STATUS_OK, ErrorMsg = "" });
					}
					else
					{
						//retVal.ResponseData = new { id = dataPointId, value = value, date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
						retVal.ControlResponse.Add(new ExpControlStatus { Id = datapointId, Value = value, Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ErrorStatus = ExpErrorStatus.STATUS_ERROR, ErrorMsg = "modbus-error-status-20" });
					}
				}
				catch (Exception ex)
				{
					//retVal.ResponseStatus.ErrorMsg = "Failed at step: [" + step + "]<br />Cause: " + ex.Message;// + ", \nat: " + ex.StackTrace;
					//retVal.ControlResponse.Add(new EseControlStatus { Id = datapointId, Value = value, Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ErrorStatus = EseErrorStatus.STATUS_ERROR, ErrorMsg = "Failed at step: [" + step + "] < br /> Cause: " + ex.Message });
					retVal.ControlResponse.Add(new ExpControlStatus { Id = datapointId, Value = value, Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ErrorStatus = ExpErrorStatus.STATUS_ERROR, ErrorMsg = ex.Message });
				}
			}

			return retVal;
		}

		public static decimal WriteToDevice(ExpDevice device, string dataPointId, string value)
		{
			// extra datapoint's info
			int start = int.Parse(device[Defaults.DB_REGISTER_ADDRESS].ToString());
			int registerType = int.Parse(device[Defaults.DB_REGISTER_TYPE].ToString());
			int functionCode = int.Parse(device[Defaults.DB_FUNCTION_CODE].ToString());
			int writeValue = decimal.ToInt16(Convert.ToDecimal(value));

			byte[] _SyncScanResult = { };
			decimal result = 0;

#if FileLogger
			_Logger = new FileLogger(
			DEFAULT_LOG_LEVEL,
			Defaults.DEFAULT_LOG_FOLDER,
			"StandAloneWrite");

			ModbusWrapper _Modbus = new ModbusWrapper(_Logger);

			_Logger.WriteLine(3, "-------------------------------------------------------------------------------------------------------------------------");
			_Logger.WriteLine(3, $"Device. IP Address: {device.Host}, Port: {device.Port}, Unit ID: {device.UnitID}");

			_Modbus.ConnectWrite(device.Host, device.Port, device.UnitID);

			_Converter = new Experiment.Modbus.Converters(_Logger);
#else
			_Converter = new Experiment.Modbus.Converters(new DebugLogger(0));
#endif

			if (_Modbus.IsConnectedWrite)
			{
				// Connected
				// Writing to device
				_Logger.WriteLine(3, "Connected.");

				switch (functionCode)
				{
					// Write Single Coil (FC=05).
					case 5:

						// Convert int to boolean
						bool boolVal;
						switch (writeValue)
						{
							case 0:
								boolVal = false;
								break;
							case 1:
								boolVal = true;
								break;
							default:
								//throw new InvalidOperationException("Integer value is not valid");
								boolVal = true;
								break;
						}

						_Modbus.WriteSingleCoil(start, boolVal);
						break;

					// Write Holding Register (FC=06).
					case 6:
						_Modbus.WriteHoldingRegister(start, writeValue);
						break;

					default:
						//throw new NotImplementedException();
						break;
				}

				/*
				// Write Holding Register to Master device (FC=06).
				_Modbus.WriteHoldingRegister(start, writeValue);
				
				_Logger.WriteLine(3,
					$"DatapointId: {dataPointId}, " +
					$"Function code: {functionCode}, " +
					$"StartAddress: {start}, " +
					$"RegisterType: {registerType}, " +
					$"BitSize: {_Converter.ParseRegisterType_Dict(registerType).BitSize}, " +
					$"Value: {writeValue}");

				_Logger.WriteLine(3, $"Result: {result}");
				*/
				_Modbus.DisconnectWrite();

				_Logger.WriteLine(3, "Disconnected.");
			}
			else
			{
				_Logger.WriteLine(3, "Unable to connect to modbus device.");

				throw new Exception("modbus-error-status-10");
			}

			return result;
		}

		public static decimal ReadFromDeviceMobile(
			string Host,
			int Port,
			int UnitID,
			int Address,
			int FunctionCode,
			int RegisterType,
			decimal Multiplier,
			decimal Offset,
			string dataPointId)
		{
			// extra datapoint's info
			int start = Address;
			//int registerType = int.Parse(device[Defaults.DB_REGISTER_TYPE].ToString());
			int functionCode = FunctionCode;
			int registerType = RegisterType;
			decimal multiplier = Multiplier;
			decimal offset = Offset;

			byte[] _SyncScanResult = { };
			decimal result = 0;

#if FileLogger
			_Logger = new FileLogger(
			DEFAULT_LOG_LEVEL,
			Defaults.DEFAULT_LOG_FOLDER,
			"StandAloneScan");

			ModbusWrapper _Modbus = new ModbusWrapper(_Logger);

			_Logger.WriteLine(3, "-------------------------------------------------------------------------------------------------------------------------");
			_Logger.WriteLine(3, $"Device. IP Address: {Host}, Port: {Port}, Unit ID: {UnitID}");

			_Modbus.Connect(Host, Port, UnitID);

			_Converter = new Experiment.Modbus.Converters(_Logger);
#else
			_Converter = new Experiment.Modbus.Converters(new DebugLogger(0));
#endif

			if (_Modbus.IsConnected)
			{
				// Connected
				// Reading from device
				_Logger.WriteLine(3, "Connected.");

				switch (functionCode)
				{
					// Read Coils from Server device (FC=01).
					case 1:
						_SyncScanResult = _Modbus.ReadCoils(
							start, _Converter.ParseRegisterType_Dict(registerType).BitSize);

						result = _Converter.ParseData(
							_SyncScanResult, 0, registerType, multiplier, offset);
						break;

					// Read Discrete Inputs from Server device(FC=02).
					case 2:
						_SyncScanResult = _Modbus.ReadDiscreteInputs(
							start, _Converter.ParseRegisterType_Dict(registerType).BitSize);

						result = _Converter.ParseData(
							_SyncScanResult, 0, registerType, multiplier, offset);
						break;

					// Read Holding Registers from Master device (FC=03).
					case 3:
						_SyncScanResult = Experiment.Core.Utils.Convert(
							_Modbus.ReadHoldingRegisters(
								start, _Converter.ParseRegisterType_Dict(registerType).BitSize / 16));

						result = _Converter.ParseData(
							_SyncScanResult, 0, registerType, multiplier, offset);
						break;

					// Read Input Registers from Master device (FC=04).
					case 4:
						_SyncScanResult = Experiment.Core.Utils.Convert(
							_Modbus.ReadInputRegisters(
								start, _Converter.ParseRegisterType_Dict(registerType).BitSize / 16));

						result = _Converter.ParseData(
							_SyncScanResult, 0, registerType, multiplier, offset);
						break;

					default:
						//throw new NotImplementedException();
						break;
				}

				_Logger.WriteLine(3, $"Function code: {functionCode}, " +
					$"Start: {start}, " +
					$"Register Type: {registerType}, " +
					$"BitSize: {_Converter.ParseRegisterType_Dict(registerType).BitSize}, " +
					$"Multiplier: {multiplier}, " +
					$"Offset: {offset} ");

				_Logger.WriteLine(3, $"Result: {result}");

				_Modbus.Disconnect();

				_Logger.WriteLine(3, "Disconnected.");
			}
			else
			{
				_Logger.WriteLine(3, "Unable to connect to modbus device.");

				throw new Exception("Unable to connect to modbus device.");
			}

			return result;
		}
		public static decimal WriteToDeviceMobile(
			string Host,
			int Port,
			int UnitID,
			int Address,
			int FunctionCode,
			string dataPointId, int Value)
		{
			// extra datapoint's info
			int start = Address;
			//int registerType = int.Parse(device[Defaults.DB_REGISTER_TYPE].ToString());
			int functionCode = FunctionCode;
			int writeValue = Value;

			byte[] _SyncScanResult = { };
			decimal result = 0;

#if FileLogger
			_Logger = new FileLogger(
			DEFAULT_LOG_LEVEL,
			Defaults.DEFAULT_LOG_FOLDER,
			nameof(WriteToDeviceMobile));

			ModbusWrapper _Modbus = new ModbusWrapper(_Logger);

			_Logger.WriteLine(3, "-------------------------------------------------------------------------------------------------------------------------");
			_Logger.WriteLine(3, $"Device. IP Address: {Host}, Port: {Port}, Unit ID: {UnitID}");

			_Modbus.ConnectWrite(Host, Port, UnitID);

			_Converter = new Experiment.Modbus.Converters(_Logger);
#else
			_Converter = new Experiment.Modbus.Converters(new DebugLogger(0));
#endif

			if (_Modbus.IsConnectedWrite)
			{
				// Connected
				// Writing to device
				_Logger.WriteLine(3, "Connected.");

				switch (functionCode)
				{
					// Write Single Coil (FC=05).
					case 5:

						// Convert int to boolean
						bool boolVal;
						switch (writeValue)
						{
							case 0:
								boolVal = false;
								break;
							case 1:
								boolVal = true;
								break;
							default:
								//throw new InvalidOperationException("Integer value is not valid");
								boolVal = true;
								break;
						}

						_Modbus.WriteSingleCoil(start, boolVal);
						break;

					// Write Holding Register (FC=06).
					case 6:
						_Modbus.WriteHoldingRegister(start, writeValue);
						break;

					default:
						//throw new NotImplementedException();
						break;
				}

				/*
				// Write Holding Register to Master device (FC=06).
				_Modbus.WriteHoldingRegister(start, writeValue);
				
				_Logger.WriteLine(3,
					$"DatapointId: {dataPointId}, " +
					$"Function code: {functionCode}, " +
					$"StartAddress: {start}, " +
					$"RegisterType: {registerType}, " +
					$"BitSize: {_Converter.ParseRegisterType_Dict(registerType).BitSize}, " +
					$"Value: {writeValue}");

				_Logger.WriteLine(3, $"Result: {result}");
				*/
				_Modbus.DisconnectWrite();

				_Logger.WriteLine(3, "Disconnected.");
			}
			else
			{
				_Logger.WriteLine(3, "Unable to connect to modbus device.");

				throw new Exception("modbus-error-status-10");
			}

			return result;
		}
	}
}
