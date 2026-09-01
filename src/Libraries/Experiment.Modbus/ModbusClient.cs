//#define UpdateSysVars // UpdateLoggerVars. Enabled: Vars form DB, Disabled: Default Vars

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

using Experiment.Core;
using Experiment.Core.IO;
using Experiment.Core.BL.Data;
using Experiment.Core.BL.Data.SysVars;
using Experiment.Core.Metadata;

namespace Experiment.Modbus{
	/// <summary>
	/// Implements a ModbusClient.
	/// </summary>
	public partial class ModbusClient
	{
		ILogger _Logger;
		ExpSql _Db;
		IDictionary<SysVarName, object> _Vars = new Dictionary<SysVarName, object>();

		private bool debug = false;
		private TcpClient tcpClient;
		private string ipAddress = "127.0.0.1";
		private int port = 502;
		private uint transactionIdentifierInternal = 0;
		private byte[] transactionIdentifier = new byte[2];
		private byte[] protocolIdentifier = new byte[2];
		private byte[] crc = new byte[2];
		private byte[] length = new byte[2];
		private byte unitIdentifier = 0x01;
		private byte functionCode;
		private byte[] startingAddress = new byte[2];
		private byte[] quantity = new byte[2];
		private bool udpFlag = false;
		private int portOut;
		private int connectTimeout = 1000;
		public byte[] receiveData;
		public byte[] sendData;
		private bool connected = false;
		public int NumberOfRetries { get; set; } = 3;
		private int countRetries = 0;

		public delegate void ReceiveDataChangedHandler(object sender);
		public event ReceiveDataChangedHandler ReceiveDataChanged;

		public delegate void SendDataChangedHandler(object sender);
		public event SendDataChangedHandler SendDataChanged;

		public delegate void ConnectedChangedHandler(object sender);
		public event ConnectedChangedHandler ConnectedChanged;

		NetworkStream stream;

		/// <summary>
		/// Parameterless constructor
		/// </summary>
		public ModbusClient(ILogger logger)
		{
			Debug.Assert(logger != null,
				"ModbusClient::CTOR: logger can't be NULL!");

			_Logger = logger;

#if UpdateSysVars
			_Db = ExpSql.GenerateFromDefaults(logger);
			UpdateSysVars();
#endif

			if (debug) StoreLogData.Instance.Store("Experiment.Modbus library initialized for Experiment.Modbus-TCP", System.DateTime.Now);
		}

