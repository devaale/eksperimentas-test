using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json;

namespace Website.Data.ContentEngine
{
	public class ContentEngineResult
	{
		[JsonProperty(PropertyName = "submit")]
		public string SubmitUrl { get; set; }

		[JsonProperty(PropertyName = "options")]
		public ContentEngineOptions Options { get; set; }

		[JsonProperty(PropertyName = "messages")]
		public List<ContentEngineMessage> Messages { get; set; }

		[JsonProperty(PropertyName = "data")]
		public List<IContentEngineItem> Data { get; set; }

		/// <summary>
		/// Ctor
		/// </summary>
		public ContentEngineResult()
		{
			Options = new ContentEngineOptions();
			Messages = new List<ContentEngineMessage>();
			Data = new List<IContentEngineItem>();
		}
	}
}
