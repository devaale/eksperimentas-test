/**
 * WARNING!!!
 *
 * If to enable ENABLE_ASYNC_MODBUS_EVENTS, Experiment.Modbus library behavior will change and synchronous results will be empty!
 * DON'T TOUCH IT IF YOU DON'T KNOW WHAT YOU'RE DOING!
 */
//#define ENABLE_ASYNC_MODBUS_EVENTS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Experiment.Modbus;

using Experiment.Core.BL.Data;
using Experiment.Core.Data;
using Experiment.Core.Metadata;

using Experiment.DeviceScanner.Data;

namespace Experiment.DeviceScanner{
	internal class ModbusScanner
	{
		#region Constants
		const string TYPE_NAME = nameof(ModbusScanner);

		const int THREAD_TIMEOUT = 60; // secs

		#endregion

		#region Attributes

		ScanThreadStateObject _State;
		ModbusWrapper _Modbus;
		string _ScanSessionId;

		private Experiment.Modbus.Converters _Converter;
		static DatapointsCollection _DataPointsInfo;
		static ILogger _Logger;
		static ExpSql _Db;

		Int16[] result;
		byte[] _SyncResult;
		byte[] _AsyncResult;

		#endregion

		internal ModbusScanner(ScanThreadStateObject state, ILogger logger)
		{
			var vLoc = string.Format("{0}/{1}::{2}()", state.DebugThreadId, TYPE_NAME, nameof(ModbusScanner));

			_State = state;
			_SyncResult = null;
			_AsyncResult = null;
			_Logger = logger;
			_Db = ExpSql.GenerateFromDefaults(logger);
			_DataPointsInfo = new DatapointsCollection(_Db);
			_Converter = new Experiment.Modbus.Converters(state.Logger);

			try
			{
				_Modbus = new ModbusWrapper(_State.Logger);
			}
			catch (Exception ex)
			{
				_State.Logger.WriteLine(0, string.Format("{0}, {1}", vLoc, ex.Message));
			}

#if ENABLE_ASYNC_MODBUS_EVENTS
			_Mw.ModbusClient.ReceiveDataChanged += new Experiment.Modbus.ModbusClient.ReceiveDataChangedHandler(UpdateReceiveData);
#endif
		}

		internal void Start()
		{
			var vLoc = string.Format("{0}/{1}::{2}, DeviceId={3}", _State.DebugThreadId, TYPE_NAME, nameof(Start), _State.Device.Id);
			bool everythingOkay = true;

			try
			{
				_State.Logger.WriteLine(5, string.Format("{0}", vLoc));
				ScanSession session = new ScanSession(_State.Db, _State.Device.Id);

				//_ScanSessionId = _State.Db.ScanSessionBegin(_State.Id);
				//_State.Logger.WriteLine(5, string.Format("{0}. _ScanSessionId: {1}", wLocation, _ScanSessionId));

				foreach (ScanDatapoint datapoint in session.ScanDatapoints)
				{
					everythingOkay = Scan(datapoint, session);
					_State.Logger.WriteLine(5, string.Format("{0}. {1}: {2}", vLoc, nameof(everythingOkay), everythingOkay));

					if (!everythingOkay)
					{
						_State.Logger.WriteLine(1, string.Format("{0}. {1}: {2}", vLoc, nameof(everythingOkay), everythingOkay));
						break;
					}

				}

				// if was db level error or some other
				if (everythingOkay)
				{/*
					_State.Db.ScanSessionEnd(
						_ScanSessionId,
						ScanSessionStatus.ScanSuccessfulFinished);*/
				}
				else
				{/*
					_State.Db.ScanSessionStatusSet(
						_ScanSessionId,
						ScanSessionStatus.Error);*/
				}
				//_State.Logger.WriteLine(5, string.Format("{0}. _State.Db.ScanSessionEnd(), _State.Db.ScanSessionStatusSet(). OK", wLocation));
				_State.Logger.WriteLine(5, string.Format("{0}. {1}: {2}", vLoc, nameof(everythingOkay), everythingOkay));
			}
			catch (Exception ex)
			{
				_State.Logger.WriteLine(0, string.Format("{0}. {1}", vLoc, ex.Message));
				everythingOkay = false;
			}
		}

