using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IWallet
	{
		Guid Id { get; set; }
		string UserId { get; set; }
		string Address { get; set; }
		string PrivateKey { get; set; }
		string PublicKey { get; set; }
		bool System { get; set; }
		bool Primary { get; set; }
		DateTime Created { get; }
	}
}
