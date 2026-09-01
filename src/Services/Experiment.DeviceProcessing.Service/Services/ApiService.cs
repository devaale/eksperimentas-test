using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Experiment.Core.Metadata;
using Experiment.Data.Models;

using Experiment.DeviceProcessing.Service.Models;

namespace Experiment.DeviceProcessing.Service.Services{
	internal class ApiService
	{
		private const string TYPE_NAME = nameof(ApiService);
		private const string BASE_URL = "http://localhost:10000/";
		private const string URI_GET_ALL_INFO = "get_all_info";
		private const string URI_AI_CONTROL = "ai_control";

		private readonly ILogger _logger;
		private readonly HttpClient _client;

		public ApiService(ILogger logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_client = BuildClient();
		}

		public async Task<string> GetAllInfo(string dInfo, Device device)
		{
			var vLoc = $"{dInfo}/{TYPE_NAME}::{nameof(GetAllInfo)}";
			var url = CreateUri(URI_GET_ALL_INFO);
			_logger.WriteLine(5, $"{vLoc} url: {url}");
			_logger.WriteLine(5, $"{vLoc} DeviceId: {device?.Id}");

			var response = await _client.GetAsync(url).ConfigureAwait(false);
			var rJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			_logger.WriteLine(5, $"{vLoc} rJson: {rJson}");
			return rJson;
		}

		public async Task<AiDecisionOnSpotResult> AiDecisionOnSpot(string dInfo, AiDecisionOnSpotParams p)
		{
			var vLoc = $"{dInfo}/{TYPE_NAME}::{nameof(AiDecisionOnSpot)}";
			var url = CreateUri(URI_AI_CONTROL);
            var payload = SerializeAiDecisionPayload(p);
			_logger.WriteLine(5, $"{vLoc} url: {url}");
			_logger.WriteLine(5, $"{vLoc} payload: {payload}");

			using (var content = new StringContent(payload))
			{
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				var response = await _client.PostAsync(url, content).ConfigureAwait(false);
				var rJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				_logger.WriteLine(5, $"{vLoc} rJson: {rJson}");
				return JsonConvert.DeserializeObject<AiDecisionOnSpotResult>(rJson);
			}
		}

		private static string SerializeAiDecisionPayload(AiDecisionOnSpotParams p)
		{
			var payload = new
			{
				temperatures = new decimal[]
				{
					p.CurrentTemp,
					p.AirTempMin,
					p.AirTempMax,
					p.TargetTemp,
					p.CurrentAirTemp,
					p.TargetTempFirst,
					p.TargetTempSecond,
					p.AbsCurrentTempMinusTargetTemp,
				},
				hours = new decimal[]
				{
					p.CurrentHour,
					p.TargetHourStart,
					p.TargetHourEnd,
				},
				energy = new decimal[]
				{
					p.CurrentEnergy,
					p.EnergyMaxCapacity,
				},
				energy_production_consumption = new decimal[]
				{
					p.EnergyProductionRate,
					p.EnergyProductionMax,
					p.HeaterEnergyConsumptionRate,
					p.CoolerEnergyConsumptionRate,
				},
				temp_influence = new decimal[]
				{
					p.HeatingTempInfluence,
					p.CoolingTempInfluence,
				},
				flags = new bool[]
				{
					p.IsSunny,
					p.HeatingOn,
					p.CoolingOn,
				},
				random_chances = new decimal[]
				{
					p.SunnyDayChance,
					p.TemperatureIncreaseWhenSunnyChance,
					p.TemperatureIncreaseWhenCloudyChance,
				},
			};

			return JsonConvert.SerializeObject(payload);
		}

		private HttpClient BuildClient()
		{
			var client = new HttpClient
			{
				BaseAddress = new Uri(BASE_URL)
			};

			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			return client;
		}

		private static string CreateUri(string relativePath)
		{
			return relativePath;
		}
	}
}
