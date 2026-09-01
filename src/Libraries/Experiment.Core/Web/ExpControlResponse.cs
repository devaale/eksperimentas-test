using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace Experiment.Core.Web{
	[JsonObject(MemberSerialization.OptIn)]
	public class ExpControlResponse
	{
		[JsonProperty(PropertyName = "control-response")]
		public List<ExpControlStatus> ControlResponse;
	}
}