using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Experiment.Data.Services.SuperHow.Metadata{
	public interface IShTransactionStatus : IShBase
	{
#if SER1
		[JsonProperty("hash")]
#endif
		string Hash { get; set; }

#if SER1
		[JsonProperty("group")]
#endif
		string Group { get; set; }

#if SER1
		[JsonProperty("timestamp")]
#endif
		string Timestamp { get; set; }
	}
}
