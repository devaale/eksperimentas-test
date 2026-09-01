using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Experiment.Data.Enums;

namespace Experiment.DeviceProcessing.Service.Models{
	/// <summary>
	/// Corresponds [prcDatapointApiData] stored procedure result values
	/// </summary>
	public class AiDatapointInfo
	{
		public int DatapointId { get; set; }
		public string Alias { get; set; }
		public decimal Multiplier { get; set; }
		public ParameterDirection Direction { get; set; }
		public DatapointSettingValueType ValueType { get; set; }
		public bool Mandatory { get; set; }
		public decimal? Value { get; set; }
	}
}
