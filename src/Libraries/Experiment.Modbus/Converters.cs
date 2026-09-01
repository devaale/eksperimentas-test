//#define 
// UpdateLoggerVars. Enabled: Vars form DB, Disabled: Default Vars

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Experiment.Core;
using Experiment.Core.IO;
using Experiment.Core.BL.Data;
using Experiment.Core.BL.Data.SysVars;
using Experiment.Core.Metadata;

namespace Experiment.Modbus{
	public class Converters
	{
		#region Attributes

		ILogger _Logger;
		ExpSql _Db;
		IDictionary<SysVarName, object> _Vars = new Dictionary<SysVarName, object>();
		Dictionary<int, RegisterType> _Dict;

		#endregion

		#region Init

		public Converters(ILogger logger)
		{
			Debug.Assert(logger != null,
				"ParsingServiceBase::CTOR: logger can't be NULL!");

			_Logger = logger;

#if UpdateSysVars
			_Db = new ExpSql(new SqlConnection(Defaults.ConnectionString), logger);
			UpdateSysVars();
#endif

			// Register Type
			_Dict = new Dictionary<int, RegisterType>();
			
			// 1 Bit Reigsters
			_Dict.Add(1, new RegisterType() { Id = 1, BitSize = 1, Real = 0, Signed = 0, BytesOrder = 0 });
			
			// 16 Bit Reigsters
			_Dict.Add(16000, new RegisterType() { Id = 16000, BitSize = 16, Real = 0, Signed = 0, BytesOrder = 0 });
			_Dict.Add(16010, new RegisterType() { Id = 16010, BitSize = 16, Real = 0, Signed = 1, BytesOrder = 0 });
			
			// 32 Bit Reigsters
			_Dict.Add(32000, new RegisterType() { Id = 32000, BitSize = 32, Real = 0, Signed = 0, BytesOrder = 12 });
			_Dict.Add(32001, new RegisterType() { Id = 32001, BitSize = 32, Real = 0, Signed = 0, BytesOrder = 21 });
			_Dict.Add(32010, new RegisterType() { Id = 32010, BitSize = 32, Real = 0, Signed = 1, BytesOrder = 12 });
			_Dict.Add(32011, new RegisterType() { Id = 32011, BitSize = 32, Real = 0, Signed = 1, BytesOrder = 21 });
			_Dict.Add(32100, new RegisterType() { Id = 32100, BitSize = 32, Real = 1, Signed = 0, BytesOrder = 12 });
			_Dict.Add(32101, new RegisterType() { Id = 32101, BitSize = 32, Real = 1, Signed = 0, BytesOrder = 21 });

			// 64 Bit Reigsters
			_Dict.Add(64000, new RegisterType() { Id = 64000, BitSize = 64, Real = 0, Signed = 0, BytesOrder = 1234 });
			_Dict.Add(64001, new RegisterType() { Id = 64001, BitSize = 64, Real = 0, Signed = 0, BytesOrder = 4321 });
			_Dict.Add(64010, new RegisterType() { Id = 64010, BitSize = 64, Real = 0, Signed = 1, BytesOrder = 1234 });
			_Dict.Add(64011, new RegisterType() { Id = 64011, BitSize = 64, Real = 0, Signed = 1, BytesOrder = 4321 });
			_Dict.Add(64100, new RegisterType() { Id = 64100, BitSize = 64, Real = 1, Signed = 0, BytesOrder = 1234 });
			_Dict.Add(64101, new RegisterType() { Id = 64101, BitSize = 64, Real = 1, Signed = 0, BytesOrder = 4321 });
		}

		void UpdateSysVars()
		{
			_Vars = _Db.SysVarsGet(SysVarModule.Parsing);

			if (_Vars.ContainsKey(SysVarName.PARSE_LOG_LEVEL))
			{
				_Logger.LogLevel = Convert.ToInt32(_Vars[SysVarName.PARSE_LOG_LEVEL]);
			}

			if (_Logger is FileLogger)
			{
				FileLogger logger = (FileLogger)_Logger;
				if (_Vars.ContainsKey(SysVarName.PARSE_LOG_LOCATION))
				{
					logger.LogFolder = _Vars[SysVarName.PARSE_LOG_LOCATION].ToString();
				}

				if (_Vars.ContainsKey(SysVarName.SCAN_LOG_USE_DATES))
				{
					logger.UseDatesInLogFileNames = _Vars[SysVarName.SCAN_LOG_USE_DATES].ToString().Equals("1");
				}
			}
		}

