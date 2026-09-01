using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Services.SuperHow.Metadata;
using Newtonsoft.Json;

namespace Experiment.Data.Services.SuperHow.Data{
	public class ShSendRequest : IShSendRequest
	{
		/// <summary>
		/// "senderPrivateKey": "5C0EC86EC32C493637830B2E77CFE8189B9A155002AACDD19050EA978D1B7681",
		/// </summary>
		[JsonProperty("senderPrivateKey")]
		public string SenderPrivateKey { get; set; }

		/// <summary>
		/// "recipientAddress": "TD7JAQGZCS6LPIK5CO6GMYZUCDIGUYEZLWPYV7Y",
		/// </summary>
		[JsonProperty("recipientAddress")]
		public string RecipientAddress { get; set; }

		/// <summary>
		/// "amount": 30,
		/// </summary>
		[JsonProperty("amount")]
		public int Amount { get; set; }

		/// <summary>
		/// "namespaceName": "cat.currency",
		/// </summary>
		[JsonProperty("namespaceName")]
		public string NamespaceName { get; set; }

		/// <summary>
		/// "message": "Sending tokens"
		/// </summary>
		[JsonProperty("message")]
		public object Message { get; set; }
	}
}
