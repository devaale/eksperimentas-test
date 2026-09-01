#define USE_JSON_NEWTON
#define PAYSERA_TEST
//#define AUTHORIZE_PRE_DATA	// Don't enable as user might be not logged in

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

#if USE_JSON_NEWTON
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
#else
using System.Text.Json;
#endif

using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

using EVP.WebToPay.ClientAPI;

using Experiment.Core;
using Experiment.Data.Metadata;

// MVVM
using Experiment.Data.Enums;
using Experiment.Data.Models;

using M = Experiment.Data.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Enums;
using Experiment.Maui.Models;
using Experiment.Maui.ViewModels.Devices;

namespace Experiment.Maui.Services{
	public class ApiServices
	{
		#region Constants
		const string TYPE_NAME = nameof(ApiServices);
		const bool DEBUG = true;

		/// <summary>
		/// Back-end Url
		/// </summary>
		internal static string ServerWebUrl { get => Settings.Server; }

		/// <summary>
		/// Back-end API url (eg. hostname/
		/// </summary>
		internal static string ServerApiUrl { get => ServerWebUrl + Defaults.URL_API_ADDON; }

		// Words
		internal static string LanguageApiUrl { get => ServerApiUrl + "Language"; }
		internal static string WordApiUrl { get => ServerApiUrl + "Word"; }

		// Users
		internal static string TokenApiUrl { get => ServerWebUrl + "Token"; }  // Warning: this host is different
		internal static string AccountApiUrl { get => ServerApiUrl + "Account/"; }
		internal static string RegisterApiUrl { get => AccountApiUrl + "Register"; }
		internal static string UserApiUrl { get => ServerApiUrl + "User"; }

		// Licensing
		internal static string LicenseApiUrl { get => ServerApiUrl + "License"; }

		// Objects
		internal static string ObjectApiUrl { get => ServerApiUrl + "Object"; }

		// Groups
		internal static string GroupApiUrl { get => ServerApiUrl + "Group"; }

		// Algorithms
		internal static string AlgorithmApiUrl { get => ServerApiUrl + "Algorithm"; }

		// Alarms
		internal static string AlarmApiUrl { get => ServerApiUrl + "Alarm"; }

		// Devices
		internal static string DeviceApiUrl { get => ServerApiUrl + "Device"; }

		// DeviceTopic
		internal static string DeviceTopicApiUrl { get => ServerApiUrl + "DeviceTopic"; }

		// Datapoints
		internal static string DatapointApiUrl { get => ServerApiUrl + "Datapoint"; }
		internal static string DatapointChartUrl { get => DatapointApiUrl + "/Chart"; }
		internal static string ByDevicesApiUrl { get => DatapointApiUrl + "/ByDevices"; }
		internal static string ByObjectApiUrl { get => DatapointApiUrl + "/ByObject"; }

		// Dashboard
		internal static string DashboardApiUrl { get => ServerApiUrl + "Dashboard"; }

		// DatapointValue
		internal static string DatapointValueApiUrl { get => ServerApiUrl + "DatapointValue"; }
		internal static string DatapointValueSearchApiUrl { get => DatapointValueApiUrl + "/Search"; }
		internal static string DatapointValueDownloadApiUrl { get => DatapointValueApiUrl + "/Download"; }
		/// <summary>
		/// @deprecated - used only in dashboard (remove it)
		/// </summary>
		internal static string DatapointValueValuesApiUrl { get => DatapointValueApiUrl + "/Values"; }

		// Ecosystem
		internal static string PostApiUrl { get => ServerApiUrl + "Post"; }
		internal static string PostImageApiUrl { get => ServerApiUrl + "PostImage"; }
		internal static string PostImageApiUrlRaw { get => PostImageApiUrl + "/Raw"; }
		internal static string PostReactionApiUrl { get => ServerApiUrl + "PostReaction"; }

		// Friends
		internal static string FriendApiUrl { get => ServerApiUrl + "Friend"; }

		// Blocked
		internal static string BlockedApiUrl { get => ServerApiUrl + "Blocked"; }

		// Messages
		internal static string MessageApiUrl { get => ServerApiUrl + "Message"; }

		// Situation
		internal static string SituationApiUrl { get => ServerApiUrl + "Situation"; }

		// Token transaction
		internal static string TokenTransactionApiUrl { get => ServerApiUrl + "TokenTransaction"; }

		// Payments
		internal static string PaymentUrl { get => ServerWebUrl + "Payment/"; }
		internal static string PaymentAcceptUrl { get => PaymentUrl + "Accept"; }
		internal static string PaymentCancelUrl { get => PaymentUrl + "Cancel"; }

		internal static string BillingApiUrl { get => ServerApiUrl + "Billing"; }
		internal static string BillingApiConfirmOrderUrl { get => BillingApiUrl + "/ConfirmOrder"; }

		internal static string DatapointFormulaApiUrl { get => ServerApiUrl + "DatapointFormula/"; }

		#endregion

		#region Attributes

		#endregion

		#region Helpers
		/// <summary>
		/// Get Authorized HTTP client
		/// </summary>
		/// <returns></returns>
		internal HttpClient GetHttpClient(bool withSecurity = true)
		{
			var retVal = new HttpClient();
			retVal.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
			if(withSecurity)
			{
				retVal.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
					Settings.LoginTokenType, Settings.LoginToken);
			}
			return retVal;
		}

		#endregion

		#region Multilingual