		#endregion

		#region Methods

			/// <summary>
			/// Converts Registers to 16 Bit Unsigned Integer value
			/// 
			/// Register type	|	16 bit unsigned
			/// Register ID		|	16000
			/// </summary>
			/// <param name="bytes">2 bytes array received from data array</param>
			/// <returns>16 bit unsigned integer value</returns>
		public UInt16 ConvertRegistersToUInt16(byte[] bytes)
		{
			if (bytes.Length != 2)
			{
				_Logger.WriteLine(3, "Converters::ConvertRegistersToUInt16 Input Array length invalid - Array length must be '2'");
				throw new ArgumentException("Converters::ConvertRegistersToUInt16 Input Array length invalid - Array length must be '2'");
			}
			return BitConverter.ToUInt16(bytes, 0);
		}

		/// <summary>
		/// Converts Registers to 16 Bit Signed Integer value
		/// 
		/// Register type	|	16 bit signed
		/// Register ID		|	16010
		/// </summary>
		/// <param name="bytes">2 bytes array received from data array</param>
		/// <returns>16 bit signed integer value</returns>
		public Int16 ConvertRegistersToInt16(byte[] bytes)
		{
			if (bytes.Length != 2)
			{
				_Logger.WriteLine(3, "Converters::ConvertRegistersToInt16 Input Array length invalid - Array length must be '2'");
				throw new ArgumentException("Converters::ConvertRegistersToInt16 Input Array length invalid - Array length must be '2'");
			}
			return BitConverter.ToInt16(bytes, 0);
		}

		/// <summary>
		/// Converts Registers to 32 Bit Unsigned Integer value
		/// 
		/// Register type	|	32 bit unsigned
		/// Register ID		|	32000
		/// </summary>
		/// <param name="bytes">4 bytes array received from data array</param>
		/// <returns>32 bit unsigned integer value</returns>
		public UInt32 ConvertRegistersToUInt32(byte[] bytes)
		{
			if (bytes.Length != 4)
			{
				_Logger.WriteLine(3, "Converters::ConvertRegistersToUInt32 Input Array length invalid - Array length must be '4'");
				throw new ArgumentException("Converters::ConvertRegistersToUInt32 Input Array length invalid - Array length must be '4'");
			}
			return BitConverter.ToUInt32(bytes, 0);
		}

		/// <summary>
		/// Converts Registers to 32 Bit Signed Integer value
		/// 
		/// Register type	|	32 bit signed
		/// Register ID		|	32010
		/// </summary>
		/// <param name="bytes">4 bytes array received from data array</param>
		/// <returns>32 bit signed integer value</returns>
		public Int32 ConvertRegistersToInt32(byte[] bytes)
		{
			if (bytes.Length != 4)
			{
				_Logger.WriteLine(3, "Converters::ConvertRegistersToInt32 Input Array length invalid - Array length must be '4'");
				throw new ArgumentException("Converters::ConvertRegistersToInt32 Input Array length invalid - Array length must be '4'");
			}
			return BitConverter.ToInt32(bytes, 0);
		}

		/// <summary>
		/// Converts Registers to 32 Bit Real value
		/// 
		/// Register type	|	32 bit real
		/// Register ID		|	32100
		/// </summary>
		/// <param name="bytes">4 bytes array received from data array</param>
		/// <returns>32 bit real value</returns>
		public float ConvertRegistersToInt32Float(byte[] bytes)
		{
			if (bytes.Length != 4)
			{
				_Logger.WriteLine(3, "Converters::ConvertRegistersToInt32Float Input Array length invalid - Array length must be '4'");
				throw new ArgumentException("Converters::ConvertRegistersToInt32Float Input Array length invalid - Array length must be '4'");
			}
			return BitConverter.ToSingle(bytes, 0);
		}

