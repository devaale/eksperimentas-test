using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class Order : IOrder
	{
		/// <summary>
		/// Order Unique Key Id
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// UserId
		/// </summary>
		public string UserId { get; set; }

		/// <summary>
		/// Order number
		/// </summary>
		public string OrderNo { get; set; }

		/// <summary>
		/// Full price before discount calculated
		/// </summary>
		public decimal FullPrice { get; set; }

		/// <summary>
		/// How many tokens for discount used, each token 2%
		/// </summary>
		public int UsedTokens { get; set; }

		/// <summary>
		/// Discount sum
		/// </summary>
		public decimal Discount { get; set; }

		/// <summary>
		/// Final price, what to pay for user
		/// </summary>
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
		public OrderState State { get; set; }

		/// <summary>
		/// When order was posted
		/// </summary>
		public DateTime Posted { get; set; }
		public string PostedIp { get; set; }

		/// <summary>
		/// When order was completed, paid, rejected and so on. can be NULL, means not processed to the end.
		/// </summary>
		public DateTime? Completed { get; set; }
		public string CompletedIp { get; set; }

		/// <summary>
		/// Technical completion data
		/// </summary>
		public string Data { get; set; }

		/// <summary>
		/// Order details, part of the invoice details if will be needed
		/// </summary>
		public ICollection<OrderDetail> OrderDetails { get; set; }
	}
}
