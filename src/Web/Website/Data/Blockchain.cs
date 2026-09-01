using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services.Description;

using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

using Experiment.Core;
using Experiment.Core.Text;

using Experiment.Data.Enums;
using M = Experiment.Data.Models;
using Experiment.Data.Services;

using SH=Experiment.Data.Services.SuperHow;
using Experiment.Data.Services.SuperHow.Data;
using Experiment.Data.Services.SuperHow.Metadata;

using Website.Models;
using Website.Controllers;
using System.Data.Entity.Migrations;

namespace Website.Data
{
	/// <summary>
	/// Blockchain
	/// </summary>
	public class Blockchain : SuperHowService
	{
		#region Const
		internal const string TYPE_NAME = nameof(Blockchain);

		#endregion

		#region Attributes
		ApplicationDbContext db;

		/// <summary>
		/// C# Interactive
		/// System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("https://gw.energus.superhow.net/api"))
		/// </summary>
		const string _UrlService = "aHR0cHM6Ly9ndy5lbmVyZ3VzLnN1cGVyaG93Lm5ldC9hcGk=";

		#endregion

		#region Properties

		#endregion

		#region Ctor

		public Blockchain()
		{
			db = new ApplicationDbContext();
		}

		public Blockchain(ApplicationDbContext db)
		{
			if (db == null)
			{
				throw new ArgumentNullException(
					string.Format("{0} parameter must be valid {0} instance!",
					nameof(db), nameof(ApplicationDbContext))
				);
			}

			this.db = db;
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Main system, primary wallet retrieval
		/// </summary>
		/// <returns></returns>
		internal Wallet GetSystemPrimaryWallet()
		{
			return db.Wallets.Where(w =>
				w.System == true &&
				w.Primary == true).FirstOrDefault();
		}

		/*
		/// <summary>
		/// Send
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		protected async Task<int> Send(string userId, IShSendRequest request)
		{

			if (request == null)
				return 10; // No request itself given, nothing to send

			var json = !(request.Message is string);

			var client = CreateHttpClient();
			var requestJson = JsonConvert.SerializeObject(request);

			HttpContent content = new StringContent(requestJson);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			// Sending the request
			var url = (!json ? UrlTransactionSend : UrlTransactionSendJson);
			var requestResponse = await client.PutAsync(url, content);

			// Receiving the response
			string resultJson = await requestResponse.Content.ReadAsStringAsync();

			var log = new BlockchainLog()
			{
				UserId = userId,
				RequestUri = url,
				ReqestParams = requestJson,
				Result = resultJson,
				Status = requestResponse.StatusCode
			};
			WriteLog(log);

			return 0;
		}*/

		/// <summary>
		/// After email confirm we linking new user to blochain
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>requires to transfer coins</returns>
		protected async Task<bool> LinkAccountToBlockchain(string userId)
		{
			var retVal = false;
			Wallet wallet;

			try
			{
				wallet = db.Wallets.Where(w => w.UserId == userId).FirstOrDefault();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return false;
			}

			if (wallet == null)
			{
				var state = await AccountCreateAsync(userId);
				//var client = CreateHttpClient();
				//var requestJson = JsonConvert.SerializeObject(new ShMessageRequest() { Message = userId });

				//HttpContent content = new StringContent(requestJson);
				//content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

				// Sending the request
				//var url = UrlAccountCreate;
				//var requestResponse = await client.PostAsync(url, content);

				// Receiving the response
				//string resultJson = await requestResponse.Content.ReadAsStringAsync();

				var log = new BlockchainLog()
				{
					UserId = userId,
					RequestUri = state.Url,
					ReqestParams = state.RequestJson,
					Result = state.ResultJson,
					Status = state.Response.StatusCode,
				};
				WriteLog(log);

				if (state.Response.IsSuccessStatusCode)
				{
					var oResult = JsonConvert.DeserializeObject<ShUserInfo>(state.ResultJson);
					wallet = new Wallet()
					{
						UserId = userId,
						Address = oResult.Address.Address,
						System = false,
						Primary = false,
						PrivateKey = oResult.PrivateKey,
						PublicKey = oResult.PublicKey,
					};

					db.Wallets.Add(wallet);
					db.SaveChanges();

					retVal = true;
				}
			}

			return retVal;
		}

		/// <summary>
		/// Send tokens from primary system wallet
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		protected async Task<bool> SendFromSystemAccount(string userId, int amount)
		{
			// Retrieving system sender
			var sender = GetSystemPrimaryWallet();

			if (sender == null)
				return false; // Execute on your db: "Arvydas 2023-05-08 0010 wall.sql"

			var receiverWallet = db.Wallets.Where(rw => rw.UserId == userId).FirstOrDefault();
			// This will be null if user blockchain wasn't linked yet or not yet completed link
			if (receiverWallet == null)
				return false;

			var sendRequest = new ShSendRequest()
			{
				SenderPrivateKey = sender.PrivateKey,
				RecipientAddress = receiverWallet.Address,
				Amount = amount,
				NamespaceName = SH.Defaults.ENERGUS_MOSAIC_NAME,
				Message = "Tokens for account creation and email verification.",
			};

			var state = await Send(userId, sendRequest);
			if(state != null)
			{
                var user = db.Users.Find(userId);
                user.Tokens = SH.Defaults.DEFAULT_TOKENS;
                db.Users.AddOrUpdate(user);

				var systemAccountBalances = await GetBalanceAsync(sender.Address);
				var balanceAmount = systemAccountBalances.Where(b => b.MosaicId.Equals(SH.Defaults.ENERGUS_MOSAIC_ID)).FirstOrDefault().amount;
                var systemUser = db.Users.Find(sender.UserId);
                systemUser.Tokens = balanceAmount / SH.Defaults.TOKENS_MULTIPLIER;
                db.Users.AddOrUpdate(systemUser);

                db.SaveChanges();

                var log = new BlockchainLog()
				{
					UserId = userId,
					RequestUri = state.Url,
					ReqestParams = state.RequestJson,
					Result = state.ResultJson,
					Status = state.Response.StatusCode
				};
				WriteLog(log);
			}
			return state != null;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Write record to tblBlockchainLog
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="requestUri"></param>
		/// <param name="reqestParams"></param>
		/// <param name="result"></param>
		public void WriteLog (string userId, string requestUri, string reqestParams, string result)
		{
			var log = new BlockchainLog()
			{
				UserId = userId,
				RequestUri = requestUri,
				ReqestParams = reqestParams,
				Result = result,
			};

			WriteLog(log);
		}

		/// <summary>
		/// Write record to tblBlockchainLog
		/// </summary>
		/// <param name="log"></param>
		public void WriteLog(BlockchainLog log)
		{
			if(log != null)
			{
				db.BlockchainLogs.Add(log);
				db.SaveChanges();
			}
		}

		/// <summary>
		/// Sending tokens from one to another user.
		/// 
		/// Should be authorized
		/// </summary>
		/// <param name="currentUser"></param>
		/// <param name="receiverUserId"></param>
		/// <param name="senderUserId"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		public async Task<bool> Send(IIdentity currentUser, string receiverUserId, string senderUserId, int amount)
		{
			// Should be authorized and IIdentity should present
			if (currentUser == null)
				return false;

			// Only curent user can be sender
			var currentUserId = currentUser.GetUserId();
			if (!currentUserId.Equals(senderUserId))
				return false;

			var sender = db.Wallets.Where(w => w.UserId.Equals(senderUserId)).FirstOrDefault();
			// Sender user available in db?
			if (sender == null)
				return false;

			var receiver = db.Wallets.Where(w => w.UserId.Equals(receiverUserId)).FirstOrDefault();
			// receiver user available in db?
			if (receiver == null)
				return false;
			var request = new ShSendRequest()
			{
				SenderPrivateKey = sender.PrivateKey,
				RecipientAddress = receiver.Address,
				Amount = SH.Defaults.TOKENS_MULTIPLIER * amount,
				NamespaceName = SH.Defaults.ENERGUS_MOSAIC_NAME,
				Message = string.Format("{0} sent {1} to {2}", senderUserId, amount, receiverUserId),
			};

			var state = await Send(currentUserId, request);
			if (state != null)
			{
				var log = new BlockchainLog()
				{
					UserId = currentUserId,
					RequestUri = state.Url,
					ReqestParams = state.RequestJson,
					Result = state.ResultJson,
					Status = state.Response.StatusCode
				};
				WriteLog(log);
			}
			return state != null;
		}

		/// <summary>
		/// Called after user's email just was confirmed.
		/// 
		/// This means that we need:
		/// 
		/// 1. To link this user account to blochain;
		/// 2. To transfer to it default amount of tokens.
		/// </summary>
		/// <param name="currentUser"></param>
		/// <returns></returns>
		public async Task<bool> EmailConfirmed(string userId)
		{
			var proceed = await LinkAccountToBlockchain(userId);
			if (!proceed)
				return proceed;

			proceed = await SendFromSystemAccount(userId, SH.Defaults.DEFAULT_AMOUNT);

            return proceed;
		}

		public async Task<bool> OrderCompleted(Guid orderId)
		{
			// Should exist such order record
			var order = db.Orders.Find(orderId);
			if (order == null)
				return false;

			// Every order has one details record
			var orderDetails = db.OrderDetails.Where(od => od.OrderId.Equals(orderId)).FirstOrDefault();
			if (orderDetails == null)
				return false;

			// Get sysetrm primary wallet, where used tokens should return
			var receiver = GetSystemPrimaryWallet();
			if (receiver == null)
				return false;

			// Get sender's wallet
			var sender = db.Wallets.Where(w => w.UserId.Equals(order.UserId)).FirstOrDefault();
			if (sender == null)
				return false;

			// Forming blochain request
			var request = new ShSendRequest()
			{
				SenderPrivateKey = sender.PrivateKey,
				RecipientAddress = receiver.Address,
				Amount = SH.Defaults.TOKENS_MULTIPLIER * order.UsedTokens,
				NamespaceName = SH.Defaults.ENERGUS_MOSAIC_NAME,
				Message = new ShSendJsonInfo()
				{
					Id = order.Id,
					UserId = order.UserId,
					OrderNo = order.OrderNo,
					FullPrice = order.FullPrice,
					UsedTokens = order.UsedTokens,
					Discount = order.Discount,
					FinalPrice = order.FinalPrice,
					PaymentMethodId = order.PaymentMethodId,
					Posted = order.Posted,

					// OrderDetails part
					LicenseType = orderDetails.LicenseType,
					NumMonths = orderDetails.NumMonths,
				},
			};

			var state = await Send(order.UserId, request);
			if (state != null)
			{
				var log = new BlockchainLog()
				{
					UserId = order.UserId,
					RequestUri = state.Url,
					ReqestParams = state.RequestJson,
					Result = state.ResultJson,
					Status = state.Response.StatusCode
				};
				WriteLog(log);
			}
			return state != null;
		}

		#endregion
	}
}