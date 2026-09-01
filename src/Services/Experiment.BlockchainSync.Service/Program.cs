using System.Data;
using System.Diagnostics;

using Newtonsoft.Json;

using Experiment.Core;
using Experiment.Core.BL.Data;
using Experiment.Core.IO;
using Experiment.Core.Metadata;
using Experiment.Data.Models;
using Experiment.Data.Services;

using SH = Experiment.Data.Services.SuperHow;
using Experiment.Data.Services.SuperHow.Data;

namespace Experiment.BlockchainSync.Service{
	class Program
	{
		const int DEFAULT_LOG_LEVEL = 5;

		static ILogger Logger;
		static Db Db;
		static ShService ShService;

		static async Task Main(string[] args)
		{
			var App = Process.GetCurrentProcess();
			var AppName = App.ProcessName;
			var HelloMsg = string.Format("{0} (C) Energus", AppName);

			Console.WriteLine(HelloMsg);
			Console.WriteLine(new string('=', HelloMsg.Length));
			Console.WriteLine();

			if (args.Length < 1)
			{
				Console.WriteLine(string.Format("Usage: \"{0}.exe\" go", AppName));
			}
			else
			{
				Logger = new ConsoleLogger(
					DEFAULT_LOG_LEVEL,
					AppName);
				var vStep = "Init";

				try
				{
					vStep = nameof(Db.GenerateFromDefaults);
					Logger.WriteLine(5, "Generating DB access from defaults..");
					Db = Db.GenerateFromDefaults(Logger);
					Logger.WriteLine(5, string.Format("Success: {0}", !Db.IsError));

					vStep = nameof(ShService);
					Logger.WriteLine(5, string.Format("Initialize {0}...", nameof(ShService)));
					ShService = new ShService();

					vStep = nameof(Db.UsersList);
					Logger.WriteLine(5, "Retrieving all users from DB..");
					var users = Db.UsersList();

					foreach (var user in users)
					{
						vStep = "foreach";
						bool hasBlockchain = false;
						bool validBlockchain = false;
						bool needsTransfer = false;

						Logger.WriteLine(5, "");
						Logger.WriteLine(5, string.Format("** User, Id={0}, Name={1}, Address={2}",
							user.Id,
							user.Name,
							user.Address));

						hasBlockchain = !string.IsNullOrEmpty(user.Address);
						Logger.WriteLine(5, string.Format("Has blockchain: {0}", hasBlockchain));

						// If it has blochchain, we need to verify it
						vStep = nameof(hasBlockchain);
						if (hasBlockchain)
						{
							//logger.WriteLine(5, "* Verifying blockchain account...");
							// If acccount unavailable, it returns JSON OBJECT with error status that object wasn't found
							// If account available, it returns JSON ARRAY, normally with one elemement BUT
							//		In case if user has no any currency, this object will be empty, eg. JSON: [{}]
							//		But this already means, that account available, just no funds
							var userBalance = await ShService.GetBalanceAsync(user.Address);
							validBlockchain = userBalance != null;

							// Now if we something got in return, user has account, but now checking is he has any funds
							if(validBlockchain)
							{
								foreach(var fund in userBalance)
								{
									if(SH.Defaults.ENERGUS_MOSAIC_ID.Equals(fund.MosaicId))
									{
										var tokenAmmount = fund.amount / SH.Defaults.TOKENS_MULTIPLIER;
										if(tokenAmmount != user.Tokens)
										{
											Logger.WriteLine(4, string.Format(
												"User token amount and blokchain token amount not matching, updating it to {0}...", tokenAmmount));

											Db.UserBalanceUpdate(user.Id, tokenAmmount);
										}

										break;
									}
								}
							}

							Logger.WriteLine(5, string.Format("Valid blockchain: {0}", validBlockchain));
						}

						// If no blockchain or valid blockchain?
						if (!hasBlockchain || !validBlockchain)
						{
							Logger.WriteLine(5, string.Format("Creating User's Blokchain account..."));
							var state = await ShService.AccountCreateAsync(user.Id);

							Logger.WriteLine(5, string.Format("Deserializing the result.."));
							var accountCreateResult = JsonConvert.DeserializeObject<ShUserInfo>(state.ResultJson);

							var accountSuccessfulCreated = state.Response.IsSuccessStatusCode && accountCreateResult != null;
							if (accountSuccessfulCreated)
							{
								accountSuccessfulCreated = accountCreateResult.IsAddressOk;
							}

							Logger.WriteLine(5, string.Format("User Id: {0}, Name: {1}, new Address is {2}",
								user.Id, user.Name, accountCreateResult.GetAddress()));

							if (accountSuccessfulCreated)
							{
								var log = new BlockchainLog()
								{
									UserId = user.Id,
									RequestUri = state.Url,
									ReqestParams = state.RequestJson,
									Result = state.ResultJson,
									Status = state.Response.StatusCode,
								};

								Logger.WriteLine(5, "Writing Blockchain Log...");
								Db.WriteBlockchainLog(log);

								var wallet = new Wallet()
								{
									UserId = user.Id,
									Address = accountCreateResult.GetAddress(),
									System = false,
									Primary = false,
									PrivateKey = accountCreateResult.PrivateKey,
									PublicKey = accountCreateResult.PublicKey,
								};
								Logger.WriteLine(5, "Updating wallet...");
								Db.WalletUpdate(wallet);

								var blokchainAmmount = SH.Defaults.DEFAULT_AMOUNT;
								var tokenAmmount = SH.Defaults.DEFAULT_AMOUNT / SH.Defaults.TOKENS_MULTIPLIER;

								//Logger.WriteLine(4, string.Format("[Blockchain] Sending from system account: {1}...",
								//	blokchainAmmount));
								await SendFromSystemAccount(user.Id, blokchainAmmount);

								Logger.WriteLine(4, string.Format(
											"Setting user's token balance from Blockchain to {0}", 
											tokenAmmount));
								Db.UserBalanceUpdate(user.Id, tokenAmmount);
							}
							else
							{
								Logger.WriteLine(0, string.Format("Failed to create Blockchain account for User, Id={0}, Name={1}",
									user.Id, user.Name));
							}
						}

						// Rest delay
						Thread.Sleep(1 * 1000);
					}

				}
				catch (Exception ex)
				{
					Logger.WriteLine(0, string.Format("{0}, {1}", vStep, ex.Message));
				}
			}

			Console.WriteLine();
			Console.WriteLine("Press Any Key To Continue...");
			Console.ReadKey();
		}

		/// <summary>
		/// Send tokens from primary system wallet
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		internal static async Task<bool> SendFromSystemAccount(string userId, int amount)
		{
			// Retrieving system sender
			var sender = Db.GetSystemPrimaryWallet();

			if (sender == null)
				return false; // Execute on your db: "Arvydas 2023-05-08 0010 wall.sql"

			var receiverWallet = Db.GetUserWallet(userId);
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

			var state = await  ShService.Send(userId, sendRequest);
			if (state != null)
			{
				var log = new BlockchainLog()
				{
					UserId = userId,
					RequestUri = state.Url,
					ReqestParams = state.RequestJson,
					Result = state.ResultJson,
					Status = state.Response.StatusCode
				};
				Db.WriteBlockchainLog(log);
			}
			return state != null;
		}
	}

}
