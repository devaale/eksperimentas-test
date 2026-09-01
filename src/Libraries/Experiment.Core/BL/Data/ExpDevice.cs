using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

using Experiment.Core;
using Experiment.Core.Data;

namespace Experiment.Core.BL.Data{
	/// <summary>
	/// It is good to be able to switch to Experiment.Data.Models.Device, but this is Experiment.Core class, which is higher in hierarchy
	/// </summary>
	public class ExpDevice : DataRowItem
	{
		const int DEFAULT_INTERVAL = 3600;
		const int DEFAULT_DEVICE_PORT = 502;
		const int DEFAULT_UNIT_ID = 1;

		public string Name { get { return this[Defaults.DB_NAME].ToString(); } }
		public int Interval
		{
			get
			{
				int retVal = DEFAULT_INTERVAL;
				if(!int.TryParse(this[Defaults.DB_INTERVAL].ToString(), out retVal))
				{
					retVal = DEFAULT_INTERVAL;
				}
				return retVal;
			}
		}

		public string Host { get; protected set; }
		public int Port { get; protected set; }
		public int UnitID { get; protected set; }

		public ExpDevice(DataRow row)
			: base(row)
		{
			Port = DEFAULT_DEVICE_PORT;
			UnitID = DEFAULT_UNIT_ID;

			Init();
		}

		void Init()
		{
			string url = this[Defaults.DB_URL].ToString();
			int unitId = int.Parse(this[Defaults.DB_UNIT_ID].ToString());

			Validation.RequireValidString(url, "url");

			string[] tokens = url.Split(':');

			if (tokens.Length > 0)
			{
				Host = tokens[0];
			}

			if(tokens.Length > 1)
			{ 
				int port = 0;

				if(int.TryParse(tokens[1], out port))
				{
					Port = port;
				}
			}

			UnitID = unitId;
		}
	}
}
