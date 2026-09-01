using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using Newtonsoft.Json;

namespace Experiment.Data.Services.SuperHow.Metadata{
	/// <summary>
	/// @see https://gw.energus.superhow.net/api-docs/#/default/put_transaction_send
	/// </summary>
	public interface IShSendRequest
	{
		/// <summary>
		/// "senderPrivateKey": "5C0EC86EC32C493637830B2E77CFE8189B9A155002AACDD19050EA978D1B7681",
		/// </summary>
		string SenderPrivateKey { get; set; }

		/// <summary>
		/// "recipientAddress": "TD7JAQGZCS6LPIK5CO6GMYZUCDIGUYEZLWPYV7Y",
		/// </summary>
		string RecipientAddress { get; set; }

		/// <summary>
		/// "amount": 30,
		/// </summary>
		int Amount { get; set; }

		/// <summary>
		/// "namespaceName": "cat.currency",
		/// </summary>
		string NamespaceName { get; set; }

		/// <summary>
		/// "message": "Sending tokens"
		/// </summary>
		object Message { get; set; }
	}
}
