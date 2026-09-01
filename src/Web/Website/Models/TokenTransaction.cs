using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;

namespace Website.Models
{
	[Table("tblTokenTransaction")]
	public class TokenTransaction : ITokenTransaction
	{
		/// <summary>
		/// Primary key, Id
		/// </summary>
		[Key]
		// Not helping, better works to assign it in constructor
		//[DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
		public Guid Id { get; set; }

		/// <summary>
		/// Transaction date
		/// </summary>
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime Date { get; set; }

		/// <summary>
		/// Sender user Id
		/// </summary>
		[Required]
		[StringLength(128)]
		public string SenderUserId { get; set; }
		/// <summary>
		/// Sender User
		/// </summary>
		[ForeignKey(nameof(SenderUserId))]
		public virtual ApplicationUser Sender { get; set; }

		/// <summary>
		/// Receiver user Id
		/// </summary>
		[Required]
		[StringLength(128)]
		public string ReceiverUserId { get; set; }
		/// <summary>
		/// Receiver User
		/// </summary>
		[ForeignKey(nameof(ReceiverUserId))]
		public virtual ApplicationUser Receiver { get; set; }

		/// <summary>
		/// Amout of transferred tokens
		/// </summary>
		[Required]
		public int Tokens { get; set; }

		/// <summary>
		/// Token transfer transaction status
		/// </summary>
		[Required]
		public TokenTransactionStatus Status { get; set; }

		/// <summary>
		/// Last status set user Id
		/// </summary>
		[StringLength(128)]
		public string StatusUserId { get; set; }
		/// <summary>
		/// Receiver User
		/// </summary>
		[ForeignKey(nameof(StatusUserId))]
		public virtual ApplicationUser StatusUser { get; set; }

		public TokenTransaction()
		{
			Id = Guid.NewGuid();
		}
	}
}