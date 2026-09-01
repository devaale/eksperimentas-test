using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

using Experiment.Data.Metadata;
using Experiment.Data.Enums;

namespace Website.Models
{
    [Table("tblDevice")]
	public class Device : IDevice
	{
		[Key]
		public int Id { get; set; }

		[StringLength(256)]
		public string Name { get; set; }

		[Column(TypeName = "ntext")]
		public string Description { get; set; }

		//public DeviceType Type { get; set; } // Removed 2023-10-10 @AG

		/// <summary>
		/// Parent or user, which datapoint that was
		/// </summary>
		public int ObjectId { get; set; }

		[ForeignKey(nameof(ObjectId))]
		public EObject Object { get; set; }

		[StringLength(256)]
		public string Url { get; set; }

		public int UnitId { get; set; }

		public int Interval { get; set; }

        public DeviceProtocol Protocol { get; set; }

        [StringLength(256)]
        public string ClientId { get; set; }

        [StringLength(256)]
        public string Topic { get; set; }
		public decimal DeprGL { get; set; }
		public decimal DeprA { get; set; }
		public decimal DeprLIR { get; set; }
		public decimal DeprRL { get; set; }
		public decimal DeprC { get; set; }
		public decimal DeprSD { get; set; }
		public string ClientUsername { get; set; }
		public string ClientPassword { get; set; }

		[Column("lastScanTime")]
		public DateTime? LastScanTime { get; set; }

		[Column("projectedScanTime")]
		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime? ProjectedScanTime { get; set; }

		/// <summary>
		/// Device's datapoints
		/// </summary>
		public ICollection<Datapoint> Datapoints { get; set; }
	}
}