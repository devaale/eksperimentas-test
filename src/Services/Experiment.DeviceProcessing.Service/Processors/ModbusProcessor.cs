/**
 * WARNING!!!
 *
 * If to enable ENABLE_ASYNC_MODBUS_EVENTS, Experiment.Modbus library behavior will change and synchronous results will be empty!
 * DON'T TOUCH IT IF YOU DON'T KNOW WHAT YOU'RE DOING!
 */
//#define ENABLE_ASYNC_MODBUS_EVENTS
using System;
using System.Text;
using System.Threading.Tasks;

using Experiment.Modbus;

using Experiment.Core;
using Experiment.Core.Metadata;
using Experiment.Data.Models;

using Experiment.DeviceProcessing.Service.Data;

namespace Experiment.DeviceProcessing.Service.Processors{
	internal class ModbusProcessor : IDeviceProcessor
	{
		private const string TYPE_NAME = nameof(ModbusProcessor);

		private const int DEFAULT_DEVICE_PORT = 502;

		private readonly ThreadStateObject _state;
		private readonly ModbusWrapper _modbus;
		private readonly Experiment.Modbus.Converters _converter;

		private byte[] _syncResult;
		private byte[] _asyncResult;
		private string _receiveData;

		internal ModbusProcessor(ThreadStateObject state, ILogger logger)
		{
			var vLoc = $"{state.DebugThreadId}/{TYPE_NAME}::{nameof(ModbusProcessor)}()";
			state?.Logger.WriteLine(5, vLoc);

			_state = state ?? throw new ArgumentNullException(nameof(state));
			_syncResult = null;
			_asyncResult = null;
			_converter = new Experiment.Modbus.Converters(state.Logger);

			try
			{
				_modbus = new ModbusWrapper(_state.Logger);
			}
			catch (Exception ex)
			{
				_modbus = null;
				_state.Logger.WriteLine(0, $"{vLoc}, {ex.Message}");
			}

#if ENABLE_ASYNC_MODBUS_EVENTS
			_modbus?.ModbusClient.ReceiveDataChanged += new Experiment.Modbus.ModbusClient.ReceiveDataChangedHandler(UpdateReceiveData);
#endif
		}

		public Task StartAsync()
		{
			StartInternal();
			return Task.CompletedTask;
		}

		private void StartInternal()
		{
			var vLoc = $"{BuildLocation(nameof(StartInternal))}, DeviceId={_state.Device.Id}, NumDatapoints={_state.Device.Datapoints?.Count}";
			_state.Logger.WriteLine(5, vLoc);

			var everythingOkay = true;

			try
			{
				foreach (var datapoint in _state.Device.Datapoints)
				{
					everythingOkay = Scan(datapoint);

					_state.Logger.WriteLine(5, $"{vLoc}. {nameof(everythingOkay)}: {everythingOkay}");

					if (!everythingOkay)
					{
						_state.Logger.WriteLine(1, $"{vLoc}. {nameof(everythingOkay)}: {everythingOkay}");
						break;
					}
				}

				_state.Logger.WriteLine(5, $"{vLoc}. {nameof(everythingOkay)}: {everythingOkay}");
			}
			catch (Exception ex)
			{
				_state.Logger.WriteLine(0, $"{vLoc}. {ex.Message}");
				everythingOkay = false;
			}
		}

		private bool Scan(Datapoint datapoint)
		{
			var vLoc = BuildLocation(nameof(Scan));
			_state.Logger.WriteLine(5, vLoc);

			try
			{
				_state.Logger.WriteLine(5, $"{vLoc}. Start");

				if (datapoint == null)
				{
					_state.Logger.WriteLine(1, $"{vLoc}. Datapoint is null.");
					return false;
				}

				SetThreadState(Data.ThreadState.ReadingFromDb, vLoc, "ScanThreadState.ReadingFromDb");
				_syncResult = null;
				SetThreadState(Data.ThreadState.ScanningDevice, vLoc, "ScanThreadState.ScanningDevice");

				if (!EnsureModbusInitialized(vLoc) || !HasDeviceUrl(vLoc))
				{
					return false;
				}

				return ExecuteScan(datapoint, vLoc);
			}
			catch (Exception ex)
			{
				_state.Logger.WriteLine(0, $"{vLoc}, {ex.Message}");
			}

			return false;
		}

