using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

using Experiment.Data.Services.SuperHow.Metadata;

namespace Experiment.Data.Services.SuperHow.Data{
	public class ShAddress : IShAddress
	{
#if !SER1
		[JsonProperty("address")]
#endif
		public string Address { get; set; }

#if !SER1
		[JsonProperty("networkType")]
#endif
		public string NetworkType { get; set; }
	}
}
