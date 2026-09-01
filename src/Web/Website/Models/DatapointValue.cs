//#define FORCE_COMPUTED_DATE

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using Experiment.Data.Metadata;

namespace Website.Models
{
	[Table("tblDatapointValue")]
	public class DatapointValue : IDatapointValue
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int DatapointId { get; set; }

		[ForeignKey(nameof(DatapointId))]
		public Datapoint Datapoint { get; set; }

#if FORCE_COMPUTED_DATE
		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]	// If to enable this, default value will be written and transfered date probably ignored
#endif
		public DateTime Date { get; set; }
		public decimal Value { get; set; }
	}
}