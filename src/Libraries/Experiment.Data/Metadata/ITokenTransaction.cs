using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Enums;

namespace Experiment.Data.Metadata{
	/// <summary>
	/// Token transfer transaction
	/// </summary>
	public interface ITokenTransaction
	{
		/// <summary>
		/// Primary key, Id
		/// </summary>
		Guid Id { get; set; }

		/// <summary>
		/// Transaction date
		/// </summary>
		DateTime Date { get; set; }

		/// <summary>
		/// Sender user Id
		/// </summary>
		string SenderUserId { get; set; }

		/// <summary>
		/// Receiver user Id
		/// </summary>
		string ReceiverUserId { get; set; }

		/// <summary>
		/// Amout of transferred tokens
		/// </summary>
		int Tokens { get; set; }

		/// <summary>
		/// Token transfer transaction status
		/// </summary>
		TokenTransactionStatus Status { get; set; }

		/// <summary>
		/// Last status set user Id
		/// </summary>
		string StatusUserId { get; set; }

	}
}
