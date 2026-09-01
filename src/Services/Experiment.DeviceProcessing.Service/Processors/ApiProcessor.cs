using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Experiment.Core.BL.Data;
using Experiment.Core.Metadata;
using Experiment.Data.Models;

using Experiment.DeviceProcessing.Service.Data;
using Experiment.DeviceProcessing.Service.Models;
using Experiment.DeviceProcessing.Service.Services;

namespace Experiment.DeviceProcessing.Service.Processors{
	internal class ApiProcessor : IDeviceProcessor
	{
		private const string TYPE_NAME = nameof(ApiProcessor);

		private readonly ThreadStateObject _state;
		private readonly ILogger _logger;
		private readonly ExpSql _db;
		private readonly ApiService _apiService;

		internal ApiProcessor(ThreadStateObject state, ILogger logger)
		{
			_state = state;
			_logger = logger;
			_apiService = new ApiService(logger);

			var vLoc = $"{_state.DebugThreadId}/{TYPE_NAME}::{nameof(ApiProcessor)}";
			_logger.WriteLine(5, vLoc);

			_db = ExpSql.GenerateFromDefaults(logger);
		}

		public async Task StartAsync()
		{
			var vLoc = $"{_state.DebugThreadId}/{TYPE_NAME}::{nameof(StartAsync)}";
			var stage = "Start";

			try
			{
				switch (_state.Device.UnitId)
				{
					case 0:
						stage = $"{nameof(_apiService)}::{nameof(ApiService.GetAllInfo)}";
						await HandleUnitZeroAsync(vLoc, stage).ConfigureAwait(false);
						break;

					case 1:
						stage = $"{nameof(GetDatapointApiData)}";
						await HandleUnitOneAsync(vLoc).ConfigureAwait(false);
						break;

					default:
						_logger.WriteLine(4, $"{vLoc} Unsupported UnitId: {_state.Device.UnitId}");
						break;
				}
			}
			catch (Exception ex)
			{
				_logger.WriteLine(0, $"{vLoc}/{stage}/{ex.Message}");
			}
		}

		private async Task HandleUnitZeroAsync(string vLoc, string stage)
		{
			var json = await _apiService.GetAllInfo($"{_state.DebugThreadId}", _state.Device).ConfigureAwait(false);

			if (!string.IsNullOrEmpty(json))
			{
				stage = nameof(_db.ScanDataJsonUpdate);
				_logger.WriteLine(5, $"{vLoc}/{stage}");
				_db.ScanDataJsonUpdate(_state.Device.Id, json);

				stage = "DONE!";
				_logger.WriteLine(5, $"{vLoc}/{stage}");
			}
			else
			{
				stage = "NO DATA";
				_logger.WriteLine(4, $"{vLoc}/{stage}");
			}
		}

		private async Task HandleUnitOneAsync(string vLoc)
		{
			_logger.WriteLine(5, $"{vLoc} Unit Id is 1, AI Support enabling dp!");

			var datapoints = GetDatapointApiData();
			if (datapoints == null)
			{
				return;
			}

			var decisionParams = AiDecisionOnSpotParams.From(datapoints);

			_logger.WriteLine(5, $"{vLoc}/{nameof(_apiService.AiDecisionOnSpot)}({nameof(decisionParams)})");
			AiDecisionOnSpotResult result = await _apiService.AiDecisionOnSpot(
				$"{_state.DebugThreadId}", decisionParams).ConfigureAwait(false);

			if (result != null)
			{
				SaveDecisionResult(vLoc, datapoints, result);
			}
		}

		private void SaveDecisionResult(string vLoc, IEnumerable<AiDatapointInfo> datapoints, AiDecisionOnSpotResult result)
		{
			var stage = "Filtering result datapoints";
			_logger.WriteLine(5, $"{vLoc}/{stage}");

			var datapointsForSave = datapoints.Where(dp =>
				dp.Direction == Experiment.Data.Enums.ParameterDirection.In ||
				dp.Direction == Experiment.Data.Enums.ParameterDirection.Both);

			var saved = 0;
			foreach (var dp in datapointsForSave)
			{
				stage = "Processing every result datapoint";
				_logger.WriteLine(5, $"{vLoc}/{stage}");

				switch (dp.Alias)
				{
					default:

						_logger.WriteLine(5, $"{vLoc} found strange datapoint for save, {dp.DatapointId}!{dp.Alias}, SKIPPING!");
						break;

					case AiDecisionOnSpotResult.JSONP_DECISION:

						stage = $"Saving DP: {dp.DatapointId}:{dp.Alias} value: {(int)result.Decision}";
						_logger.WriteLine(5, $"{vLoc}/{stage}");
						_state.Db.ScanValueWrite(
							_state.Device.Id, dp.DatapointId, (int)result.Decision); // call

						saved++;
						break;
				}

				if (saved > 0)
				{
					break;
				}
			}

			if (saved > 0)
			{
				stage = $"Updating scan session";
				_logger.WriteLine(5, $"{vLoc}/{stage}");
				_state.Db.DeviceLastScanUpdate(_state.Device.Id, DateTime.Now); // call
			}
		}

		private List<AiDatapointInfo> GetDatapointApiData()
		{
			var vLoc = $"{_state.DebugThreadId}/{TYPE_NAME}::{nameof(GetDatapointApiData)}";

			var stage = $"[prcDatapointApiData] {_state.Device.Id}";
			_logger.WriteLine(5, $"{vLoc}/{stage}");
			DataTable table = _state.Db.Query(stage);

			var datapoints = table.AsEnumerable()
			.Select(row => new AiDatapointInfo()
			{
				DatapointId = row.Field<int>(nameof(AiDatapointInfo.DatapointId)),
				Alias = row.Field<string>(nameof(AiDatapointInfo.Alias)),
				Multiplier = row.Field<decimal>(nameof(AiDatapointInfo.Multiplier)),
				Direction = row.Field<Experiment.Data.Enums.ParameterDirection>(nameof(AiDatapointInfo.Direction)),
				ValueType = row.Field<Experiment.Data.Enums.DatapointSettingValueType>(nameof(AiDatapointInfo.ValueType)),
				Mandatory = row.Field<bool>(nameof(AiDatapointInfo.Mandatory)),
				Value = row.Field<decimal?>(nameof(AiDatapointInfo.Value)),
			}).ToList();

			return datapoints;
		}
	}
}
