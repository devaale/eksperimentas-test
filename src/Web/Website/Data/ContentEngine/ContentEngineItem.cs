using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace Website.Data.ContentEngine
{
	public abstract class ContentEngineItem : IContentEngineItem
	{
		[JsonProperty(PropertyName = "label")]
		public string Label { get; set; }

		[JsonProperty(PropertyName = "type")]
		public string Type { get; set; }
	}
}