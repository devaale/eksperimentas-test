using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Experiment.Core;
using Experiment.Core.Data;

namespace Experiment.Core.BL.Data{
    public class ExpSystem : DbPrimitive
    {
        public string SystemId
        {
            get { return Id; }
        }

        public int DeviceId
        {
            get { return (int)this[Defaults.DB_DEVICE_ID]; }
        }

        public int DataStart
        {
            get { return (int)this[Defaults.DB_DATA_START]; }
        }

        public int BitSize
        {
            get { return (int)this[Defaults.DB_BITSIZE]; }
        }

        public decimal Multiplier
        {
            get { return (decimal)this[Defaults.DB_MULTIPLIER]; }
        }

        public ExpSystem(DataRow row)
            : base(row)
        {
        }
    }
}
