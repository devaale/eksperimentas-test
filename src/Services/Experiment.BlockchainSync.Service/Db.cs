using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Experiment.Core;
using Experiment.Core.BL.Data;
using Experiment.Core.IO;
using Experiment.Core.Metadata;
using Experiment.Data.Metadata;
using Experiment.Data.Models;

namespace Experiment.BlockchainSync.Service{
	internal class Db : ExpSql
	{
		#region Const
		const string SQL_USER_LIST = "EXEC [prcUserAll]";
		const string SQL_USER_BALANCE_UPDATE = "[prcUserTokenBalanceUpdate] @userId, @tokens";
		const string SQL_WALLET_PRIMARY_SYSTEM = "[prcWalletPrimarySystem]";
		const string SQL_WALLET_OF_USER = "[prcWalletOfUser] @userId";
		const string SQL_WALLET_UPDATE = "[prcWalletUpdate] @userId, @address, @privateKey, @publicKey";
		const string SQL_BLOCKCHAIN_LOG = "[prcBlockchainLog] @userId, @requestUri, @reqestParams, @result, @status";

		#endregion

		#region Ctor
		internal Db(IDbConnection cn, ILogger logger)
			: base(cn, logger)
		{
		}

		#endregion

		#region Helpers
		protected Wallet? TableToWallet (DataTable table)
		{
			if (table != null)
			{
				if (table.Rows.Count > 0)
				{
					var row = table.Rows[0];

					return new Wallet()
					{
						Id = (Guid)row[nameof(Wallet.Id)],
						UserId = row[nameof(Wallet.UserId)].ToString(),
						Address = row[nameof(Wallet.Address)].ToString(),
						PrivateKey = row[nameof(Wallet.PrivateKey)].ToString(),
						PublicKey = row[nameof(Wallet.PublicKey)].ToString(),
						System = (bool)row[nameof(Wallet.System)],
						Primary = (bool)row[nameof(Wallet.Primary)],
						Created = (DateTime)row[nameof(Wallet.Created)],
					};
				}
			}

			return null;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns primary system wallet, if it exists
		/// </summary>
		/// <returns></returns>
		internal Wallet? GetSystemPrimaryWallet()
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_WALLET_PRIMARY_SYSTEM;
			var table = Query(cmd);
			return TableToWallet(table);
		}

		internal Wallet? GetUserWallet(string userId)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_WALLET_OF_USER;
			AddParameter(cmd, "@userId", userId);
			var table = Query(cmd);
			return TableToWallet(table);
		}       
		
		/// <summary>
				/// Returns all users list
				/// </summary>
				/// <returns></returns>
		internal IEnumerable<User> UsersList()
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_USER_LIST;
			//AddParameter(cmd, "@datapointId", datapointId);
			var table = Query(cmd);

			var result = table.AsEnumerable()
				.Select(row => new User()
				{
					Id = row[nameof(User.Id)].ToString(),
					Name = row[nameof(User.Name)].ToString(),
					Address = DBNull.Value.Equals(row[nameof(User.Address)]) ? string.Empty : row[nameof(User.Address)].ToString(),
					Tokens = (int)row[nameof(User.Tokens)],
				}).ToList();

			return result;
		}

		internal void UserBalanceUpdate(string userId, int tokens)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_USER_BALANCE_UPDATE;  // [prcUserTokenBalanceUpdate] @userId, @tokens
			AddParameter(cmd, "@userId", userId);
			AddParameter(cmd, "@tokens", tokens);
			Execute(cmd);
		}

		/// <summary>
		/// Update user's wallet
		/// </summary>
		/// <param name="wallet"></param>
		internal void WalletUpdate(Wallet wallet)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_WALLET_UPDATE;  // [prcWalletUpdate] @userId, @address, @privateKey, @publicKey
			AddParameter(cmd, "@userId", wallet.UserId);
			AddParameter(cmd, "@address", wallet.Address);
			AddParameter(cmd, "@privateKey", wallet.PrivateKey);
			AddParameter(cmd, "@publicKey", wallet.PublicKey);
			Execute(cmd);
		}

		/// <summary>
		/// Write to blockchain log
		/// </summary>
		/// <param name="log"></param>
		internal void WriteBlockchainLog(BlockchainLog log)
		{
			IDbCommand cmd = Connection.CreateCommand();
			cmd.CommandText = SQL_BLOCKCHAIN_LOG; // [prcBlockchainLog] @userId, @requestUri, @reqestParams, @result, @status
			AddParameter(cmd, "@userId", log.UserId);
			AddParameter(cmd, "@requestUri", log.RequestUri);
			AddParameter(cmd, "@reqestParams", log.ReqestParams);
			AddParameter(cmd, "@result", log.Result);
			AddParameter(cmd, "@status", log.Status);
			Execute(cmd);
		}

		#endregion

		#region Static
		internal static new Db GenerateFromDefaults(ILogger logger)
		{
			// Modified for easier debug
			if (logger == null)
				logger = new DebugLogger(5);

			var cnStr = Defaults.ConnectionString;
			var cn = new SqlConnection(cnStr);
			var retVal = new Db(cn, logger);
			return retVal;
		}

		#endregion
	}
}
