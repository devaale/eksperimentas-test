using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
    public class DatapointSetting  : IDatapointSetting
    {
        public int Protocol { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ParameterDirection Direction { get; set; }
        public DatapointSettingValueType ValueType { get; set; }
        public bool Mandatory { get; set; }
    }
}