		/// <summary>
		/// Converts Registers to 64 Bit Unsigned Integer value
		/// 
		/// Register type	|	64 bit unsigned
		/// Register ID		|	64000
		/// </summary>
		/// <param name="bytes">8 bytes array received from data array</param>
		/// <returns>64 bit unsigned integer value</returns>
		public UInt64 ConvertRegistersToUInt64(byte[] bytes)
		{
			if (bytes.Length != 8)
			{
				_Logger.WriteLine(3, "Converters::ConvertRegistersToUInt64 Input Array length invalid - Array length must be '8'");
				throw new ArgumentException("Converters::ConvertRegistersToUInt64 Input Array length invalid - Array length must be '8'");
			}
			return BitConverter.ToUInt64(bytes, 0);
		}

		/// <summary>
		/// Converts Registers to 64 Bit Signed Integer value
		/// 
		/// Register type	|	64 bit signed
		/// Register ID		|	64010
		/// </summary>
		/// <param name="bytes">8 bytes array received from data array</param>
		/// <returns>64 bit signed integer value</returns>
		public Int64 ConvertRegistersToInt64(byte[] bytes)
		{
			if (bytes.Length != 8)
			{
				_Logger.WriteLine(3, "Converters::ConvertRegistersToInt64 Input Array length invalid - Array length must be '8'");
				throw new ArgumentException("Converters::ConvertRegistersToInt64 Input Array length invalid - Array length must be '8'");
			}
			return BitConverter.ToInt64(bytes, 0);
		}

		/// <summary>
		/// Converts Registers to 64 Bit Real value
		/// 
		/// Register type	|	64 bit real
		/// Register ID		|	64100
		/// </summary>
		/// <param name="bytes">8 bytes array received from data array</param>
		/// <returns>64 bit real value</returns>
		public double ConvertRegistersToInt64Double(byte[] bytes)
		{
			if (bytes.Length != 8)
			{
				_Logger.WriteLine(3, "Converters::ConvertRegistersToInt64Double Input Array length invalid - Array length must be '8'");
				throw new ArgumentException("Converters::ConvertRegistersToInt64Double Input Array length invalid - Array length must be '8'");
			}
			return BitConverter.ToDouble(bytes, 0);
		}

		/// <summary>
		/// Get shortened Bytes Array of Data
		/// </summary>
		/// <param name="bytes">Bytes array received from data array</param>
		/// <param name="startIndex">Data start index</param>
		/// <param name="bitSize">Size of bit</param>
		/// <returns>Shortened bytes array of data array</returns>
		public byte[] GetBytesOfData(byte[] bytes, int startIndex, int bitSize)
		{
			byte[] bytesArray = new byte[bitSize / 8];

			try
			{
				if (bytes == null)
				{
					//_Logger.WriteLine(3, "Converters::GetBytesOfData No data");
					throw new ArgumentException("Converters::GetBytesOfData No data");
				}

				if (bytes.Length < bitSize / 8)
				{
					//_Logger.WriteLine(3, "Converters::GetBytesOfData Input Array length invalid");
					throw new ArgumentException("Converters::GetBytesOfData Input Array length invalid");
				}

				if (bytes.Length <= startIndex)
				{
					//_Logger.WriteLine(3, "Converters::GetBytesOfData Input Array startIndex invalid");
					throw new ArgumentException("Converters::GetBytesOfData Input Array startIndex invalid");
				}

				Buffer.BlockCopy(bytes, startIndex, bytesArray, 0, bitSize / 8);
			}
			catch (Exception ex)
			{
				_Logger.WriteLine(3, ex.Message);
			}

			return bytesArray;
		}

