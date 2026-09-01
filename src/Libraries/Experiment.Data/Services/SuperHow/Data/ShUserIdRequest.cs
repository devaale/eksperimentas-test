using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

using Experiment.Data.Services.SuperHow.Metadata;

namespace Experiment.Data.Services.SuperHow.Data{
	public class ShUserIdRequest : IShUserIdRequest
	{
#if !SER1
		[JsonProperty("userId")]
#endif
		public string UserId { get; set; }
	}
}

