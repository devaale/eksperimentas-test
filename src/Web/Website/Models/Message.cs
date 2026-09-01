using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using Experiment.Data.Metadata;
using Experiment.Data.Models;

namespace Website.Models
{
	[Table("tblMessage")]
	public class Message : IMessage
	{
		/// <summary>
		/// Id of the record
		/// </summary>
		[Key]
		public int Id { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime Date { get; set; }
		/// <summary>
		/// Sender's UserId
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
		/// Receiver's UserId
		/// </summary>
		[Required]
		[StringLength(128)]
		public string ReceiverUserId { get; set; }
		/// <summary>
		/// Receiver User
		/// </summary>
		[ForeignKey(nameof(ReceiverUserId))]
		public virtual ApplicationUser Receiver { get; set; }

		[Required]
		[StringLength(4096)]
		public string Body { get; set; }

		public DateTime? Read { get; set; }
	}
}
