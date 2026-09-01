using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

using Experiment.Data.Services.SuperHow.Data;

namespace Experiment.Data.Services.SuperHow.Metadata{
	public interface IShUserInfo : IShBase
	{

#if SER1
		[JsonProperty("userId")]
#endif
		string UserId { get; set; }

#if SER1
		[JsonProperty("address")]
#endif
		ShAddress Address { get; set; }

#if SER1
		[JsonProperty("privateKey")]
#endif
		string PrivateKey { get; set; }

#if SER1
		[JsonProperty("publicKey")]
#endif
		string PublicKey { get; set; }

#if SER1
		[JsonProperty("transactionStatus")]
#endif
		ShTransactionStatus TransactionStatus { get; set; }
	}
}
