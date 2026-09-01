using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using Experiment.Core.Base;
using Experiment.Data.Metadata;

namespace Website.Models
{
	[Table("tblDatapointFormula")]
	public class DatapointFormula : IDatapointFormula
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Alias { get; set; }

		[Required]
		[DefaultValue(0)]
		public int NumDatapoints { get; set; }

		[Required]
		[DefaultValue(false)]
		public bool Aggregated { get; set; }
	}
}
