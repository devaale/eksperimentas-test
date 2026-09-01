using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Experiment.Data.Services.SuperHow.Metadata{
	public interface IShUserIdRequest : IShBase
	{
#if SER1
		[JsonProperty("userId")]
#endif
		string UserId { get; set; }
	}
}
