using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IBlockchainLog
	{
		int Id { get; set; }
		string UserId { get; set; }
		DateTime Created { get; set; }
		string RequestUri { get; set; }
		string ReqestParams { get; set; }
		string Result { get; set; }
		HttpStatusCode Status { get; set; }
	}
}
