using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace Website.Data.ContentEngine
{
	public class ContentEngineField : ContentEngineItem
	{
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "required")]
		public bool Required { get; set; }

		[JsonProperty(PropertyName = "value")]
		public object Value { get; set; }

		[JsonProperty(PropertyName = "defaultValue")]
		public object DefaultValue { get; set; }

		[JsonProperty(PropertyName = "visible")]
		public bool? Visible { get; set; }

		[JsonProperty(PropertyName = "readOnly")]
		public bool? ReadOnly { get; set; }

		public ContentEngineField()
		{
			Visible = true;
			Required = false;
			ReadOnly = false;
		}
	}
}