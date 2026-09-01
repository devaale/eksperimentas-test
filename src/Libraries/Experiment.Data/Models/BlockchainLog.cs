using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class BlockchainLog : IBlockchainLog
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public DateTime Created { get; set; }
		public string RequestUri { get; set; }
		public string ReqestParams { get; set; }
		public string Result { get; set; }
		public HttpStatusCode Status { get; set; }
	}
}
