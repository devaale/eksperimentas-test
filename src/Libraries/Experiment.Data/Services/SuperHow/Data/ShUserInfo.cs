using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

using Experiment.Data.Services.SuperHow.Metadata;

namespace Experiment.Data.Services.SuperHow.Data{
	public class ShUserInfo : IShUserInfo
	{
#if !SER1
		[JsonProperty("userId")]
#endif
		public string UserId { get; set; }

#if !SER1
		[JsonProperty("address")]
#endif
		public ShAddress Address { get; set; }

#if !SER1
		[JsonProperty("privateKey")]
#endif
		public string PrivateKey { get; set; }

#if !SER1
		[JsonProperty("publicKey")]
#endif
		public string PublicKey { get; set; }

#if !SER1
		[JsonProperty("transactionStatus")]
#endif
		public ShTransactionStatus TransactionStatus { get; set; }

		[JsonIgnore]
		public bool IsAddressOk
		{
			get
			{
				if (Address == null)
					return false;

				return !string.IsNullOrEmpty(Address.Address);
			}
		}

		/// <summary>
		/// GetAddres via few nested levels
		/// </summary>
		/// <returns></returns>
		public string GetAddress()
		{
			if(IsAddressOk)
			{
				return Address.Address;
			}
			else
			{
				return null;
			}
		}
	}
}
