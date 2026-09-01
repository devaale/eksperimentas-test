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
	[Table("tblDatapointSetting")]
	public class DatapointSetting : IDatapointSetting
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int Protocol { get; set; }

		[Required]
		[StringLength(256)]
		public string Name { get; set; }

		public string Description { get; set; }

		[Required]
		[DefaultValue(ParameterDirection.None)]
		public ParameterDirection Direction { get; set; }

		[Required]
		[DefaultValue(DatapointSettingValueType.Normal)]
		public DatapointSettingValueType ValueType { get; set; }

		[Required]
		[DefaultValue(false)]
		public bool Mandatory { get; set; }
	}
}