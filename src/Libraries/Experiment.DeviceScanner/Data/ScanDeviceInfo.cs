using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Experiment.Core;
using Experiment.Core.Data;

using Experiment.Data.Enums;

namespace Experiment.DeviceScanner.Data{
	internal class ScanDeviceInfo : DbPrimitive
	{
		#region Properties

		internal DeviceProtocol Protocol { get => (DeviceProtocol)this[Defaults.DB_PROTOCOL]; }

		internal string Url { get => (string)this[Defaults.DB_URL]; }

		internal int Interval { get => (int)this[Defaults.DB_INTERVAL]; }

		internal DateTime? LastScanTime
		{
			get
			{
				object oValue = this[Defaults.DB_LAST_SCAN_TIME];
				DateTime? retVal;
				if(DBNull.Value.Equals(oValue))
				{
					retVal = null;
				} else
				{
					retVal = (DateTime)oValue;
				}

				return retVal;
			}
		}

		internal DateTime ProjectedScanTime { get => (DateTime)this[Defaults.DB_PROJECTED_SCAN_TIME]; }

		#endregion

		internal ScanDeviceInfo(DataRow row)
			: base(row)
		{
		}

	}
}
