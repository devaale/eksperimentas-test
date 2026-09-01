using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Experiment.Core.Web{
	public class HttpRequestState
	{
		public string Url { get; set; }
		public string RequestJson { get; set; }
		public string ResultJson { get; set; }
		public HttpResponseMessage Response { get; set; }
	}
}