		/// <summary>
		/// Change Bytes Order (Swapping)
		/// </summary>
		/// <param name="bytes">Bytes array received from data array</param>
		/// <param name="bitSize">Size of bit</param>
		/// <param name="bytesOrder">New bytes oder</param>
		/// <returns>Bytes array with new bytes order</returns>
		public byte[] BytesOrder(byte[] bytes, int bitSize, int bytesOrder)
		{
			if (bytes == null)
			{
				_Logger.WriteLine(3, "Converters::GetBytesOfData No data");
				throw new ArgumentException("Converters::GetBytesOfData No data");
			}

			if (bytes.Length != bitSize / 8)
			{
				_Logger.WriteLine(3, "Converters::BytesOrder Input Array length invalid");
				throw new ArgumentException("Converters::BytesOrder Input Array length invalid");
			}

			byte[] bytesArray = null;

			switch (bytesOrder)
			{
				// 32 Bit Registers
				case 12:
					bytesArray = new byte[] {
									bytes[0],
									bytes[1],
									bytes[2],
									bytes[3],
								};
					break;

				case 21:
					bytesArray = new byte[] {
									bytes[2],
									bytes[3],
									bytes[0],
									bytes[1],
								};
					break;

				// 64 Bit Registers
				case 1234:
					bytesArray = new byte[] {
									bytes[0],
									bytes[1],
									bytes[2],
									bytes[3],
									bytes[4],
									bytes[5],
									bytes[6],
									bytes[7],
								};
					break;

				case 3412:
					bytesArray = new byte[] {
									bytes[4],
									bytes[5],
									bytes[6],
									bytes[7],
									bytes[0],
									bytes[1],
									bytes[2],
									bytes[3],
								};
					break;

				case 2143:
					bytesArray = new byte[] {
									bytes[2],
									bytes[3],
									bytes[0],
									bytes[1],
									bytes[6],
									bytes[7],
									bytes[4],
									bytes[5],
								};
					break;

				case 4321:
					bytesArray = new byte[] {
									bytes[6],
									bytes[7],
									bytes[4],
									bytes[5],
									bytes[2],
									bytes[3],
									bytes[0],
									bytes[1],
								};
					break;

				default:
					break;
			}

			return bytesArray;
		}

		/// <summary>
		/// Parse Register Type
		/// </summary>
		/// <param name="registerType">Register type</param>
		/// <returns>Methods of Register Type Dictionary</returns>
		public RegisterType ParseRegisterType_Dict(int registerType)
		{
			if (_Dict.ContainsKey(registerType))
				return _Dict[registerType];
			else
			{
				_Logger.WriteLine(3, "Converters::ParseRegisterType_Dict Register Type not found");
				return null;
			}
		}

