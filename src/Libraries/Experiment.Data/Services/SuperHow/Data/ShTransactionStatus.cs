using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Services.SuperHow.Metadata;
using Newtonsoft.Json;

namespace Experiment.Data.Services.SuperHow.Data{
	public class ShTransactionStatus : IShTransactionStatus
	{
#if !SER1
		[JsonProperty("hash")]
#endif
		public string Hash { get; set; }

#if !SER1
		[JsonProperty("group")]
#endif
		public string Group { get; set; }

#if !SER1
		[JsonProperty("timestamp")]
#endif
		public string Timestamp { get; set; }
	}
}