		bool Scan(ScanDatapoint datapoint, ScanSession session)
		{
			var vLoc = string.Format("{0}/{1}::{2}", _State.DebugThreadId, TYPE_NAME, nameof(Scan));
			int byteNum = 0;
			bool retVal = false;
			decimal value;

			try
			{
				_State.Logger.WriteLine(5, string.Format("{0}. Start", vLoc));

				_State.State = ScanThreadState.ReadingFromDb;
				_State.Logger.WriteLine(5, string.Format("{0}. ScanThreadState.ReadingFromDb = _State.State: {1}", vLoc, _State.State));

				_SyncResult = null;
				_State.State = ScanThreadState.ScanningDevice;
				_State.Logger.WriteLine(5, string.Format("{0}. ScanThreadState.ScanningDevice = _State.State: {1}", vLoc, _State.State));

				try {
					_State.Logger.WriteLine(5, string.Format("{0}. _Modbus.Connect(). Start", vLoc));

					_Modbus.Connect(
						session.DeviceInfo.Host,
						session.DeviceInfo.Port,
						session.DeviceInfo.UnitID);

					if (_Modbus.IsConnected)
					{
						_State.Logger.WriteLine(5, string.Format("{0}. _Modbus.Connect(). Connected", vLoc));
						// Connected

						// Reading from device

						// byte number calculation
						switch (datapoint.RegisterType)
						{
							// 1 Bit Registers
							// 1 Bit
							case 1:
								byteNum = 1;
								break;

							// 16 Bit Registers
							// 16 Bit Unsigned
							case 16000: case 16010:
								byteNum = 1;
								break;

							// 32 Bit Registers
							// 32 Bit Unsigned
							case 32000: case 32001: case 32010: case 32011: case 32100: case 32101:
								byteNum = 2;
								break;

							// 64 Bit Registers
							// 64 Bit Unsigned
							case 64000: case 64001: case 64010: case 64011: case 64100: case 64101:
								byteNum = 4;
								break;

							default:
								break;
						}

						// Read data
						switch (datapoint.FunctionCode)
						{
							// Read Coils from Server device (FC1).
							case 1:
								_SyncResult = _Modbus.ReadCoils(datapoint.RegisterAddress, byteNum);
								break;

							// Read Discrete Inputs from Server device(FC2).
							case 2:
								_SyncResult = _Modbus.ReadDiscreteInputs(datapoint.RegisterAddress, byteNum);
								break;

							// Read Holding Registers from Master device (FC3).
							case 3:
								result = _Modbus.ReadHoldingRegisters(datapoint.RegisterAddress, byteNum);
								// Convert int16 to bytes array
								_SyncResult = Core.Utils.Convert(result);
								break;

							// Read Input Registers from Master device (FC4).
							case 4:
								result = _Modbus.ReadInputRegisters(datapoint.RegisterAddress, byteNum);
								// Convert int16 to bytes array
								_SyncResult = Core.Utils.Convert(result);
								break;

							default:
								//throw new NotImplementedException();
								break;
						}

						// Parse data
						value = _Converter.ParseData(_SyncResult,
							0, datapoint.RegisterType, datapoint.Multiplier, datapoint.Offset);

						// Writing the result to DB
						retVal = _State.Db.ScanValueWrite(
							session.DeviceId(),
							datapoint.Id,
							value);

						// Reporting how finished
						_State.Logger.WriteLine(5, string.Format("{0}, Scanned DeviceId: {1}, , Datapoint: {2}, Value: {3}",
							vLoc, session.DeviceId(), datapoint.Id, value));
					}
					else
					{
						// Unable to connect
						_State.State = ScanThreadState.ConnectionError;
						_State.Logger.WriteLine(1, string.Format("{0}, {1} Unable to connect!", vLoc, GetThreadIdAndState()));
					}

					_Modbus.Disconnect();

					_State.Logger.WriteLine(5, string.Format("{0}/_Modbus.Connect()/Finish", vLoc));
				}
				catch (Exception ex)
				{
					_State.Logger.WriteLine(0, string.Format("{0}/_Modbus.Connect()/Error: {1}", vLoc, ex.Message));
				}
			}
			catch (Exception ex)
			{
				_State.Logger.WriteLine(0, string.Format("{0}, {1}", vLoc, ex.Message));
			}
			
			return retVal;
		}

		string receiveData = null;

		void UpdateReceiveData(object sender)
		{
			var vLoc = string.Format("{0}/{1}::{2}", _State.DebugThreadId, TYPE_NAME, nameof(UpdateReceiveData));

			receiveData = "" + BitConverter.ToString(_Modbus.ModbusClient.receiveData).Replace("-", "");
			receiveData = receiveData.Remove(0, 18);
			_AsyncResult = ASCIIEncoding.ASCII.GetBytes(receiveData);

			if (_SyncResult != null)
			{
				_State.Logger.WriteLine(4, string.Format("{0} {1}, RECEIVED: _AsyncResult is NULL!", vLoc, GetThreadIdAndState()));
			}
			else if (_SyncResult == null)
			{
				_State.Logger.WriteLine(4, string.Format("{0} {1}, RECEIVED: _SyncResult is NULL!", vLoc, GetThreadIdAndState()));
			}
			else
			{
				_State.Logger.WriteLine(4, string.Format("{0} {1}, RECEIVED: ALL FINE!", vLoc, GetThreadIdAndState()));
			}

			_Modbus.Disconnect();

			_State.Logger.WriteLine(5, string.Format("{0} {1}, received: [{2}]", vLoc, GetThreadIdAndState(), receiveData));
		}

		public string GetThreadIdAndState()
		{
			return _State.DebugThreadId + " [" + _State.State.ToString() + "]";
		}

	}
}
