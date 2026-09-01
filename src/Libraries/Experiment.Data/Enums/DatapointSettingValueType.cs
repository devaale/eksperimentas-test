using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Enums{
    public enum DatapointSettingValueType : byte
    {
        /// <summary>
        /// Normal, means Decimal
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Boolean, value converted to "true" or "false"
        /// </summary>
        Boolean = 1,

        /// <summary>
        /// Current time, not decimal or bool.
        /// 
        /// Warning! Not saved as DATAPOINT!
        /// This property simply should be passed to API.
        /// </summary>
        CurrentTime = 3,
    }
}