		/// <summary>
		/// Retrieves all the languages
		/// </summary>
		/// <returns></returns>
		internal async Task<List<M.Language>> LanguageListAsync()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(LanguageListAsync));
			Debug.WriteLine(vLoc + ": " + LanguageApiUrl);

			var client = GetHttpClient(false);  // This doesn't require authorization, as language list is used in registration, when user isn't logged in
			var json = await client.GetStringAsync(LanguageApiUrl);
			var words = JsonConvert.DeserializeObject<List<M.Language>>(json);
			return words;
		}

		/// <summary>
		/// Updates current language words
		/// </summary>
		/// <returns></returns>
		internal async Task UpdateWordsAsync()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(UpdateWordsAsync));
			var url = WordApiUrl + "?lang=" + Settings.Language;
			Debug.WriteLine(string.Format("{0}: {1}", vLoc, url));

			E.Loaded = false;
			E.Words.Clear();

#if AUTHORIZE_PRE_DATA
			var client = GetAuthorizedHttpClient();
#else
			var client = GetHttpClient(false);  // This doesn't require authorization, as ML words present even if user isn't logged in
#endif
			var json = await client.GetStringAsync(url);
			var words = JsonConvert.DeserializeObject<List<M.Word>>(json);

			foreach (var word in words)
			{
				if (!E.Words.ContainsKey(word.Alias))
				{
					E.Words.Add(word.Alias, word.Text);
				}
				else
				{
					Debug.WriteLine(string.Format(
						"{0}, for some reason alias={1} was already added!",
						vLoc, word.Alias));
					E.Words[word.Alias] = word.Text;
				}
			}
			E.Loaded = true;
		}

		#endregion

		#region Objects
		/*
		public async Task<M.Object> ObjectNewAsync()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(ObjectNewAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = await client.GetStringAsync(ObjectApiUrl + "/New");
			var record = JsonConvert.DeserializeObject<M.Object>(json);
			return record;
		}
		*/
		public async Task<List<M.Object>> ObjectListAsync()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(ObjectListAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = await client.GetStringAsync(ObjectApiUrl);
			var records = JsonConvert.DeserializeObject<List<M.Object>>(json);

			var user = await Dictionaries.Instance.GetCurrentUser(false);

			foreach (var r in records)
				r.IsOwnedObject = user.Id.Equals(r.UserId);

			return records;
		}

		public async Task<HttpResponseMessage> ObjectPostAsync(IObject record)
		{
			var vLoc = string.Format("{0}::{1}(IObject record={2})", TYPE_NAME, nameof(ObjectPostAsync), record);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(record);
			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(ObjectApiUrl, content);
			return response;
		}

		public async Task<HttpResponseMessage> ObjectPutAsync(IObject record)
		{
			var vLoc = string.Format("{0}::{1}(IObject record={2})", TYPE_NAME, nameof(ObjectPutAsync), record);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(record);

			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PutAsync(
				ObjectApiUrl + "/" + record.Id, content);
			return response;

		}

		/// <summary>
		/// Delete existing friend
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public async Task<HttpResponseMessage> ObjectDeleteAsync(IObject record)
		{
			var vLoc = string.Format("{0}::{1}(IObject record={2})", TYPE_NAME, nameof(ObjectDeleteAsync), record);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var response = await client.DeleteAsync($"{ObjectApiUrl}/{record.Id}");
			return response;
		}

        public async Task<HttpResponseMessage> ObjectEnableAiAsync(IObject record)
        {
            var vLoc = string.Format("{0}::{1}(IObject record={2})", TYPE_NAME, nameof(ObjectEnableAiAsync), record);
            Debug.WriteLine(vLoc);

            var client = GetHttpClient();
            var json = JsonConvert.SerializeObject(record.Id);

            var response = await client.GetAsync($"{ObjectApiUrl}/Ai?id={record.Id}");
            return response;
        }

        #endregion

        #region Groups
        public async Task<M.Group> GroupNewAsync(int objectId)
		{
			var vLoc = TYPE_NAME + nameof(GroupNewAsync);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = await client.GetStringAsync(GroupApiUrl + "/New?objectId=" + objectId.ToString());
			var record = JsonConvert.DeserializeObject<M.Group>(json);
			return record;
		}

		public async Task<List<M.Group>> GroupListAsync(int objectId)
		{
			var vLoc = TYPE_NAME + nameof(GroupListAsync);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var url = GroupApiUrl + "?objectId=" + objectId;
			Debug.WriteLine(string.Format("{0}, {1}", vLoc, url));
			var json = await client.GetStringAsync(url);
			var records = JsonConvert.DeserializeObject<List<M.Group>>(json);
			return records;
		}

		// Create New Group
		public async Task<HttpResponseMessage> GroupPostAsync(IGroup record)
		{
			var vLoc = TYPE_NAME + "::GroupPostAsync(IGroup record)";
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(record);
			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(GroupApiUrl, content);
			return response;
		}

		// Edit
		public async Task<HttpResponseMessage> GroupPutAsync(IGroup record)
		{
			var vLoc = TYPE_NAME + "::GroupPutAsync(IGroup record)";
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(record);

			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PutAsync(
				GroupApiUrl + "/" + record.Id, content);
			return response;
		}

		/// <summary>
		/// Delete existing group
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public async Task<HttpResponseMessage> GroupDeleteAsync(IGroup record)
		{
			var vLoc = TYPE_NAME + "::GroupDeleteAsync(IGroup record)";
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var response = await client.DeleteAsync(
				GroupApiUrl + "/" + record.Id);
			return response;
		}

		#endregion

		#region Algortihms

		// Not used. I leave it for perspective.
		public async Task<VisualAlgorithm> AlgorithmNewAsync()
		{
			var vLoc = TYPE_NAME + "::AlgorithmNewAsync()";
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = await client.GetStringAsync(AlgorithmApiUrl + "/New");
			var record = JsonConvert.DeserializeObject<VisualAlgorithm>(json);
			return record;
		}

		public async Task<List<VisualAlgorithm>> AlgortihmsListAsync(string objectId)
		{
			var vLoc = TYPE_NAME + "::AlgortihmListAsync()";
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			string test = AlgorithmApiUrl + "/All?objectId=" + objectId;

			var json = await client.GetStringAsync(AlgorithmApiUrl + "/All?objectId=" + objectId);
			var records = JsonConvert.DeserializeObject<List<VisualAlgorithm>>(json);
			return records;
		}

		// New
		public async Task<HttpResponseMessage> AlgorithmPostAsync(IAlgorithm record)
		{
			var vLoc = TYPE_NAME + "::AlgorithmPostAsync(IAlgorithm record)";
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(record);
			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(AlgorithmApiUrl, content);
			return response;
		}

		// Edit
		public async Task<HttpResponseMessage> AlgorithmPutAsync(IAlgorithm record)
		{
			var vLoc = TYPE_NAME + "::AlgorithmPutAsync(IAlgorithm record)";
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(record);

			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PutAsync(
				AlgorithmApiUrl + "/" + record.Id, content);
			return response;
		}

		/// <summary>
		/// Delete existing algorithm
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public async Task<HttpResponseMessage> AlgorithmDeleteAsync(IAlgorithm record)
		{
			var vLoc = TYPE_NAME + "::AlgorithmDeleteAsync(IAlgorithm record)";
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var response = await client.DeleteAsync(
				AlgorithmApiUrl + "/" + record.Id);
			return response;
		}

		#endregion

		#region Alarms

		public async Task<List<VisualAlgorithm>> AlarmsListAsync(int objectId)
		{
			var vLoc = string.Format("{0}::{1}(int objectId={2})",
				TYPE_NAME, nameof(AlarmsListAsync), objectId);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var url = string.Format("{0}?objectId={1}", AlarmApiUrl, objectId);
			Debug.WriteLine(string.Format("{0}, {1}: {2}", vLoc, nameof(url), url));

			var rJson = await client.GetStringAsync(url);
			Debug.WriteLine(string.Format("{0}, {1}: {2}", vLoc, nameof(rJson), rJson));

			var records = JsonConvert.DeserializeObject<List<VisualAlgorithm>>(rJson);
			return records;
		}

		public async Task<List<VisualAlgorithm>> AlarmsListAsync(int objectId, decimal status)
		{
			var vLoc = string.Format("{0}::{1}(int objectId={2}, decimal status={3})",
				TYPE_NAME, nameof(AlarmsListAsync), objectId, status);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var url = string.Format("{0}?objectId={1}&status={2}", AlarmApiUrl, objectId, status);
			Debug.WriteLine(string.Format("{0}, {1}: {2}", vLoc, nameof(url), url));

			var rJson = await client.GetStringAsync(url);
			Debug.WriteLine(string.Format("{0}, {1}: {2}", vLoc, nameof(rJson), rJson));

			var records = JsonConvert.DeserializeObject<List<VisualAlgorithm>>(rJson);
			return records;
		}

		public async Task<int> AlarmsSituationForMenuAsync(string objectId, decimal status)
		{
			var vLoc = string.Format("{0}(string objectId={1}, decimal status={2})",
				TYPE_NAME, objectId, status);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var url = string.Format("{0}?ObjectId={1}&Status={2}",
				AlarmApiUrl, objectId, status);
			Debug.WriteLine(vLoc, url);

			var json = await client.GetStringAsync(url);
			var records = JsonConvert.DeserializeObject<List<VisualAlgorithm>>(json);

			int result = 0;

			foreach (VisualAlgorithm rec in records)
			{
				if (rec.Read == null)
					result++;
			}
			return result;
		}

		public async Task<bool> AlarmsReadAsync(int objectId)
		{
			var vLoc = string.Format("{0}::{1}(int objectId={2})", TYPE_NAME, nameof(AlarmsReadAsync), objectId);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var url = string.Format("{0}/Read?objectId={1}", AlarmApiUrl, objectId);
			var result = await client.GetAsync(url);
			return result.IsSuccessStatusCode;
		}

		#endregion

		#region Devices

		/// <summary>
		/// Get devices list
		/// </summary>
		/// <returns></returns>
		public async Task<List<VisualDevice>> DeviceListAsync(string objectId)
		{
			var vLoc = string.Format("{0}::{1}(string objectId={2})", TYPE_NAME, nameof(DeviceListAsync), objectId);
			Debug.WriteLine(vLoc);

			//var request = new HttpRequestMessage(HttpMethod.Post, URL_API_DEVICE);
			//var response = await client.SendAsync(request);
			var client = GetHttpClient();
			var url = string.Format("{0}/All?objectIds={1}", DeviceApiUrl, objectId);

			Debug.WriteLine(vLoc + ", " + url);
			var json = await client.GetStringAsync(url);
			var devices = JsonConvert.DeserializeObject<List<VisualDevice>>(json);
			return devices;
		}

		/// <summary>
		/// Datapoint lists
		/// </summary>
		/// <param name="deviceIds"></param>
		/// <returns></returns>
		public async Task<List<DatapointViewModel>> DatapointListByDeviceIdsAsync(string deviceIds)
		{
			var vLoc = string.Format("{0}::{1}(string deviceIds={2})", TYPE_NAME, nameof(DatapointListByDeviceIdsAsync), deviceIds);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = await client.GetStringAsync(DatapointApiUrl + "?deviceIds=" + deviceIds.ToString());
			Debug.WriteLine(string.Format("{0}, {1}", vLoc, json));
			var records = JsonConvert.DeserializeObject<List<DatapointViewModel>>(json);
			return records;
		}

		public async Task<List<M.Datapoint>> DatapointListByObjectIdAsync(string objectId)
		{
			var vLoc = string.Format("{0}::{1}(string objectId={2})", TYPE_NAME, nameof(DatapointListByObjectIdAsync), objectId);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = await client.GetStringAsync(DatapointApiUrl + "?objectId=" + objectId.ToString());
			var records = JsonConvert.DeserializeObject<List<M.Datapoint>>(json);
			return records;
		}

		public async Task<List<M.Datapoint>> DatapointListAsync()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(DatapointListAsync));
			Debug.WriteLine(vLoc);

			var result = await DatapointListByObjectIdAsync(Settings.ObjectId.ToString());
			return result;
		}

		/// <summary>
		/// Save new device
		/// </summary>
		/// <param name="device"></param>
		/// <returns></returns>
		public async Task<HttpResponseMessage> DevicePostAsync(M.Device device)
		{
			var vLoc = string.Format("{0}::{1}(M.Device device={2})", TYPE_NAME, nameof(DevicePostAsync), device);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(device);
			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(DeviceApiUrl, content);
			return response;
		}

		/// <summary>
		/// Update existing device
		/// </summary>
		/// <param name="device"></param>
		/// <returns></returns>
		public async Task<HttpResponseMessage> DevicePutAsync(M.Device device)
		{
			var vLoc = string.Format("{0}::{1}(M.Device device={2})", TYPE_NAME, nameof(DevicePutAsync), device);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(device);

			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PutAsync(
				DeviceApiUrl + "/" + device.Id, content);
			return response;
		}

		/// <summary>
		/// Delete existing device
		/// </summary>
		/// <param name="device"></param>
		/// <returns></returns>
		public async Task<HttpResponseMessage> DeviceDeleteAsync(M.Device device)
		{
			var vLoc = string.Format("{0}::{1}(M.Device device={2})", TYPE_NAME, nameof(DeviceDeleteAsync), device);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var response = await client.DeleteAsync(
				DeviceApiUrl + "/" + device.Id);
			return response;
		}

		/// <summary>
		/// Save new datapoint
		/// </summary>
		/// <param name="datapoint"></param>
		/// <returns></returns>
		public async Task<HttpResponseMessage> DatapointPostAsync(M.Datapoint datapoint)
		{
			var vLoc = string.Format("{0}::{1}(M.Datapoint datapoint={2})", TYPE_NAME, nameof(DatapointPostAsync), datapoint);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(datapoint);
			Debug.WriteLine(vLoc + ", JSON: " + json);

			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(DatapointApiUrl, content);
			return response;
		}

		/// <summary>
		/// Update existing datapoint
		/// </summary>
		/// <param name="datapoint"></param>
		/// <returns></returns>
		public async Task<HttpResponseMessage> DatapointPutAsync(M.Datapoint datapoint)
		{
			var vLoc = string.Format("{0}::{1}(M.Datapoint datapoint={2})", TYPE_NAME, nameof(DatapointPutAsync), datapoint);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(datapoint);
			Debug.WriteLine(vLoc + ", JSON: " + json);

			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PutAsync(
				DatapointApiUrl + "/" + datapoint.Id, content);
			return response;
		}

		/// <summary>
		/// Delete existing datapoint
		/// </summary>
		/// <param name="datapoint"></param>
		/// <returns></returns>
		public async Task<HttpResponseMessage> DatapointDeleteAsync(M.Datapoint datapoint)
		{
			var vLoc = string.Format("{0}::{1}(M.Datapoint datapoint={2})", TYPE_NAME, nameof(DatapointDeleteAsync), datapoint);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var response = await client.DeleteAsync(
				DatapointApiUrl + "/" + datapoint.Id);
			return response;
		}


		/// <summary>
		/// This method returns Datapoints with their DatapointValues for chart, according to chartParams.
		/// I left it to exist, but it is unused today. Main method which today used is below or ChartDatapointValues.
		/// </summary>
		/// <param name="chartParams"></param>
		/// <returns></returns>
		public async Task<List<M.Datapoint>> ChartDatapoints(ChartSearchParams chartParams)
		{
			var vLoc = string.Format("{0}::{1}[Secondary]({2} {3})",
				TYPE_NAME, nameof(ChartDatapointValues),
				nameof(ChartSearchParams), nameof(chartParams));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var pJson = JsonConvert.SerializeObject(chartParams);
			Debug.WriteLine(string.Format("{0}, JSON: [{1}]", vLoc, pJson));

			HttpContent content = new StringContent(pJson);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(DatapointChartUrl, content);

			// Receiving the response
			string rJson = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<List<M.Datapoint>>(rJson);
		}

		/// <summary>
		/// Datapoint values lists
		/// 
		/// @deprecated, was used for main chart, but replaced with ChartDatapoints(VisualChartParameters chartParams)
		/// </summary>
		/// <param name="deviceIds"></param>
		/// <returns></returns>
		public async Task<List<M.DatapointValue>> ChartDatapointValues(ChartSearchParams chartParams)
		{
			var vLoc = string.Format("{0}::{1}[Main]({2} {3})", 
				TYPE_NAME, nameof(ChartDatapointValues), 
				nameof(ChartSearchParams), nameof(chartParams));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var pJson = JsonConvert.SerializeObject(chartParams);
			Debug.WriteLine(string.Format("{0}, {1}: [{2}]", vLoc, nameof(pJson), pJson));

			HttpContent content = new StringContent(pJson);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(DatapointValueSearchApiUrl, content);

			// Receiving the response
			string rJson = await response.Content.ReadAsStringAsync();
			Debug.WriteLine(string.Format("{0}, {1}: [{2}]", vLoc, nameof(rJson), rJson));

			// Possible exception throw
			return JsonConvert.DeserializeObject<List<M.DatapointValue>>(rJson);
		}

		public async Task<string> ChartDatapointValuesDownload(VisualChartSearchParams chartParams)
		{
			var vLoc = string.Format("{0}::{1}(VisualChartParameters chartParams)", TYPE_NAME, nameof(ChartDatapointValuesDownload));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var pJson = JsonConvert.SerializeObject(chartParams);
			Debug.WriteLine(string.Format("{0}, JSON: [{1}]", vLoc, pJson));

			HttpContent content = new StringContent(pJson);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(DatapointValueDownloadApiUrl, content);

			// Receiving the response
			string rJson = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<string>(rJson);
		}

		/// <summary>
		/// @deprecated - used only in dashboard (remove it)
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		public async Task<List<DatapointValue>> DatapointValues (DatapointValueFilter filter)
		{
			var vLoc = string.Format("{0}::{1}(DatapointValueFilter filter)", TYPE_NAME, nameof(DatapointValues));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var pJson = JsonConvert.SerializeObject(filter);
			Debug.WriteLine(string.Format("{0}, JSON: [{1}]", vLoc, pJson));

			HttpContent content = new StringContent(pJson);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(DatapointValueValuesApiUrl, content);

			// Receiving the response
			string rJson = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<List<M.DatapointValue>>(rJson);
		}
		#endregion

		#region DeviceTopic
		public async Task<List<DeviceTopic>> DeviceTopicListAsync(int deviceId)
		{
			var vLoc = string.Format($"{TYPE_NAME}::{nameof(DeviceTopicListAsync)}(string {nameof(deviceId)}={deviceId}");
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var url = string.Format($"{DeviceTopicApiUrl}/?deviceId={deviceId}");
			Debug.WriteLine(vLoc + ", " + url);

			var rJson = await client.GetStringAsync(url);
			var deviceTopics = JsonConvert.DeserializeObject<List<DeviceTopic>>(rJson);
			return deviceTopics;
		}

		/// <summary>
		/// Save new device topic
		/// </summary>
		/// <param name="deviceTopic"></param>
		/// <returns></returns>
		public async Task<HttpResponseMessage> DeviceTopicPostAsync(M.DeviceTopic deviceTopic)
		{
			var vLoc = $"{TYPE_NAME}::{nameof(DeviceTopicPostAsync)}";
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(deviceTopic);
			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(DeviceTopicApiUrl, content);
			return response;
		}

		/// <summary>
		/// Update existing device topic
		/// </summary>
		/// <param name="deviceTopic"></param>
		/// <returns></returns>
		public async Task<HttpResponseMessage> DeviceTopicPutAsync(M.DeviceTopic deviceTopic)
		{
			var vLoc = $"{TYPE_NAME}::{nameof(DeviceTopicPutAsync)}";
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(deviceTopic);

			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PutAsync(
				DeviceTopicApiUrl + "/" + deviceTopic.Id, content);
			return response;
		}

		public async Task<HttpResponseMessage> DeviceTopicDeleteAsync(M.DeviceTopic deviceTopic)
		{
			var vLoc = $"{TYPE_NAME}::{nameof(DeviceTopicDeleteAsync)}";
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var response = await client.DeleteAsync(
				DeviceTopicApiUrl + "/" + deviceTopic.Id);
			return response;
		}

		#endregion

		#region Users
		/// <summary>
		/// User's registering RESTful API method
		/// </summary>
		/// <param name="email"></param>
		/// <param name="password"></param>
		/// <param name="confirmPassword"></param>
		/// <returns></returns>
		internal async Task<HttpResponseMessage> RegisterAsync(RegisterBindingModel rbm)
		{
			var vLoc = string.Format("{0}::{1}({2} rbm)", TYPE_NAME, nameof(RegisterAsync), nameof(RegisterBindingModel));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient(false);

#if USE_JSON_NEWTON
			var json = JsonConvert.SerializeObject(rbm);
#else
			var json = JsonSerializer.Serialize(model);
#endif
			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			return await client.PostAsync(RegisterApiUrl, content);

			//var response = await client.PostAsync(ServerApiUrl_REGISTER, content);
			//return response.IsSuccessStatusCode;
		}

		/// <summary>
		/// User's Login API method
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public async Task<string> LoginAsync(string username, string password)
		{
			var vLoc = string.Format("{0}::{1}(string username, string password)", TYPE_NAME, nameof(LoginAsync));
			Debug.WriteLine(vLoc);

			// Preparing request
			var keyValues = new List<KeyValuePair<string, string>>()
			{
				new KeyValuePair<string, string>("username", username),
				new KeyValuePair<string, string>("password", password),
				new KeyValuePair<string, string>("grant_type", "password"),
			};

			// Http header or message
			var request = new HttpRequestMessage(
				HttpMethod.Post, TokenApiUrl);
			request.Content = new FormUrlEncodedContent(keyValues);

			// Initializing client
			var client = GetHttpClient(false);

			// Sending the request
			var response = await client.SendAsync(request);

			// Receiving the response
			string content = await response.Content.ReadAsStringAsync();
			return content;
		}

		public async Task<HttpResponseMessage> PingAsync()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(PingAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var url = string.Format("{0}/Ping", AccountApiUrl);
			var response = await client.GetAsync(url);
			return response;
		}

		internal async Task<HttpResponseMessage> ForgotPasswordAsync(string email)
		{
			var client = GetHttpClient(false);
			var url = string.Format("{0}/ForgotPassword?email={1}", AccountApiUrl, email);
			var response = await client.GetAsync(url);
			return response;
		}

		/// <summary>
		/// User's logout
		/// </summary>
		/// <returns></returns>
		public async Task<HttpResponseMessage> UserLogoutAsync()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(UserLogoutAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var url = string.Format("{0}/{1}", AccountApiUrl, "Logout");
			return await client.GetAsync(url);
		}

		/// <summary>
		/// Searching for any user which public name contains name text
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public async Task<List<M.User>> UserSearchAsync(UserRelationType type, string name)
		{
			var vLoc = string.Format("{0}::{1}(UserRelationType type={2}, string name={3})", TYPE_NAME, nameof(UserSearchAsync), type, name);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var url = string.Format("{0}/Search?type={1}&name={2}", UserApiUrl, type, name);
			var json = await client.GetStringAsync(url);
			var result = JsonConvert.DeserializeObject<List<M.User>>(json);
			return result;
		}

		public async Task<VisualUser> UserInfoAsync(UserInfoType type, string id)
		{
			var vLoc = string.Format("{0}::{1}(UserInfoType type={2}, string id={3})",
				TYPE_NAME, nameof(UserInfoAsync), type, id);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var url = $"{UserApiUrl}/Info?{(type == UserInfoType.Post ? "postId" : "userId")}={id}";
			var json = await client.GetStringAsync(url);
			Debug.WriteLine(string.Format("{0}: {1}", vLoc, json));

			var result = JsonConvert.DeserializeObject<VisualUser>(json);
			return result;
		}

		/// <summary>
		/// Updating user's info
		/// 
		/// Currently updating only language
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public async Task<HttpResponseMessage> UserPutAsync(User record)
		{
			var vLoc = string.Format("{0}::{1}(IUser user={2})", TYPE_NAME, nameof(UserPutAsync), record);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(record);

			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PutAsync(
				UserApiUrl + "/" + record.Id, content); 
			return response;
		}

		#endregion

		#region Billing
		public async Task<List<VisualLicenseProduct>> LicenseProductsAsync()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(LicenseProductsAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = await client.GetStringAsync(string.Format("{0}/Products",
				BillingApiUrl, Settings.Language));
			var records = JsonConvert.DeserializeObject<List<VisualLicenseProduct>>(json);
			return records;
		}

		public async Task<List<PaymentMethod>> PaymentMethodsAsync()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(PaymentMethodsAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = await client.GetStringAsync(string.Format("{0}/PaymentMethods",
				BillingApiUrl, Settings.Language));
			var records = JsonConvert.DeserializeObject<List<PaymentMethod>>(json);
			return records;
		}

		public async Task<HttpResponseMessage> OrderPostAsync(IOrder order)
		{
			var vLoc = string.Format("{0}::{1}(IOrder order)", TYPE_NAME, nameof(OrderPostAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(order);
			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(BillingApiUrl + "/PostOrder", content);
			return response;
		}


		#endregion

		#region Ecosystem
		/// <summary>
		/// Returns all user's friends
		/// </summary>
		/// <returns></returns>
		public async Task<List<VisualPost>> PostListAsync(PostFeedType feedType, DateTime? sinceDate)
		{
			var vLoc = String.Format(
				"{0}::{1}(DateTime? {0})",
				TYPE_NAME,
				nameof(PostListAsync),
				(sinceDate.HasValue ? sinceDate.Value.ToString(Defaults.DEFAULT_DATETIME_FORMAT) : "NULL")
			);
			Debug.WriteLine(vLoc);

			string dateStr = (sinceDate.HasValue ? sinceDate.Value.ToString(Defaults.DEFAULT_DATETIME_FORMAT) : "null");

			var client = GetHttpClient();
			var url = string.Format("{0}?feed={1}&sinceDate={2}",
				PostApiUrl, (int)feedType, dateStr);
			Debug.WriteLine(vLoc, url);

			var json = await client.GetStringAsync(url);
			var records = JsonConvert.DeserializeObject<List<VisualPost>>(json);

			foreach (var record in records)
			{
				if (record.ImageId.HasValue)
				{
					var imageUrl = string.Format("{0}?id={1}&type={2}",
						PostImageApiUrlRaw,
						record.ImageId.ToString(), 2);

					Debug.WriteLine(vLoc, "ImageUrl=" + imageUrl);

					record.ImageUrl = Utils.CreateImageUrl(record.ImageId.Value, ImageType.Thumb);
				}
			}

			return records;
		}

		public async Task<HttpResponseMessage> PostNewAsync(M.PostNew post)
		{
			var vLoc = string.Format("{0}::{1}(M.PostNew post)", TYPE_NAME, nameof(PostNewAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(post);
			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(PostApiUrl, content);
			return response;
		}

		public async Task<HttpResponseMessage> PostReactionAsync(IPostReaction postReaction)
		{
			var vLoc = string.Format("{0}::{1}(IPostReaction postReaction)", TYPE_NAME, nameof(PostReactionAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(postReaction);
			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(PostReactionApiUrl, content);
			return response;
		}

		#endregion

		#region Friends or Family

		/// <summary>
		/// Returns all user's friends
		/// </summary>
		/// <returns></returns>
		public async Task<List<M.Friend>> FriendListAsync()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(FriendListAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = await client.GetStringAsync(FriendApiUrl);
			Debug.WriteLine("{0}, JSON: {1}", vLoc, json);

			var records = JsonConvert.DeserializeObject<List<M.Friend>>(json);
			return records;
		}

		public async Task<HttpResponseMessage> FriendPostAsync(IRelatedPerson record)
		{
			var vLoc = string.Format("{0}::{1}(IRelatedPerson record)", TYPE_NAME, nameof(FriendPostAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(record);
			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(FriendApiUrl, content);
			return response;
		}

		public async Task<HttpResponseMessage> FriendPutAsync(IRelatedPerson record)
		{
			var vLoc = string.Format("{0}::{1}(IRelatedPerson record)", TYPE_NAME, nameof(FriendPutAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(record);

			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PutAsync(
				FriendApiUrl + "/" + record.Id, content);
			return response;
		}

		/// <summary>
		/// Delete existing friend
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public async Task<HttpResponseMessage> FriendDeleteAsync(IRelatedPerson record)
		{
			var vLoc = string.Format("{0}::{1}(IRelatedPerson record)", TYPE_NAME, nameof(FriendDeleteAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var response = await client.DeleteAsync(
				FriendApiUrl + "/" + record.Id);
			return response;
		}

		#endregion

		#region Blocked

		/// <summary>
		/// Returns all user's Blocked
		/// </summary>
		/// <returns></returns>
		public async Task<List<M.Blocked>> BlockedListAsync()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(BlockedListAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = await client.GetStringAsync(BlockedApiUrl);
			Debug.WriteLine("{0}, JSON: {1}", vLoc, json);

			var records = JsonConvert.DeserializeObject<List<M.Blocked>>(json);
			return records;
		}

		public async Task<HttpResponseMessage> BlockedPostAsync(IRelatedPerson record)
		{
			var vLoc = string.Format("{0}::{1}(IRelatedPerson blocked)", TYPE_NAME, nameof(BlockedPostAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(record);
			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(BlockedApiUrl, content);
			return response;
		}

		public async Task<HttpResponseMessage> BlockedPutAsync(IRelatedPerson record)
		{
			var vLoc = string.Format("{0}::{1}(IRelatedPerson record)", TYPE_NAME, nameof(BlockedPutAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(record);

			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PutAsync(
				BlockedApiUrl + "/" + record.Id, content);
			return response;
		}

		/// <summary>
		/// Delete existing blocked (unblock user)
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public async Task<HttpResponseMessage> BlockedDeleteAsync(IRelatedPerson record)
		{
			var vLoc = string.Format("{0}::{1}(IRelatedPerson record)", TYPE_NAME, nameof(BlockedDeleteAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var response = await client.DeleteAsync(
				BlockedApiUrl + "/" + record.Id);
			return response;
		}

		#endregion

		#region Messages
		/// <summary>
		/// Load all user's chat conversations with all users
		/// </summary>
		/// <returns></returns>
		public async Task<ObservableCollection<VisualChatConversation>> ChatConversationsAsync()
		{
			var vLoc = string.Format("{0}::{1}()",
				TYPE_NAME, nameof(ChatConversationsAsync));
			Debug.WriteLine(string.Format("{0}, Start...", vLoc));

			var client = GetHttpClient();
			var url = string.Format("{0}", MessageApiUrl);
			Debug.WriteLine(string.Format("{0}, {1}", vLoc, url));

			var json = await client.GetStringAsync(url);
			Debug.WriteLine(string.Format("{0}, {1}", vLoc, json));
			var records = JsonConvert.DeserializeObject<ObservableCollection<VisualChatConversation>>(json);
			return records;
		}

		/// <summary>
		/// Load specific conversation, with specific user (via userId) or owner of post (or user via postId)
		/// </summary>
		/// <returns></returns>
		public async Task<List<VisualChatMessage>> ChatConversationAsync(
			UserInfoType type,
			string id,
			ListLoadMode loadMode,
			DateTime? firstDate,
			DateTime? lastDate)
		{
			var vLoc = string.Format("{0}::{1}(UserInfoType type={2}, string id={3}, ListLoadMode loadMode={4}, DateTime? firstDate={5}, DateTime? lastDate={6})",
				TYPE_NAME, nameof(ChatConversationAsync), type, id, loadMode, firstDate, lastDate);
			Debug.WriteLine(vLoc, "Start...");

			string firstDateStr = (firstDate.HasValue ? firstDate.Value.ToString(Defaults.DEFAULT_DATETIME_FORMAT) : "null");
			string lastDateStr = (lastDate.HasValue ? lastDate.Value.ToString(Defaults.DEFAULT_DATETIME_FORMAT) : "null");

			var client = GetHttpClient();
			var url = string.Format("{0}?{1}={2}&loadMode={3}&firstDate={4}&lastDate={5}",
				MessageApiUrl,
				(type == UserInfoType.User ? "receiverUserId" : "postId"),
				id, loadMode, firstDateStr, lastDateStr);
			Debug.WriteLine(vLoc, url);

			var json = await client.GetStringAsync(url);
			var records = JsonConvert.DeserializeObject<List<VisualChatMessage>>(json);
			return records;
		}

		/// <summary>
		/// Posting of new message
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public async Task<HttpResponseMessage> MessagePostAsync(IMessage item)
		{
			var vLoc = string.Format("{0}::{1}(IMessage item={2})", TYPE_NAME, nameof(MessagePostAsync), item);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(item);
			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(MessageApiUrl, content);
			return response;
		}

		public async Task<bool> MessageReadAsync(int messageId)
		{
			var vLoc = string.Format("{0}::{1}(int messageId={2})", TYPE_NAME, nameof(MessageReadAsync), messageId);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var url = string.Format("{0}/Read?messageId={1}", MessageApiUrl, messageId);
			var json = await client.GetStringAsync(url);
			var result = JsonConvert.DeserializeObject<bool>(json);
			return result;
		}

		#endregion

		#region Situation
		public async Task<Situation> SituationAsync()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(SituationAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var url = string.Format("{0}", SituationApiUrl);
			var json = await client.GetStringAsync(url);
			var result = JsonConvert.DeserializeObject<Situation>(json);
			return result;
		}

		#endregion

		#region Token transaction
		public async Task<HttpResponseMessage> TokenTransactionPostAsync(ITokenTransaction tokenTransaction)
		{
			var vLoc = string.Format("{0}::{1}(ITokenTransaction tokenTransaction)", TYPE_NAME, nameof(TokenTransactionPostAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(tokenTransaction);
			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(TokenTransactionApiUrl, content);
			return response;
		}


		#endregion

		#region Payment systems integration
		public async Task PayseraPostOrder(IOrder order)
		{
			var vLoc = string.Format("{0}::{1}(IOrder order)", TYPE_NAME, nameof(PayseraPostOrder));
			Debug.WriteLine(vLoc);

			// Deciphering required for Paysera params
			var payseraProjectId = Defaults.PAYSERA_PROJECT_ID;
			var payseraSignPassword = Defaults.PAYSERA_SIGN_PASSWORD;

			// Initializing Paysera Client
			// @see https://github.com/evp/webtopay-lib-dotnet/blob/master/example/EVP.WebToPay.ClientAPIExample/Default.aspx.cs
			var client = new Client(payseraProjectId, payseraSignPassword);
			// Make a new request
			MacroRequest request = client.NewMacroRequest();

			// Order number
			// Should be saved somewhere and unique for every request.
			request.OrderId = order.OrderNo;

			// Money ammount in CENTS!
			var priceCents = Convert.ToInt32(Math.Abs(order.FinalPrice * 100));
			var testRevertedPrice = Convert.ToDecimal(priceCents / 100);
			Debug.Assert(
				Decimal.Equals(testRevertedPrice, order.FinalPrice),
				string.Format("{0}, {1}::{2} The price {3} has a remainder of more than two decimal places!",
				vLoc, nameof(IOrder), nameof(IOrder.FinalPrice), order.FinalPrice));
			request.Amount = priceCents;

			request.Currency = "EUR";
			//request.Country = "LT"; // Country of buyer (we don't know; itsn't mandatory)

			// Urls
			request.AcceptUrl = PaymentAcceptUrl;
			request.CancelUrl = PaymentCancelUrl;
			request.CallbackUrl = BillingApiConfirmOrderUrl;

			// Change this to "true" if you want to test
#if PAYSERA_TEST
			request.Test = true;
#else
			request.Test = false;
#endif

			Debug.WriteLine(string.Format("{0}, request=[{1}]", vLoc, JsonConvert.SerializeObject(request)));

			string redirectUrl = client.BuildRequestUrl(request);
			Debug.WriteLine(string.Format("{0}, {1}", vLoc, redirectUrl));
			//Response.Redirect(redirectUrl);

			// @see https://learn.microsoft.com/en-us/xamarin/essentials/open-browser?tabs=android
			try
			{
				await Browser.OpenAsync(redirectUrl, BrowserLaunchMode.SystemPreferred);
			}
			catch (Exception ex)
			{
				// An unexpected error occured. No browser may be installed on the device.
				Debug.WriteLine(string.Format("{0}, {1}", vLoc, ex.Message));
			}

		}

		#endregion

		#region Datapoint Formula
		public async Task<List<VisualDatapointFormula>> DatapointFormulaListAsync(string lang)
		{
			var vLoc = string.Format("{0}::{1}({2})", TYPE_NAME, nameof(DatapointFormulaListAsync), lang);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var url = string.Format("{0}?lang={1}", DatapointFormulaApiUrl, lang);
			var json = await client.GetStringAsync(url);
			var records = JsonConvert.DeserializeObject<List<VisualDatapointFormula>>(json);
			return records;
		}

		public async Task<List<M.DatapointFormulaPresetChain>> DatapointFormulaPresetChainsAsync(int formulaId)
		{
			var vLoc = string.Format("{0}::{1}({2})", TYPE_NAME, nameof(DatapointFormulaPresetChainsAsync), formulaId);
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var url = string.Format("{0}{1}/PresetChains", DatapointFormulaApiUrl, formulaId);
			var json = await client.GetStringAsync(url);
			var records = JsonConvert.DeserializeObject<List<M.DatapointFormulaPresetChain>>(json);
			return records;
		}

		#endregion

		#region Dashboard
		/// <summary>
		/// Returns grouped datapoints with minimum info (id, name, device name as group)
		/// Used by Dashboard charts options.
		/// </summary>
		/// <param name="objectId"></param>
		/// <returns></returns>
		public async Task<List<GroupedIntIdItem>> GetGroupedDatapointsAsync(int objectId)
		{
			var url = string.Format("{0}/GroupedDatapoints?objectId={1}", DatapointApiUrl, objectId);
			var vLoc = string.Format("{0}::{1}(string objectId={2}), url={3}", TYPE_NAME, nameof(GetGroupedDatapointsAsync), objectId, url);
			Debug.WriteLineIf(DEBUG, vLoc);

			var client = GetHttpClient();
			var rJson = await client.GetStringAsync(url);

			Debug.WriteLine(string.Format("{0}, result: {1}", vLoc, rJson));

			var result = JsonConvert.DeserializeObject<List<GroupedIntIdItem>>(rJson);
			return result;
		}

		public async Task<DashboardSetting> DashboardSettingLoadAsync(int objectId)
		{
			var client = GetHttpClient();

			var url = string.Format("{0}/DashboardSetting?objectId={1}", DashboardApiUrl, objectId);
			var vLoc = string.Format("{0}::{1}(objectId={2}), url={3}", TYPE_NAME, nameof(DashboardSettingLoadAsync), objectId, url);
			var json = await client.GetStringAsync(url);

			Debug.WriteLine(string.Format("{0}, result: {1}", vLoc, json));

			var result = JsonConvert.DeserializeObject<DashboardSetting>(json);
			return result;
		}

		/// <summary>
		/// Posting of DashboardSetting to the server
		/// </summary>
		/// <param name="ds"></param>
		/// <returns></returns>
		public async Task<HttpResponseMessage> PostDashboardSettingAsync(DashboardSetting ds)
		{
			var vLoc = string.Format("{0}::{1}(DashboardSetting ds)", TYPE_NAME, nameof(PostDashboardSettingAsync));
			Debug.WriteLine(vLoc);

			var client = GetHttpClient();
			var json = JsonConvert.SerializeObject(ds);
			Debug.WriteLine(string.Format("{0}, {1}", vLoc, json));

			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(DashboardApiUrl, content);
			return response;
		}

		#endregion

	}
}

