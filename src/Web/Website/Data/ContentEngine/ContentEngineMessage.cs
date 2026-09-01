using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

using Newtonsoft.Json;

namespace Website.Data.ContentEngine
{
	public class ContentEngineMessage
	{
		[JsonProperty(PropertyName = "message-level")]
		public int MessageLevel { get; set; }

		[JsonProperty(PropertyName = "message")]
		public string Message { get; set; }

		public ContentEngineMessage()
		{

		}
		public ContentEngineMessage(int msgLevel, string msg)
		{
			MessageLevel = msgLevel;
			Message = msg;
		}
	}
}
