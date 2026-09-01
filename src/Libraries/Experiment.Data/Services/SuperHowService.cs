using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Experiment.Data.Services.SuperHow.Metadata;
using Experiment.Data.Services.SuperHow.Data;
using Experiment.Core.Text;
using Experiment.Core.Metadata;
using Experiment.Data.Models;
using Experiment.Core.Web;

namespace Experiment.Data.Services{
	/// <summary>
	/// @DEPRECATED currently unused 
	/// @TODO: DELETE IT
	/// </summary>
	public class SuperHowService
	{
		#region Constants
		const string TYPE_NAME = nameof(SuperHowService);

        /// <summary>
        /// C# Interactive
        /// System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("http://68.219.208.47:4000/api"))
        /// </summary>
        const string _UrlService = "aHR0cDovLzY4LjIxOS4yMDguNDc6NDAwMC9hcGk=";

		#endregion

		#region Properties

		/// <summary>
		/// Base URL
		/// </summary>
		public string UrlService { get => Base64.Instance.Decode(_UrlService); }

		// Account
		public string UrlAccount { get => UrlService + "/account"; }
		public string UrlAccountCreate { get => UrlAccount + "/create"; }
		public string UrlGetBalance { get => UrlAccount + "/getBalance"; }

		// Transaction
		public string UrlTransaction { get => UrlService + "/transaction"; }
		public string UrlTransactionSend { get => UrlTransaction + "/send"; }
		public string UrlTransactionSendJson { get => UrlTransaction + "/sendJSON"; }
		public string UrlTransactionSendMosaic { get => UrlTransaction + "/sendMosaic"; }

		#endregion

		#region Helpers
		protected virtual HttpClient CreateHttpClient()
		{
			var retVal = new HttpClient();
			//retVal.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Settings.LoginTokenType, Settings.LoginToken);
			return retVal;
		}


		/// <summary>
		/// Send
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		public virtual async Task<HttpRequestState> Send(string userId, IShSendRequest request)
		{
			if (request == null)
				return null;

			var json = !(request.Message is string);

			var state = new HttpRequestState();
			var client = CreateHttpClient();
			state.RequestJson = JsonConvert.SerializeObject(request);

			HttpContent content = new StringContent(state.RequestJson);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			// Sending the request
			state.Url = (!json ? UrlTransactionSend : UrlTransactionSendJson);
			state.Response = await client.PutAsync(state.Url, content);

			// Receiving the response
			state.ResultJson = await state.Response.Content.ReadAsStringAsync();
			return state;
		}

		#endregion

		#region Methods

		public virtual async Task<HttpRequestState> AccountCreateAsync(string userId)
		{
			var state = new HttpRequestState();
			var client = CreateHttpClient();
			state.RequestJson = JsonConvert.SerializeObject(new ShUserIdRequest() { UserId = userId });

			HttpContent content = new StringContent(state.RequestJson);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			// Sending the request
			state.Url = UrlAccountCreate;
			state.Response = await client.PostAsync(state.Url, content);

			// Receiving the response
			state.ResultJson = await state.Response.Content.ReadAsStringAsync();
			return state;
		}

		public virtual async Task<ShBalanceInfo[]> GetBalanceAsync(string address)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(GetBalanceAsync));
			ShBalanceInfo[] result = null;

			var client = CreateHttpClient();
			var url = string.Format("{0}/{1}", UrlGetBalance, address);
			var requestResponse = await client.GetAsync(url);
			string rJson = await requestResponse.Content.ReadAsStringAsync();
			try
			{
				// If this deserialization not fails, user account exists
				result = JsonConvert.DeserializeObject<ShBalanceInfo[]>(rJson);
			}
			catch { }
			return result;
		}
		#endregion
	}
}
