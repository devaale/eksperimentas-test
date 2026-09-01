using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

using Experiment.Core;
using Experiment.Core.Data;

namespace Experiment.DeviceScanner.Data{
	public class Datapoint : DbPrimitive
	{
        public string SystemId
        {
            get { return Id; }
        }

        public int RegisterAddress
        {
            get { return (int)this[Defaults.DB_REGISTER_ADDRESS]; }
        }

		public int RegisterType
		{
			get { return (int)this[Defaults.DB_REGISTER_TYPE]; }
		}

		public int FunctionCode
		{
			get { return (int)this[Defaults.DB_FUNCTION_CODE]; }
		}

		public decimal Multiplier
        {
            get { return (decimal)this[Defaults.DB_MULTIPLIER]; }
        }

		public decimal Offset
		{
			get { return (decimal)this[Defaults.DB_OFFSET]; }
		}

		public Datapoint(DataRow row)
			: base(row)
		{
		}
	}
}
