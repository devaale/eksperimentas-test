using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Enums;

namespace Experiment.Data.Metadata{
	public interface IDatapointSetting
	{
		int Protocol { get; set; }
		string Name { get; set; }
		string Description { get; set; }
		ParameterDirection Direction { get; set; }
		DatapointSettingValueType ValueType { get; set; }
		bool Mandatory { get; set; }
	}
}
