using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class TokenTransaction : ITokenTransaction
	{
		/// <summary>
		/// Primary key, Id
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Transaction date
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// Sender user Id
		/// </summary>
		public string SenderUserId { get; set; }

		/// <summary>
		/// Receiver user Id
		/// </summary>
		public string ReceiverUserId { get; set; }

		/// <summary>
		/// Amout of transferred tokens
		/// </summary>
		public int Tokens { get; set; }

		/// <summary>
		/// Token transfer transaction status
		/// </summary>
		public TokenTransactionStatus Status { get; set; }

		/// <summary>
		/// Last status set user Id
		/// </summary>
		public string StatusUserId { get; set; }

		public TokenTransaction()
		{
			Id = Guid.NewGuid();
		}
	}
}
