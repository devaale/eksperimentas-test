using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;

namespace Website.Models
{
	[Table("tblWallet")]
	public class Wallet : IWallet
	{
		[Key]
		public Guid Id { get; set; }

		[DefaultValue(null)]
		[StringLength(128)]
		public string UserId { get; set; }

		[Required]
		[StringLength(128)]
		public string Address { get; set; }

		[Required]
		public string PrivateKey { get; set; }

		[Required]
		public string PublicKey { get; set; }

		[Required]
		[DefaultValue(false)]
		public bool System { get; set; }

		[Required]
		[DefaultValue(false)]
		public bool Primary { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime Created { get; }

		public Wallet()
		{
			Id = Guid.NewGuid();
			System = false;
			Primary = false;
			UserId = null;
		}
	}
}