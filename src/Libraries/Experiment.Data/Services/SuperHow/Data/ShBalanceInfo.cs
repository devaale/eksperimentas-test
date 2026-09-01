using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Experiment.Data.Services.SuperHow.Data{
	public class ShBalanceInfo
	{
		[JsonProperty("mosaicId")]
		public string MosaicId { get; set; }

		[JsonProperty("amount")]
		public int amount { get; set; }
	}
}
