using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Experiment.Core.Metadata;
using Experiment.Data.Models;

using Experiment.DeviceScanner.Data;

namespace Experiment.DeviceScanner.Services{
	internal class ApiService
	{
		#region Const
		const string TYPE_NAME = nameof(ApiService);
		const bool DEBUG = true;

		const string PRE_URI = "http://";
		const string URI_GET_ALL_INFO = "get_all_info";

		#endregion

		#region Attributes

		ILogger _Logger;

		#endregion

		#region Ctor

		public ApiService(ILogger logger)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(ApiService));

			if (logger == null)
				throw new ArgumentNullException(string.Format("{0}, {1} parameter shouldn't be empty ", vLoc, nameof(logger)));

			_Logger = logger;
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Returns initialized and suficient for authorization HttpClient
		/// 
		/// WARNING! All auth routines initialize here.
		/// </summary>
		/// <returns></returns>
		internal HttpClient GetHttpClient()
		{
			return new HttpClient();
		}


		#endregion

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="device"></param>
		/// <returns>Data in JSON string format</returns>
		public async Task<string> GetAllInfo(ScanDeviceInfo device)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(GetAllInfo));
			_Logger.WriteLine(5, string.Format("{0}, Start..", vLoc));

			var client = GetHttpClient();
			var url = string.Format("{0}{1}/{2}", PRE_URI, device.Url, URI_GET_ALL_INFO);
			_Logger.WriteLine(5, string.Format("{0}, {1}: {2}", vLoc, nameof(url), url));

			var rJson = await client.GetStringAsync(url);
			_Logger.WriteLine(5, string.Format("{0}, {1}: {2}", vLoc, nameof(rJson), rJson));

			return rJson;

			// Deserialize to generics dictionary
			//var retVal = JsonConvert.DeserializeObject<ApiInfo>(rJson);
			//return retVal;
		}

		#endregion
	}
}
