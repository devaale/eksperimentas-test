using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace Experiment.Core.Web{
	public class ExpControlStatus
	{
		[JsonProperty("id")]
		public string Id { get; set; }
		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonProperty("value")]
		public string Value { get; set; }
		[JsonProperty("date")]
		public string Date { get; set; }
		[JsonProperty("error-status")]
		public string ErrorStatus { get; set; }
		[JsonProperty("error-msg")]
		public string ErrorMsg { get; set; }
	}
}