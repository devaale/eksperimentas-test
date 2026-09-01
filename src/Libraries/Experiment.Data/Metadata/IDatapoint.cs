using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Core.Enums;
using Experiment.Data.Enums;
using Experiment.Data.Models;

namespace Experiment.Data.Metadata{
	public interface IDatapoint
	{
		int Id { get; set; }
		int DeviceId { get; set; }
		int Order { get; set; }
		string Name { get; set; }
		string Description { get; set; }
		string MeasureUnit { get; set; }
		DatapointType DatapointType { get; set; }
		int RegisterAddress { get; set; }
		int RegisterType { get; set; }
		int FunctionCode { get; set; }
		string Alias { get; set; }
		decimal Multiplier { get; set; }
		decimal Offset { get; set; }
		int ReadWrite { get; set; }
		int? DatapointFormulaId { get; set; }
		DatePartOrInterval IntervalDatepart { get; set; }
		DatePartOrInterval AggregationDatepart { get; set; }
		DateTime LastFormulaCalcTime { get; set; }
		DeviceProtocol? DeviceProtocol { get; set; }

		// New fields since 2023-12-14
		string Topic { get; set; }
		string Theme { get; set; }
		string ResourceUri { get; set; }
		string Payload { get; set; }
		int Instance { get; set; }
		int BACnetObjectType { get; set; }
		int BACnetPropertyId { get; set; }
		int BACnetFunctionCode { get; set; }
		int BACnetDataType { get; set; }
	}
}