		private bool ExecuteScan(Datapoint datapoint, string vLoc)
		{
			var retVal = false;

			try
			{
				_state.Logger.WriteLine(5, $"{vLoc}. _Modbus.Connect(). Start");

				var urlMan = new UrlMan(DEFAULT_DEVICE_PORT, _state.Device.Url);

				_modbus.Connect(urlMan.Host, urlMan.Port.Value, _state.Device.UnitId);

				if (_modbus.IsConnected)
				{
					_state.Logger.WriteLine(5, $"{vLoc}. _Modbus.Connect(). Connected");

					var byteNum = CalculateByteCount(datapoint.RegisterType);
					_syncResult = ReadRegisters(datapoint, byteNum);

					var value = _converter.ParseData(_syncResult, 0, datapoint.RegisterType, datapoint.Multiplier, datapoint.Offset);

					retVal = _state.Db.ScanValueWrite(_state.Device.Id, datapoint.Id, value);

					_state.Logger.WriteLine(5, $"{vLoc}, Scanned DeviceId: {_state.Device.Id}, , Datapoint: {datapoint.Id}, Value: {value}");
				}
				else
				{
					_state.State = Data.ThreadState.ConnectionError;
					_state.Logger.WriteLine(1, $"{vLoc}, {GetThreadIdAndState()} Unable to connect!");
				}
			}
			catch (Exception ex)
			{
				_state.Logger.WriteLine(0, $"{vLoc}/_Modbus.Connect()/Error: {ex.Message}");
			}
			finally
			{
				try
				{
					_modbus.Disconnect();
				}
				catch (Exception disconnectEx)
				{
					_state.Logger.WriteLine(0, $"{vLoc}/_Modbus.Connect()/Error: {disconnectEx.Message}");
				}

				_state.Logger.WriteLine(5, $"{vLoc}/_Modbus.Connect()/Finish");
			}

			return retVal;
		}

		private bool EnsureModbusInitialized(string vLoc)
		{
			if (_modbus != null)
			{
				return true;
			}

			_state.Logger.WriteLine(0, $"{vLoc}. ModbusWrapper is not initialized");
			return false;
		}

		private bool HasDeviceUrl(string vLoc)
		{
			if (_state.Device.Url != null)
			{
				return true;
			}

			_state.Logger.WriteLine(1, $"{vLoc}. Device URL is missing.");
			return false;
		}

		private void SetThreadState(Data.ThreadState newState, string vLoc, string stateLabel)
		{
			_state.State = newState;
			_state.Logger.WriteLine(5, $"{vLoc}. {stateLabel} = _State.State: {_state.State}");
		}

		private string BuildLocation(string methodName)
		{
			return $"{_state.DebugThreadId}/{TYPE_NAME}::{methodName}";
		}

		private void UpdateReceiveData(object sender)
		{
			var vLoc = BuildLocation(nameof(UpdateReceiveData));
			_state.Logger.WriteLine(5, vLoc);

			_receiveData = "" + BitConverter.ToString(_modbus.ModbusClient.receiveData).Replace("-", "");
			_receiveData = _receiveData.Remove(0, 18);
			_asyncResult = Encoding.ASCII.GetBytes(_receiveData);

			if (_syncResult != null)
			{
				_state.Logger.WriteLine(4, $"{vLoc} {GetThreadIdAndState()}, RECEIVED: _AsyncResult is NULL!");
			}
			else if (_syncResult == null)
			{
				_state.Logger.WriteLine(4, $"{vLoc} {GetThreadIdAndState()}, RECEIVED: _SyncResult is NULL!");
			}
			else
			{
				_state.Logger.WriteLine(4, $"{vLoc} {GetThreadIdAndState()}, RECEIVED: ALL FINE!");
			}

			_modbus.Disconnect();

			_state.Logger.WriteLine(5, $"{vLoc} {GetThreadIdAndState()}, received: [{_receiveData}]");
		}

		public string GetThreadIdAndState()
		{
			return $"{_state.DebugThreadId} [{_state.State}]";
		}

		private static int CalculateByteCount(int registerType)
		{
			switch (registerType)
			{
				case 1:
				case 16000:
				case 16010:
					return 1;

				case 32000:
				case 32001:
				case 32010:
				case 32011:
				case 32100:
				case 32101:
					return 2;

				case 64000:
				case 64001:
				case 64010:
				case 64011:
				case 64100:
				case 64101:
					return 4;

				default:
					return 0;
			}
		}

		private byte[] ReadRegisters(Datapoint datapoint, int byteNum)
		{
			Int16[] registers;

			switch (datapoint.FunctionCode)
			{
				case 1:
					return _modbus.ReadCoils(datapoint.RegisterAddress, byteNum);

				case 2:
					return _modbus.ReadDiscreteInputs(datapoint.RegisterAddress, byteNum);

				case 3:
					registers = _modbus.ReadHoldingRegisters(datapoint.RegisterAddress, byteNum);
					return Core.Utils.Convert(registers);

				case 4:
					registers = _modbus.ReadInputRegisters(datapoint.RegisterAddress, byteNum);
					return Core.Utils.Convert(registers);

				default:
					return null;
			}
		}
	}
}
