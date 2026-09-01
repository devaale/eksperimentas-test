using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using Experiment.Core.Enums;
using Experiment.Data.Enums;
using Experiment.Data.Metadata;

namespace Website.Models
{
    [Table("tblDatapoint")]
	public class Datapoint : IDatapoint
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int DeviceId { get; set; }

		public int Order { get; set; }

		/// <summary>
		/// Parent reference
		/// </summary>
		[ForeignKey(nameof(DeviceId))]
		public Device Device { get; set; }

		[StringLength(256)]
		public string Name { get; set; }

		[Column(TypeName = "ntext")]
		public string Description { get; set; }

		[StringLength(256)]
		public string MeasureUnit { get; set; }
		
		[DefaultValue(DatapointType.Unknown)]
		public DatapointType DatapointType { get; set; }
		public int RegisterAddress { get; set; }
		public int RegisterType { get; set; }
		public int FunctionCode { get; set; }

        [StringLength(256)]
        public string Alias { get; set; }
        public decimal Multiplier { get; set; }
		public decimal Offset { get; set; }
        public int ReadWrite { get; set; }

		[DefaultValue(null)]
		public int? DatapointFormulaId { get; set; }

		[DefaultValue(DatePartOrInterval.None)]
		public DatePartOrInterval IntervalDatepart { get; set; }

		[DefaultValue(DatePartOrInterval.None)]
		public DatePartOrInterval AggregationDatepart { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime LastFormulaCalcTime { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DeviceProtocol? DeviceProtocol { get; set; }

		public string Topic { get; set; }
		public string Theme { get; set; }
		public string ResourceUri { get; set; }
		public string Payload { get; set; }
		public int Instance { get; set; }
		public int BACnetObjectType { get; set; }
		public int BACnetPropertyId { get; set; }
		public int BACnetFunctionCode { get; set; }
		public int BACnetDataType { get; set; }

		/// <summary>
		/// Datapoint values, date based
		/// </summary>
		public ICollection<DatapointValue> Values { get; set; }

		/// <summary>
		/// Datapoint formula chains
		/// </summary>
		public ICollection<DatapointFormulaChain> Chains { get; set; }

	}
}