		void UpdateSysVars()
		{
			_Vars = _Db.SysVarsGet(SysVarModule.Scan);

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

		/// <summary>
		/// Establish connection to Master device in case of Experiment.Modbus TCP.
		/// </summary>
		public void Connect(string ipAddress, int port, int unitId)
		{
			if (!udpFlag)
			{
				if (debug) _Logger.WriteLine(3, $"ModbusClient::Connect() Open TCP-Socket, IP Address: {ipAddress}, Port: {port}, UnitID: {unitId}");
				if (debug) StoreLogData.Instance.Store("Open TCP-Socket, IP Address: " + ipAddress + ", Port: " + port + ", UnitID" + unitId, System.DateTime.Now);
				tcpClient = new TcpClient();
				var result = tcpClient.BeginConnect(ipAddress, port, null, null);
				var success = result.AsyncWaitHandle.WaitOne(connectTimeout);
				if (!success)
				{
					throw new Experiment.Modbus.Exceptions.ConnectionException("connection timed out");
				}
				tcpClient.EndConnect(result);

				//tcpClient = new TcpClient(ipAddress, port);
				stream = tcpClient.GetStream();
				stream.ReadTimeout = connectTimeout;
				unitIdentifier = Convert.ToByte(unitId);
				UnitID = unitId;
				connected = true;
			}
			else
			{
				tcpClient = new TcpClient();
				connected = true;
			}

			if (ConnectedChanged != null)
				ConnectedChanged(this);
		}

		/// <summary>
		/// Calculates the CRC16 for Experiment.Modbus-RTU
		/// </summary>
		/// <param name="data">Byte buffer to send</param>
		/// <param name="numberOfBytes">Number of bytes to calculate CRC</param>
		/// <param name="startByte">First byte in buffer to start calculating CRC</param>
		public static UInt16 calculateCRC(byte[] data, UInt16 numberOfBytes, int startByte)
		{
			byte[] auchCRCHi = {
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
			0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01,
			0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81,
			0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01,
			0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
			0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01,
			0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
			0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01,
			0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
			0x40
			};

			byte[] auchCRCLo = {
			0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7, 0x05, 0xC5, 0xC4,
			0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
			0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD,
			0x1D, 0x1C, 0xDC, 0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
			0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32, 0x36, 0xF6, 0xF7,
			0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
			0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE,
			0x2E, 0x2F, 0xEF, 0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
			0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1, 0x63, 0xA3, 0xA2,
			0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
			0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB,
			0x7B, 0x7A, 0xBA, 0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
			0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0, 0x50, 0x90, 0x91,
			0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
			0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88,
			0x48, 0x49, 0x89, 0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
			0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83, 0x41, 0x81, 0x80,
			0x40
			};
			UInt16 usDataLen = numberOfBytes;
			byte uchCRCHi = 0xFF;
			byte uchCRCLo = 0xFF;
			int i = 0;
			int uIndex;
			while (usDataLen > 0)
			{
				usDataLen--;
				if ((i + startByte) < data.Length)
				{
					uIndex = uchCRCLo ^ data[i + startByte];
					uchCRCLo = (byte)(uchCRCHi ^ auchCRCHi[uIndex]);
					uchCRCHi = auchCRCLo[uIndex];
				}
				i++;
			}
			return (UInt16)((UInt16)uchCRCHi << 8 | uchCRCLo);
		}

		private bool dataReceived = false;
		private bool receiveActive = false;
		private byte[] readBuffer = new byte[256];
		private int bytesToRead = 0;

		/// <summary>
		/// Read Coils from Server device (FC=01).
		/// </summary>
		/// <param name="startingAddress">First coil to read</param>
		/// <param name="quantity">Numer of coils to read</param>
		/// <returns>Boolean Array which contains the coils</returns>
		public Byte[] ReadCoils(int startingAddress, int quantity)
		{
			if (debug) StoreLogData.Instance.Store("FC1 (Read Coils from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity, System.DateTime.Now);
			transactionIdentifierInternal++;
			if (tcpClient == null & !udpFlag)
			{
				if (debug) StoreLogData.Instance.Store("ConnectionException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.ConnectionException("connection error");
			}
			if (startingAddress > 65535 | quantity > 2000)
			{
				if (debug) StoreLogData.Instance.Store("ArgumentException Throwed", System.DateTime.Now);
				throw new ArgumentException("Starting address must be 0 - 65535; quantity must be 0 - 2000");
			}
			bool[] response;
			Byte[] responseByte;
			Int16[] responseInt16;
			this.transactionIdentifier = BitConverter.GetBytes((uint)transactionIdentifierInternal);
			this.protocolIdentifier = BitConverter.GetBytes((int)0x0000);
			this.length = BitConverter.GetBytes((int)0x0006);
			this.functionCode = 0x01;
			this.startingAddress = BitConverter.GetBytes(startingAddress);
			this.quantity = BitConverter.GetBytes(quantity);
			Byte[] data = new byte[]{
							this.transactionIdentifier[1],
							this.transactionIdentifier[0],
							this.protocolIdentifier[1],
							this.protocolIdentifier[0],
							this.length[1],
							this.length[0],
							this.unitIdentifier,
							this.functionCode,
							this.startingAddress[1],
							this.startingAddress[0],
							this.quantity[1],
							this.quantity[0],
							this.crc[0],
							this.crc[1]
			};

			crc = BitConverter.GetBytes(calculateCRC(data, 6, 6));
			data[12] = crc[0];
			data[13] = crc[1];
			if (tcpClient.Client.Connected | udpFlag)
			{
				if (udpFlag)
				{
					UdpClient udpClient = new UdpClient();
					IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
					udpClient.Send(data, data.Length - 2, endPoint);
					portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
					udpClient.Client.ReceiveTimeout = 5000;
					endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
					data = udpClient.Receive(ref endPoint);
				}
				else
				{
					stream.Write(data, 0, data.Length - 2);
					if (debug)
					{
						byte[] debugData = new byte[data.Length - 2];
						Array.Copy(data, 0, debugData, 0, data.Length - 2);
						if (debug) StoreLogData.Instance.Store("Send MocbusTCP-Data: " + BitConverter.ToString(debugData), System.DateTime.Now);
					}
					if (SendDataChanged != null)
					{
						sendData = new byte[data.Length - 2];
						Array.Copy(data, 0, sendData, 0, data.Length - 2);
						SendDataChanged(this);

					}
					data = new Byte[2100];
					int NumberOfBytes = stream.Read(data, 0, data.Length);
					if (ReceiveDataChanged != null)
					{
						receiveData = new byte[NumberOfBytes];
						Array.Copy(data, 0, receiveData, 0, NumberOfBytes);
						if (debug) StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(receiveData), System.DateTime.Now);
						ReceiveDataChanged(this);
					}
				}
			}
			if (data[7] == 0x81 & data[8] == 0x01)
			{
				if (debug) StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.FunctionCodeNotSupportedException("Function code not supported by master");
			}
			if (data[7] == 0x81 & data[8] == 0x02)
			{
				if (debug) StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
			}
			if (data[7] == 0x81 & data[8] == 0x03)
			{
				if (debug) StoreLogData.Instance.Store("QuantityInvalidException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.QuantityInvalidException("quantity invalid");
			}
			if (data[7] == 0x81 & data[8] == 0x04)
			{
				if (debug) StoreLogData.Instance.Store("ModbusException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.ModbusException("error reading");
			}
			response = new bool[quantity];
			responseByte = new byte[quantity];
			responseInt16 = new Int16[quantity];
			for (int i = 0; i < quantity; i++)
			{
				int intData = data[9 + i / 8];
				int mask = Convert.ToInt32(Math.Pow(2, (i % 8)));
				response[i] = Convert.ToBoolean((intData & mask) / mask);
				responseByte[i] = Convert.ToByte(response[i]);
				responseInt16[i] = Convert.ToInt16(response[i]);
			}
			return (responseByte);
		}

		/// <summary>
		/// Read Discrete Inputs from Server device (FC=02).
		/// </summary>
		/// <param name="startingAddress">First discrete input to read</param>
		/// <param name="quantity">Number of discrete Inputs to read</param>
		/// <returns>Boolean Array which contains the discrete Inputs</returns>
		public Byte[] ReadDiscreteInputs(int startingAddress, int quantity)
		{
			if (debug) StoreLogData.Instance.Store("FC2 (Read Discrete Inputs from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity, System.DateTime.Now);
			transactionIdentifierInternal++;
			if (tcpClient == null & !udpFlag)
			{
				if (debug) StoreLogData.Instance.Store("ConnectionException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.ConnectionException("connection error");
			}
			if (startingAddress > 65535 | quantity > 2000)
			{
				if (debug) StoreLogData.Instance.Store("ArgumentException Throwed", System.DateTime.Now);
				throw new ArgumentException("Starting address must be 0 - 65535; quantity must be 0 - 2000");
			}
			bool[] response;
			Byte[] responseByte;
			this.transactionIdentifier = BitConverter.GetBytes((uint)transactionIdentifierInternal);
			this.protocolIdentifier = BitConverter.GetBytes((int)0x0000);
			this.length = BitConverter.GetBytes((int)0x0006);
			this.functionCode = 0x02;
			this.startingAddress = BitConverter.GetBytes(startingAddress);
			this.quantity = BitConverter.GetBytes(quantity);
			Byte[] data = new byte[]
							{
							this.transactionIdentifier[1],
							this.transactionIdentifier[0],
							this.protocolIdentifier[1],
							this.protocolIdentifier[0],
							this.length[1],
							this.length[0],
							this.unitIdentifier,
							this.functionCode,
							this.startingAddress[1],
							this.startingAddress[0],
							this.quantity[1],
							this.quantity[0],
							this.crc[0],
							this.crc[1]
							};
			crc = BitConverter.GetBytes(calculateCRC(data, 6, 6));
			data[12] = crc[0];
			data[13] = crc[1];

			if (tcpClient.Client.Connected | udpFlag)
			{
				if (udpFlag)
				{
					UdpClient udpClient = new UdpClient();
					IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
					udpClient.Send(data, data.Length - 2, endPoint);
					portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
					udpClient.Client.ReceiveTimeout = 5000;
					endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
					data = udpClient.Receive(ref endPoint);
				}
				else
				{
					stream.Write(data, 0, data.Length - 2);
					if (debug)
					{
						byte[] debugData = new byte[data.Length - 2];
						Array.Copy(data, 0, debugData, 0, data.Length - 2);
						if (debug) StoreLogData.Instance.Store("Send ModbusTCP-Data: " + BitConverter.ToString(debugData), System.DateTime.Now);
					}
					if (SendDataChanged != null)
					{
						sendData = new byte[data.Length - 2];
						Array.Copy(data, 0, sendData, 0, data.Length - 2);
						SendDataChanged(this);
					}
					data = new Byte[2100];
					int NumberOfBytes = stream.Read(data, 0, data.Length);
					if (ReceiveDataChanged != null)
					{
						receiveData = new byte[NumberOfBytes];
						Array.Copy(data, 0, receiveData, 0, NumberOfBytes);
						if (debug) StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(receiveData), System.DateTime.Now);
						ReceiveDataChanged(this);
					}
				}
			}
			if (data[7] == 0x82 & data[8] == 0x01)
			{
				if (debug) StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.FunctionCodeNotSupportedException("Function code not supported by master");
			}
			if (data[7] == 0x82 & data[8] == 0x02)
			{
				if (debug) StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
			}
			if (data[7] == 0x82 & data[8] == 0x03)
			{
				if (debug) StoreLogData.Instance.Store("QuantityInvalidException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.QuantityInvalidException("quantity invalid");
			}
			if (data[7] == 0x82 & data[8] == 0x04)
			{
				if (debug) StoreLogData.Instance.Store("ModbusException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.ModbusException("error reading");
			}
			response = new bool[quantity];
			responseByte = new Byte[quantity];
			for (int i = 0; i < quantity; i++)
			{
				int intData = data[9 + i / 8];
				int mask = Convert.ToInt32(Math.Pow(2, (i % 8)));
				response[i] = Convert.ToBoolean((intData & mask) / mask);
				responseByte[i] = Convert.ToByte(response[i]);
			}
			return (responseByte);
		}

		/// <summary>
		/// Read Holding Registers from Master device (FC=03).
		/// </summary>
		/// <param name="startingAddress">First holding register to be read</param>
		/// <param name="quantity">Number of holding registers to be read</param>
		/// <returns>Int Array which contains the holding registers</returns>
		public Int16[] ReadHoldingRegisters(int startingAddress, int quantity)
		{
			if (debug) StoreLogData.Instance.Store("FC3 (Read Holding Registers from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity, System.DateTime.Now);
			transactionIdentifierInternal++;
			if (tcpClient == null & !udpFlag)
			{
				if (debug) StoreLogData.Instance.Store("ConnectionException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.ConnectionException("connection error");
			}
			if (startingAddress > 65535 | quantity > 125)
			{
				if (debug) StoreLogData.Instance.Store("ArgumentException Throwed", System.DateTime.Now);
				throw new ArgumentException("Starting address must be 0 - 65535; quantity must be 0 - 125");
			}
			Int16[] response;
			this.transactionIdentifier = BitConverter.GetBytes((uint)transactionIdentifierInternal);
			this.protocolIdentifier = BitConverter.GetBytes((int)0x0000);
			this.length = BitConverter.GetBytes((int)0x0006);
			this.functionCode = 0x03;
			this.startingAddress = BitConverter.GetBytes(startingAddress);
			this.quantity = BitConverter.GetBytes(quantity);
			Byte[] data = new byte[]{   this.transactionIdentifier[1],
							this.transactionIdentifier[0],
							this.protocolIdentifier[1],
							this.protocolIdentifier[0],
							this.length[1],
							this.length[0],
							this.unitIdentifier,
							this.functionCode,
							this.startingAddress[1],
							this.startingAddress[0],
							this.quantity[1],
							this.quantity[0],
							this.crc[0],
							this.crc[1]
			};
			crc = BitConverter.GetBytes(calculateCRC(data, 6, 6));
			data[12] = crc[0];
			data[13] = crc[1];
			if (tcpClient.Client.Connected | udpFlag)
			{
				if (udpFlag)
				{
					UdpClient udpClient = new UdpClient();
					IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
					udpClient.Send(data, data.Length - 2, endPoint);
					portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
					udpClient.Client.ReceiveTimeout = 5000;
					endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
					data = udpClient.Receive(ref endPoint);
				}
				else
				{
					stream.Write(data, 0, data.Length - 2);
					if (debug)
					{
						byte[] debugData = new byte[data.Length - 2];
						Array.Copy(data, 0, debugData, 0, data.Length - 2);
						if (debug) StoreLogData.Instance.Store("Send ModbusTCP-Data: " + BitConverter.ToString(debugData), System.DateTime.Now);
					}
					if (SendDataChanged != null)
					{
						sendData = new byte[data.Length - 2];
						Array.Copy(data, 0, sendData, 0, data.Length - 2);
						SendDataChanged(this);

					}
					data = new Byte[256];
					int NumberOfBytes = stream.Read(data, 0, data.Length);
					if (ReceiveDataChanged != null)
					{
						receiveData = new byte[NumberOfBytes];
						Array.Copy(data, 0, receiveData, 0, NumberOfBytes);
						if (debug) StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(receiveData), System.DateTime.Now);
						ReceiveDataChanged(this);
					}
				}
			}
			if (data[7] == 0x83 & data[8] == 0x01)
			{
				if (debug) StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.FunctionCodeNotSupportedException("Function code not supported by master");
			}
			if (data[7] == 0x83 & data[8] == 0x02)
			{
				if (debug) StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
			}
			if (data[7] == 0x83 & data[8] == 0x03)
			{
				if (debug) StoreLogData.Instance.Store("QuantityInvalidException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.QuantityInvalidException("quantity invalid");
			}
			if (data[7] == 0x83 & data[8] == 0x04)
			{
				if (debug) StoreLogData.Instance.Store("ModbusException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.ModbusException("error reading");
			}
			response = new Int16[quantity];
			for (int i = 0; i < quantity; i++)
			{
				byte lowByte;
				byte highByte;
				highByte = data[9 + i * 2];
				lowByte = data[9 + i * 2 + 1];

				data[9 + i * 2] = lowByte;
				data[9 + i * 2 + 1] = highByte;

				response[i] = BitConverter.ToInt16(data, (9 + i * 2));
			}
			return (response);
		}

		/// <summary>
		/// Read Input Registers from Master device (FC=04).
		/// </summary>
		/// <param name="startingAddress">First input register to be read</param>
		/// <param name="quantity">Number of input registers to be read</param>
		/// <returns>Int Array which contains the input registers</returns>
		public Int16[] ReadInputRegisters(int startingAddress, int quantity)
		{

			if (debug) StoreLogData.Instance.Store("FC4 (Read Input Registers from Master device), StartingAddress: " + startingAddress + ", Quantity: " + quantity, System.DateTime.Now);
			transactionIdentifierInternal++;
			if (tcpClient == null & !udpFlag)
			{
				if (debug) StoreLogData.Instance.Store("ConnectionException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.ConnectionException("connection error");
			}
			if (startingAddress > 65535 | quantity > 125)
			{
				if (debug) StoreLogData.Instance.Store("ArgumentException Throwed", System.DateTime.Now);
				throw new ArgumentException("Starting address must be 0 - 65535; quantity must be 0 - 125");
			}
			Int16[] response;
			this.transactionIdentifier = BitConverter.GetBytes((uint)transactionIdentifierInternal);
			this.protocolIdentifier = BitConverter.GetBytes((int)0x0000);
			this.length = BitConverter.GetBytes((int)0x0006);
			this.functionCode = 0x04;
			this.startingAddress = BitConverter.GetBytes(startingAddress);
			this.quantity = BitConverter.GetBytes(quantity);
			Byte[] data = new byte[]{   this.transactionIdentifier[1],
							this.transactionIdentifier[0],
							this.protocolIdentifier[1],
							this.protocolIdentifier[0],
							this.length[1],
							this.length[0],
							this.unitIdentifier,
							this.functionCode,
							this.startingAddress[1],
							this.startingAddress[0],
							this.quantity[1],
							this.quantity[0],
							this.crc[0],
							this.crc[1]
			};
			crc = BitConverter.GetBytes(calculateCRC(data, 6, 6));
			data[12] = crc[0];
			data[13] = crc[1];
			if (tcpClient.Client.Connected | udpFlag)
			{
				if (udpFlag)
				{
					UdpClient udpClient = new UdpClient();
					IPEndPoint endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port);
					udpClient.Send(data, data.Length - 2, endPoint);
					portOut = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
					udpClient.Client.ReceiveTimeout = 5000;
					endPoint = new IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portOut);
					data = udpClient.Receive(ref endPoint);
				}
				else
				{
					stream.Write(data, 0, data.Length - 2);
					if (debug)
					{
						byte[] debugData = new byte[data.Length - 2];
						Array.Copy(data, 0, debugData, 0, data.Length - 2);
						if (debug) StoreLogData.Instance.Store("Send ModbusTCP-Data: " + BitConverter.ToString(debugData), System.DateTime.Now);
					}
					if (SendDataChanged != null)
					{
						sendData = new byte[data.Length - 2];
						Array.Copy(data, 0, sendData, 0, data.Length - 2);
						SendDataChanged(this);
					}
					data = new Byte[2100];
					int NumberOfBytes = stream.Read(data, 0, data.Length);
					if (ReceiveDataChanged != null)
					{
						receiveData = new byte[NumberOfBytes];
						Array.Copy(data, 0, receiveData, 0, NumberOfBytes);
						if (debug) StoreLogData.Instance.Store("Receive ModbusTCP-Data: " + BitConverter.ToString(receiveData), System.DateTime.Now);
						ReceiveDataChanged(this);
					}
				}
			}
			if (data[7] == 0x84 & data[8] == 0x01)
			{
				if (debug) StoreLogData.Instance.Store("FunctionCodeNotSupportedException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.FunctionCodeNotSupportedException("Function code not supported by master");
			}
			if (data[7] == 0x84 & data[8] == 0x02)
			{
				if (debug) StoreLogData.Instance.Store("StartingAddressInvalidException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.StartingAddressInvalidException("Starting address invalid or starting address + quantity invalid");
			}
			if (data[7] == 0x84 & data[8] == 0x03)
			{
				if (debug) StoreLogData.Instance.Store("QuantityInvalidException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.QuantityInvalidException("quantity invalid");
			}
			if (data[7] == 0x84 & data[8] == 0x04)
			{
				if (debug) StoreLogData.Instance.Store("ModbusException Throwed", System.DateTime.Now);
				throw new Experiment.Modbus.Exceptions.ModbusException("error reading");
			}
			response = new Int16[quantity];
			for (int i = 0; i < quantity; i++)
			{
				byte lowByte;
				byte highByte;
				highByte = data[9 + i * 2];
				lowByte = data[9 + i * 2 + 1];

				data[9 + i * 2] = lowByte;
				data[9 + i * 2 + 1] = highByte;

				response[i] = BitConverter.ToInt16(data, (9 + i * 2));
			}
			return (response);
		}

		/// <summary>
		/// Close connection to Master Device.
		/// </summary>
		public void Disconnect()
		{
			if (debug) StoreLogData.Instance.Store("Disconnect", System.DateTime.Now);
			if (stream != null)
				stream.Close();
			if (tcpClient != null)
				tcpClient.Close();
			connected = false;
			if (ConnectedChanged != null)
				ConnectedChanged(this);

		}

		/// <summary>
		/// Destructor - Close connection to Master Device.
		/// </summary>
		~ModbusClient()
		{
			if (debug) StoreLogData.Instance.Store("Destructor called - automatically disconnect", System.DateTime.Now);
			if (tcpClient != null & !udpFlag)
			{
				if (stream != null)
					stream.Close();
				tcpClient.Close();
			}
		}

		/// <summary>
		/// Returns "TRUE" if Client is connected to Server and "FALSE" if not. In case of Experiment.Modbus RTU returns if COM-Port is opened
		/// </summary>
		public bool Connected
		{
			get
			{
				if (udpFlag & tcpClient != null)
					return true;
				if (tcpClient == null)
					return false;
				else
				{
					return connected;

				}

			}
		}

		public bool Available(int timeout)
		{
			// Ping's the local machine.
			System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
			IPAddress address = System.Net.IPAddress.Parse(ipAddress);

			// Create a buffer of 32 bytes of data to be transmitted.
			string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
			byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data);

			// Wait 10 seconds for a reply.
			System.Net.NetworkInformation.PingReply reply = pingSender.Send(address, timeout, buffer);

			if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets or Sets the IP-Address of the Server.
		/// </summary>
		public string IPAddress
		{
			get
			{
				return ipAddress;
			}
			set
			{
				ipAddress = value;
			}
		}

		/// <summary>
		/// Gets or Sets the Port were the Experiment.Modbus-TCP Server is reachable (Standard is 502).
		/// </summary>
		public int Port
		{
			get
			{
				return port;
			}
			set
			{
				port = value;
			}
		}

		/// <summary>
		/// Gets or Sets the Unit ID of the Server.
		/// </summary>
		public int UnitID
		{
			get
			{
				return unitIdentifier;
			}
			set
			{
				int minUnitId = 1;
				int maxUnitId = 255;
				byte unitIdByte = 0x01;

				if (value >= minUnitId && value <= maxUnitId){
					unitIdByte = Convert.ToByte(value);
				} else
				{
					if (value < minUnitId)
						unitIdByte = Convert.ToByte(minUnitId);
					if (value > maxUnitId)
						unitIdByte = Convert.ToByte(maxUnitId);
					else

					if (debug) StoreLogData.Instance.Store("Invalid UnitID. UnitID must be 1-255", System.DateTime.Now);
				}

				unitIdentifier = unitIdByte;
			}
		}

		/// <summary>
		/// Gets or Sets the Filename for the LogFile
		/// </summary>
		public string LogFileFilename
		{
			get
			{
				return StoreLogData.Instance.Filename;
			}
			set
			{
				StoreLogData.Instance.Filename = value;
				if (StoreLogData.Instance.Filename != null)
					debug = true;
				else
					debug = false;
			}
		}

	}
}
