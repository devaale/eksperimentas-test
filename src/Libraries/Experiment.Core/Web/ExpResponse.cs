using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace Experiment.Core.Web{
	[JsonObject(MemberSerialization.OptIn)]
	public class ExpResponse
	{
		[JsonProperty(PropertyName = "response-status")]
		public ExpErrorStatus ResponseStatus { get; set; }

		[JsonProperty(PropertyName = "response-data")]
		public object ResponseData { get; set; }

		public ExpResponse ()
		{
			ResponseStatus = new ExpErrorStatus();
		}
	}
}