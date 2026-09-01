using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using Experiment.Core.Base;
using Experiment.Data.Metadata;
using Newtonsoft.Json;

namespace Website.Models
{
	[Table("tblDatapointFormulaChain")]
	public class DatapointFormulaChain : IDatapointFormulaChain
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int DatapointId { get; set; }

		[Required]
		public int Order { get; set; }

		/// <summary>
		/// Only one should be not null.
		/// </summary>
		[DefaultValue(null)]
		public int? RelatedDatapointId { get; set; }

		/// <summary>
		/// Only one should be not null.
		/// </summary>
		[DefaultValue(null)]
		public decimal? Value { get; set; }
	}
}
