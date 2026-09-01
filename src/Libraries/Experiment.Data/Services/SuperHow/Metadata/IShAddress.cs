using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Experiment.Data.Services.SuperHow.Metadata{
	public interface IShAddress : IShBase
	{
#if SER1
		[JsonProperty("address")]
#endif
		string Address { get; set; }

#if SER1
		[JsonProperty("networkType")]
#endif
		string NetworkType { get; set; }
	}
}
