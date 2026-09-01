using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class Wallet : IWallet
	{
		public Guid Id { get; set; }
		public string UserId { get; set; }
		public string Address { get; set; }
		public string PrivateKey { get; set; }
		public string PublicKey { get; set; }
		public bool System { get; set; }
		public bool Primary { get; set; }
		public DateTime Created { get; set;  }

	}
}
