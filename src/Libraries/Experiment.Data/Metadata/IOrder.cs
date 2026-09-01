using Experiment.Data.Enums;
using Experiment.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IOrder
	{
		/// <summary>
		/// Order Unique Key Id
		/// </summary>
		Guid Id { get; set; }

		/// <summary>
		/// UserId
		/// </summary>
		string UserId { get; set; }

		/// <summary>
		/// Order number
		/// </summary>
		string OrderNo { get; set; }

		/// <summary>
		/// Full price before discount calculated
		/// </summary>
		decimal FullPrice { get; set; }

		/// <summary>
		/// How many tokens for discount used, each token 2%
		/// </summary>
		int UsedTokens { get; set; }
		
		/// <summary>
		/// Discount sum
		/// </summary>
		decimal Discount { get; set; }

		/// <summary>
		/// Final price, what to pay for user
		/// </summary>
		decimal FinalPrice { get; set; }

		/// <summary>
		/// Payment method Id, which returned from Billing Back-end
		/// </summary>
		string PaymentMethodId { get; set; }

		/// <summary>
		/// Payment Method name
		/// </summary>
		string PaymentMethod { get; set; }

		/// <summary>
		/// Current state of the order
		/// </summary>
		OrderState State { get; set; }

		/// <summary>
		/// When order was posted
		/// </summary>
		DateTime Posted { get; set; }
		string PostedIp { get; set; }

		/// <summary>
		/// When order was completed, paid, rejected and so on. can be NULL, means not processed to the end.
		/// </summary>
		DateTime? Completed { get; set; }
		string CompletedIp { get; set; }

		/// <summary>
		/// Technical completion data
		/// </summary>
		string Data { get; set; }
	}
}
