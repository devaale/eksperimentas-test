using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Web;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;

namespace Website.Models
{
	[Table("tblBlockchainLog")]
	public class BlockchainLog : IBlockchainLog
	{
		[Key]
		public int Id { get; set; }

		[DefaultValue(null)]
		public string UserId { get; set;  }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime Created { get; set; }

		[DefaultValue(null)]
		public string RequestUri { get; set; }

		[DefaultValue(null)]
		public string ReqestParams { get; set; }

		[DefaultValue(null)]
		public string Result { get; set; }

		public HttpStatusCode Status { get; set; }
	}
}