		/// <summary>
		/// Parse Data
		/// </summary>
		/// <param name="data">Bytes array received from data array</param>
		/// <param name="startaddress">Data start address</param>
		/// <param name="registerType">Register type</param>
		/// <param name="multiplier">Multiplier</param>
		/// <param name="offset">Offser</param>
		/// <returns>Decimal value</returns>
		public decimal ParseData(byte[] data, int startaddress, int registerType, decimal multiplier, decimal offset)
		{
			decimal _value = 0;

			switch (registerType)
			{
				// 1 Bit Registers
				// 1 Bit
				case 1:
					byte[] array = GetBytesOfData(data, startaddress, 8);
					_value = System.Convert.ToDecimal(array[0]);
					break;

				// 16 Bit Registers
				// 16 Bit Unsigned
				case 16000:
					_value = (System.Convert.ToDecimal(
						ConvertRegistersToUInt16(
							GetBytesOfData(data, startaddress, ParseRegisterType_Dict(registerType).BitSize)
							)) * multiplier) + offset;
					break;

				// 16 Bit Signed
				case 16010:
					_value = (System.Convert.ToDecimal(
						ConvertRegistersToInt16(
							GetBytesOfData(data, startaddress, ParseRegisterType_Dict(registerType).BitSize)
							)) * multiplier) + offset;
					break;

				// 32 Bit Registers
				// 32 Bit Unsigned
				case 32000:
					_value = (System.Convert.ToDecimal(
						ConvertRegistersToUInt32(
							BytesOrder(
								GetBytesOfData(data, startaddress, 
								ParseRegisterType_Dict(registerType).BitSize), 
								ParseRegisterType_Dict(registerType).BitSize, 
								ParseRegisterType_Dict(registerType).BytesOrder)
							)) * multiplier) + offset;
					break;

				// 32 Bit Unsigned Swapped
				case 32001:
					_value = (System.Convert.ToDecimal(
						ConvertRegistersToUInt32(
							BytesOrder(
								GetBytesOfData(data, startaddress,
								ParseRegisterType_Dict(registerType).BitSize),
								ParseRegisterType_Dict(registerType).BitSize,
								ParseRegisterType_Dict(registerType).BytesOrder)
							)) * multiplier) + offset;
					break;

				// 32 Bit Signed
				case 32010:
					_value = (System.Convert.ToDecimal(
						ConvertRegistersToInt32(
							BytesOrder(
								GetBytesOfData(data, startaddress,
								ParseRegisterType_Dict(registerType).BitSize),
								ParseRegisterType_Dict(registerType).BitSize,
								ParseRegisterType_Dict(registerType).BytesOrder)
							)) * multiplier) + offset;
					break;

				// 32 Bit Signed Swapped
				case 32011:
					_value = (System.Convert.ToDecimal(
						ConvertRegistersToInt32(
							BytesOrder(
								GetBytesOfData(data, startaddress,
								ParseRegisterType_Dict(registerType).BitSize),
								ParseRegisterType_Dict(registerType).BitSize,
								ParseRegisterType_Dict(registerType).BytesOrder)
							)) * multiplier) + offset;
					break;

				// 32 Bit Real
				case 32100:
					_value = (System.Convert.ToDecimal(
						ConvertRegistersToInt32Float(
							BytesOrder(
								GetBytesOfData(data, startaddress,
								ParseRegisterType_Dict(registerType).BitSize),
								ParseRegisterType_Dict(registerType).BitSize,
								ParseRegisterType_Dict(registerType).BytesOrder)
							)) * multiplier) + offset;
					break;

				// 32 Bit Real swapped
				case 32101:
					_value = (System.Convert.ToDecimal(
						ConvertRegistersToInt32Float(
							BytesOrder(
								GetBytesOfData(data, startaddress,
								ParseRegisterType_Dict(registerType).BitSize),
								ParseRegisterType_Dict(registerType).BitSize,
								ParseRegisterType_Dict(registerType).BytesOrder)
							)) * multiplier) + offset;
					break;

				// 64 Bit Registers
				// 64 Bit Unsigned
				case 64000:
					_value = (System.Convert.ToDecimal(
						ConvertRegistersToUInt64(
							BytesOrder(
								GetBytesOfData(data, startaddress,
								ParseRegisterType_Dict(registerType).BitSize),
								ParseRegisterType_Dict(registerType).BitSize,
								ParseRegisterType_Dict(registerType).BytesOrder)
							)) * multiplier) + offset;
					break;

				// 64 Bit Unsigned Swapped
				case 64001:
					_value = (System.Convert.ToDecimal(
						ConvertRegistersToUInt64(
							BytesOrder(
								GetBytesOfData(data, startaddress,
								ParseRegisterType_Dict(registerType).BitSize),
								ParseRegisterType_Dict(registerType).BitSize,
								ParseRegisterType_Dict(registerType).BytesOrder)
							)) * multiplier) + offset;
					break;

				// 64 Bit Signed
				case 64010:
					_value = (System.Convert.ToDecimal(
						ConvertRegistersToInt64(
							BytesOrder(
								GetBytesOfData(data, startaddress,
								ParseRegisterType_Dict(registerType).BitSize),
								ParseRegisterType_Dict(registerType).BitSize,
								ParseRegisterType_Dict(registerType).BytesOrder)
							)) * multiplier) + offset;
					break;

				// 64 Bit Signed Swapped
				case 64011:
					_value = (System.Convert.ToDecimal(
						ConvertRegistersToInt64(
							BytesOrder(
								GetBytesOfData(data, startaddress,
								ParseRegisterType_Dict(registerType).BitSize),
								ParseRegisterType_Dict(registerType).BitSize,
								ParseRegisterType_Dict(registerType).BytesOrder)
							)) * multiplier) + offset;
					break;

				// 64 Bit Real
				case 64100:
					_value = (System.Convert.ToDecimal(
						ConvertRegistersToInt64Double(
							BytesOrder(
								GetBytesOfData(data, startaddress,
								ParseRegisterType_Dict(registerType).BitSize),
								ParseRegisterType_Dict(registerType).BitSize,
								ParseRegisterType_Dict(registerType).BytesOrder)
							)) * multiplier) + offset;
					break;

				// 64 Bit Real Swapped
				case 64101:
					_value = (System.Convert.ToDecimal(
						ConvertRegistersToInt64Double(
							BytesOrder(
								GetBytesOfData(data, startaddress,
								ParseRegisterType_Dict(registerType).BitSize),
								ParseRegisterType_Dict(registerType).BitSize,
								ParseRegisterType_Dict(registerType).BytesOrder)
							)) * multiplier) + offset;
					break;

				default:
					break;
			}
			return _value;
		}

		#endregion

	}
}