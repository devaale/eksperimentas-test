using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

using Newtonsoft.Json;

namespace Website.Data.ContentEngine
{
	public class ContentEngineOptions
	{
		[JsonProperty(PropertyName = "canUpdate")]
		public bool CanUpdate { get; set; }

		[JsonProperty(PropertyName = "updateText")]
		public string UpdateText { get; set; }

		public ContentEngineOptions()
		{
			CanUpdate = false;
		}

	}
}
