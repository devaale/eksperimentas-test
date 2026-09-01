#if DEBUG

// Add own precompiler defs for work
#define DEV_ARVYDAS
//#define DEV_ROMAN
//#define DEV_DMITRIJUS

#else

#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Experiment.Data.Enums;
using Experiment.Maui.Data;

namespace Experiment.Maui{
	class Hardcoded
	{
		#region Constants

		#endregion

		#region Attributes

		#endregion

		#region Properties

		/// <summary>
		/// Protocol types
		/// 
		/// @TODO: Protocol types
		/// </summary>
		public static Dictionary<DeviceProtocol, string> ProtocolTypes = new Dictionary<DeviceProtocol, string>()
		{
			{DeviceProtocol.Unknown, "Unknown" },
			{DeviceProtocol.Modbus, "Modbus" },
			{DeviceProtocol.BACnet, "BACnet" },
			{DeviceProtocol.MQTT, "MQTT" },
			{DeviceProtocol.CoAP, "CoAP" },
			{DeviceProtocol.OpenThread, "OpenThread" },
			{DeviceProtocol.API, "API" },
		};

		public static List<KeyValuePair<DeviceProtocol, string>> ProtocolTypesList => Hardcoded.ProtocolTypes.ToList();

		/// <summary>
		/// Devices scan types
		/// </summary>
		public static Dictionary<int, string> ScanTypes = new Dictionary<int, string>()
		{
			{0, E.T("unknown") },
			{10, E.T("bluetooth") },
		};

		public static List<KeyValuePair<int, string>> ScanTypesList => Hardcoded.ScanTypes.ToList();

		/// Experiment.Modbus datapoint register types
		/// 
		/// </summary>
		public static Dictionary<int, string> RegisterTypes = new Dictionary<int, string>()
		{
			{16000, "16 bit unsigned" },
			{16010, "16 bit signed" },

			{32001, "32 bit unsigned swapped" },
			{32010, "32 bit signed" },
			{32011, "32 bit signed swapped" },
			{32100, "32 bit real" },
			{32101, "32 bit real swapped" },

			{64000, "64 bit unsigned" },
			{64001, "64 bit unsigned swapped" },
			{64010, "64 bit signed" },
			{64011, "64 bit signed swapped" },
			{64100, "64 bit real" },
			{64101, "64 bit real swapped" },
		};

		public static List<KeyValuePair<int, string>> RegisterTypeList => Hardcoded.RegisterTypes.ToList();

		/// Experiment.Modbus datapoint function codes
		/// 
		/// </summary>
		public static Dictionary<int, string> FunctionCodes = new Dictionary<int, string>()
		{
			{1, "Read Coils (01)" },
			{2, "Discrete Inputs (02)" },
			{3, "Holding Registers (03)" },
			{4, "Input Registers (04)" },
			{5, "Single Coin (05) [Write]" },
			{6, "Single Register (06) [Write]" },
		};

		public static List<KeyValuePair<int, string>> FunctionCodeList => Hardcoded.FunctionCodes.ToList();

		/// Experiment.Modbus datapoint Read / Write type
		/// 
		/// </summary>
		public static Dictionary<int, string> ReadWriteTypes = new Dictionary<int, string>()
		{
			{0, E.T("read") },
			{1, E.T("write") },
		};

		public static List<KeyValuePair<int, string>> ReadWriteTypesList => Hardcoded.ReadWriteTypes.ToList();

		#endregion

		#region Singleton 

		static Hardcoded _sInstance;
		static Hardcoded Instance
		{
			get
			{
				if (_sInstance == null)
				{
					_sInstance = new Hardcoded();
				}
				return _sInstance;
			}
		}

		#endregion

	}
}
