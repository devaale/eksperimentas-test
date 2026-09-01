using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Enums{
	public enum DeviceProtocol : int
	{
		Unknown = 0,
		Modbus = 10,
		BACnet = 20,
		MQTT = 30,
		CoAP = 40,
		OpenThread = 50,
		API = 100,
	}
}
