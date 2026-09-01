using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;

namespace Website.Models
{
	[Table("tblOrder")]
	public class Order : IOrder
	{
		/// <summary>
		/// Order Unique Key Id
		/// </summary>
		[Key]
		public Guid Id { get; set; }

		/// <summary>
		/// UserId
		/// </summary>
		[Required]
		public string UserId { get; set; }
		/// <summary>
		/// Subject's User
		/// </summary>
		[ForeignKey(nameof(UserId))]
		public virtual ApplicationUser User { get; set; }

		/// <summary>
		/// Order number
		/// </summary>
		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		[StringLength(16)]
		public string OrderNo { get; set; }

		/// <summary>
		/// Full price before discount calculated
		/// </summary>
		[Required]
		public decimal FullPrice { get; set; }

		/// <summary>
		/// How many tokens for discount used, each token 2%
		/// </summary>
		[Required]
		public int UsedTokens { get; set; }

		/// <summary>
		/// Discount sum
		/// </summary>
		[Required]
		public decimal Discount { get; set; }

		/// <summary>
		/// Final price, what to pay for user
		/// </summary>
		[Required]
		public decimal FinalPrice { get; set; }

		/// <summary>
		/// Payment method Id, which returned from Billing Back-end
		/// </summary>
		public string PaymentMethodId { get; set; }

		/// <summary>
		/// Payment Method name
		/// </summary>
		public string PaymentMethod { get; set; }

		/// <summary>
		/// Current state of the order
		/// </summary>
		[Required]
		public OrderState State { get; set; }

		/// <summary>
		/// When order was posted
		/// </summary>
		[Required]
		public DateTime Posted { get; set; }

		[StringLength(32)]
		public string PostedIp { get; set; }

		/// <summary>
		/// When order was completed, paid, rejected and so on. can be NULL, means not processed to the end.
		/// </summary>
		public DateTime? Completed { get; set; }

		[StringLength(32)]
		public string CompletedIp { get; set; }

		/// <summary>
		/// Technical completion data
		/// </summary>
		public string Data { get; set; }

		/// <summary>
		/// Order's details
		/// </summary>
		public ICollection<OrderDetail> OrderDetails { get; set; }


		public Order()
		{
			Id = Guid.NewGuid();
		}

	}
}
