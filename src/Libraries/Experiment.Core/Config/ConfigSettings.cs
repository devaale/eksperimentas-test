using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Experiment.Core.Config{
	public class ConfigSettings
	{
		[JsonProperty("host")]
		public string Host { get; set; }
		[JsonProperty("database")]
		public string Database { get; set; }
		[JsonProperty("uid")]
		public string Username { get; set; }
		[JsonProperty("pwd")]
		public string Password { get; set; }
	}
}
