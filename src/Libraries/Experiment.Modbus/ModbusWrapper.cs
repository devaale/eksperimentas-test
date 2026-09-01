using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Experiment.Core.IO;
using Experiment.Core.Metadata;

namespace Experiment.Modbus{
	public class ModbusWrapper : IConnectable
	{
		#region Constants

		#endregion

		#region Attributes
		ILogger _Logger;

		#endregion

		#region Properties
		public Experiment.Modbus.ModbusClient ModbusClient { get; protected set; }

		public ModbusLibrary.ModbusWrite ModbusWrite { get; protected set; }

		public bool IsConnected
		{
			get
			{
				if(ModbusClient !=null)
				{
					return ModbusClient.Connected;
				}
				return false;
			}
		}

		public bool IsConnectedWrite
		{
			get
			{
				if (ModbusWrite != null)
				{
					return ModbusWrite.Connd;
				}
				return false;
			}
		}

		#endregion

		#region Ctor
		public ModbusWrapper(ILogger logger)
		{
			_Logger = logger;

			ModbusClient = new Experiment.Modbus.ModbusClient(logger);
			ModbusWrite = new ModbusLibrary.ModbusWrite();
		}

		#endregion

		#region Events

		#endregion

		#region Methods

		#region Connect/Disconnect

		public bool Connect(string host, int port, int unitId)
		{
			ModbusClient.IPAddress = host;
			ModbusClient.Port = port;

			try
			{
				ModbusClient.Connect(host, port, unitId);
			}
			catch (Exception ex)
			{
				_Logger.WriteLine(3, ex.Message);
			}

			return IsConnected;
		}

		public bool ConnectWrite(string host, int port, int unitId)
		{
			ModbusWrite.IP = host;
			ModbusWrite.Port = port;

			try
			{
				ModbusWrite.Conn(host, port, unitId);
			}
			catch (Exception ex)
			{
				_Logger.WriteLine(3, ex.Message);
			}

			return IsConnectedWrite;
		}

		public void Disconnect()
		{
			if (IsConnected)
				ModbusClient.Disconnect();
		}

		public void DisconnectWrite()
		{
			if (IsConnectedWrite)
				ModbusWrite.Diss();
		}

		#endregion

		public byte[] ReadCoils(int cellAddr, int num)
		{
			return ModbusClient.ReadCoils(cellAddr - 1, num);
		}

		public byte[] ReadDiscreteInputs(int cellAddr, int num)
		{
			return ModbusClient.ReadDiscreteInputs(cellAddr - 1, num);
		}

		public Int16[] ReadHoldingRegisters(int cellAddr, int num)
		{
			return ModbusClient.ReadHoldingRegisters(cellAddr - 1, num);
		}

		public Int16[] ReadInputRegisters(int cellAddr, int num)
		{
			return ModbusClient.ReadInputRegisters(cellAddr - 1, num);
		}

		public void WriteSingleCoil(int cellAddr, bool num)
		{
			ModbusWrite.WriteSingleCoil(cellAddr - 1, num);
		}

		public void WriteHoldingRegister(int cellAddr, int num)
		{
			ModbusWrite.WriteHoldingRegister(cellAddr - 1, num);
		}

		#endregion
	}
}
