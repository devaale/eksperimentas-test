using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Experiment.Core;
using Experiment.Core.BL.Data;
using Experiment.Core.BL.Data.SysVars;
using Experiment.Core.IO;
using Experiment.Core.Metadata;
using Experiment.Data.Enums;
using Experiment.Data.Models;

using Experiment.DeviceProcessing.Service.Data;
using Experiment.DeviceProcessing.Service.Processors;
using ThreadState = Experiment.DeviceProcessing.Service.Data.ThreadState;

namespace Experiment.DeviceProcessing.Service.Runtime{
	internal sealed class DeviceProcessingEngine : IDisposable
	{
		private const string TYPE_NAME = nameof(DeviceProcessingEngine);
		private const string THREAD_ID_PATTERN = "Thread:{0}, DeviceId:{1}";
		private const int DEFAULT_LOOP_DELAY_SECONDS = 15;
		private const int FAILURE_DELAY_MINUTES = -10;
		private const int LOOP_STEP_SECONDS = 3;
		private const int SECOND_DELAY_MS = 1000;

		private readonly ILogger _logger;
		private readonly ExpSql _db;
		private readonly Func<ExpSql> _dbFactory;
		private readonly DeviceProcessorFactory _processorFactory;
		private readonly ConcurrentDictionary<int, DateTime> _processingDevices = new ConcurrentDictionary<int, DateTime>();

		private CancellationTokenSource _cts;
		private Task _heartbeatTask;
		private IDictionary<SysVarName, object> _vars = new Dictionary<SysVarName, object>();
		private int _loopDelaySeconds = DEFAULT_LOOP_DELAY_SECONDS;
		private int _threadCounter;

		internal DeviceProcessingEngine(ILogger logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_db = ExpSql.GenerateFromDefaults(_logger);
			_dbFactory = () => ExpSql.GenerateFromDefaults(_logger);
			_processorFactory = new DeviceProcessorFactory(_logger);
		}

		internal void Start()
		{
			if (_heartbeatTask != null && !_heartbeatTask.IsCompleted)
			{
				return;
			}

			_cts = new CancellationTokenSource();
			_heartbeatTask = Task.Run(() => HeartbeatAsync(_cts.Token), _cts.Token);
		}

		internal async Task StopAsync()
		{
			if (_cts == null)
			{
				return;
			}

			_cts.Cancel();

			if (_heartbeatTask != null)
			{
				try
				{
					await _heartbeatTask.ConfigureAwait(false);
				}
				catch (OperationCanceledException)
				{
					// normal shutdown path
				}
			}
		}

		private async Task HeartbeatAsync(CancellationToken token)
		{
			var vLoc = $"{TYPE_NAME}::{nameof(HeartbeatAsync)}";

			while (!token.IsCancellationRequested)
			{
				try
				{
					UpdateSysVars();

					var devices = LoadDeviceList();
					foreach (var device in devices)
					{
						token.ThrowIfCancellationRequested();
						HandleDevice(device, token);

						if (token.IsCancellationRequested)
						{
							break;
						}
					}

					await DelayLoopAsync(token).ConfigureAwait(false);
				}
				catch (OperationCanceledException)
				{
					_logger.WriteLine(4, $"{vLoc} cancellation requested.");
					break;
				}
				catch (Exception ex)
				{
					_logger.WriteLine(0, $"{vLoc}, {ex.Message}.");
				}
			}

			_logger.WriteLine(5, $"{vLoc}, stopped.");
		}

		private void HandleDevice(Device device, CancellationToken token)
		{
			var vLoc = $"{TYPE_NAME}::{nameof(HandleDevice)}(device.Id={device?.Id})";

			try
			{
				if (device == null)
				{
					_logger.WriteLine(1, $"{vLoc}, device is null.");
					return;
				}

				if (_processingDevices.TryAdd(device.Id, DateTime.Now))
				{
					StartDeviceProcessing(device, token);
					return;
				}

				if (_processingDevices.TryGetValue(device.Id, out var startedAt) &&
					startedAt < DateTime.Now.AddMinutes(FAILURE_DELAY_MINUTES))
				{
					_logger.WriteLine(3, $"{vLoc}, Device Id: {device.Id} already under scanning, but it took too long [projected:{startedAt.ToString(Defaults.DEFAULT_DATETIME_FORMAT)}]...");
					_processingDevices[device.Id] = DateTime.Now;
					StartDeviceProcessing(device, token);
				}
				else
				{
					_logger.WriteLine(4, $"{vLoc}, Device Id: {device.Id} already under scanning..");
				}
			}
			catch (Exception ex)
			{
				_logger.WriteLine(0, $"{vLoc}, {ex.Message}.");
			}
		}

		private void StartDeviceProcessing(Device device, CancellationToken token)
		{
			var state = new ThreadStateObject
			{
				Device = device,
				Db = _dbFactory(),
				State = ThreadState.Started,
				Logger = _logger,
				CurrentlyProcessingDevices = _processingDevices,
				DebugThreadId = GenerateThreadId(device.Id),
			};

			Task.Run(async () => await RunProcessorAsync(state, token).ConfigureAwait(false), token);
		}

		private async Task RunProcessorAsync(ThreadStateObject state, CancellationToken token)
		{
			var vLoc = $"{state.DebugThreadId}/{TYPE_NAME}::{nameof(RunProcessorAsync)}";

			try
			{
				var processor = _processorFactory.Create(state);
				await processor.StartAsync().ConfigureAwait(false);
			}
			catch (OperationCanceledException)
			{
				_logger.WriteLine(4, $"{vLoc} cancelled.");
			}
			catch (Exception ex)
			{
				_logger.WriteLine(0, $"{vLoc}, {ex.Message}.");
			}
			finally
			{
				if (state?.Device != null)
				{
					_processingDevices.TryRemove(state.Device.Id, out _);
				}
			}
		}

		private void UpdateSysVars()
		{
			var vLoc = $"{TYPE_NAME}::{nameof(UpdateSysVars)}";

			try
			{
				if (_db == null)
				{
					_logger.WriteLine(0, $"{vLoc}, database connection is not initialized.");
					return;
				}

				_vars = _db.SysVarsGet(SysVarModule.Scan);

				_loopDelaySeconds = GetIntVar(SysVarName.SCAN_LOOP_DELAY, DEFAULT_LOOP_DELAY_SECONDS);

				if (_vars.ContainsKey(SysVarName.SCAN_LOG_LEVEL))
				{
					_logger.LogLevel = Convert.ToInt32(_vars[SysVarName.SCAN_LOG_LEVEL]);
				}

				if (_logger is FileLogger logger)
				{
					if (_vars.ContainsKey(SysVarName.SCAN_LOG_LOCATION))
					{
						logger.LogFolder = _vars[SysVarName.SCAN_LOG_LOCATION].ToString();
					}

					if (_vars.ContainsKey(SysVarName.SCAN_LOG_USE_DATES))
					{
						logger.UseDatesInLogFileNames = _vars[SysVarName.SCAN_LOG_USE_DATES].ToString().Equals("1");
					}
				}
			}
			catch (Exception ex)
			{
				_logger.WriteLine(0, $"{vLoc}, {ex.Message}.");
			}
		}

		private List<Device> LoadDeviceList()
		{
			var vLoc = $"{TYPE_NAME}::{nameof(LoadDeviceList)}";
			_logger.WriteLine(5, vLoc);

			List<Device> devices = null;

			try
			{
				if (_db == null)
				{
					_logger.WriteLine(0, $"{vLoc}, database connection is not initialized.");
					return new List<Device>();
				}

				var sql = "prcDeviceListForProcessing";
				_logger.WriteLine(5, $"{vLoc}, SQL: {sql}");
				var ds = _db.QueryDs(sql);

				devices = ProjectDevices(ds);
				var datapoints = ProjectDatapoints(ds);

				foreach (var device in devices)
				{
					device.Datapoints = datapoints.Where(dp => dp.DeviceId == device.Id).ToList();
					_logger.WriteLine(5, $"Device Id={device.Id}, Protocol={device.Protocol}, NumDatapoints={device.Datapoints?.Count}");
				}
			}
			catch (Exception ex)
			{
				_logger.WriteLine(0, $"{vLoc}, {ex.Message}.");
			}

			return devices ?? new List<Device>();
		}

		private static List<Device> ProjectDevices(DataSet ds)
		{
			const int devicesTable = 0;

			return (from row in ds.Tables[devicesTable].AsEnumerable()
					select new Device
					{
						Id = row.Field<int>(nameof(Device.Id)),
						ObjectId = row.Field<int>(nameof(Device.ObjectId)),
						Protocol = row.Field<DeviceProtocol>(nameof(Device.Protocol)),
						UnitId = row.Field<int>(nameof(Device.UnitId)),
						Url = row.Field<string>(nameof(Device.Url)),
						Interval = row.Field<int>(nameof(Device.Interval)),
						LastScanTime = row.Field<DateTime?>(nameof(Device.LastScanTime)),
						ProjectedScanTime = row.Field<DateTime?>(nameof(Device.ProjectedScanTime)),
					}).ToList();
		}

		private static List<Datapoint> ProjectDatapoints(DataSet ds)
		{
			const int datapointsTable = 1;

			return (from row in ds.Tables[datapointsTable].AsEnumerable()
					select new Datapoint
					{
						Id = row.Field<int>(nameof(Datapoint.Id)),
						DeviceId = row.Field<int>(nameof(Datapoint.DeviceId)),
						RegisterAddress = row.Field<int>(nameof(Datapoint.RegisterAddress)),
						RegisterType = row.Field<int>(nameof(Datapoint.RegisterType)),
						FunctionCode = row.Field<int>(nameof(Datapoint.FunctionCode)),
						Multiplier = row.Field<decimal>(nameof(Datapoint.Multiplier)),
						Offset = row.Field<decimal>(nameof(Datapoint.Offset)),
					}).ToList();
		}

		private int GetIntVar(SysVarName name, int defaultValue)
		{
			if (_vars != null && _vars.ContainsKey(name))
			{
				if (int.TryParse(_vars[name].ToString(), out var value))
				{
					return value;
				}
			}

			return defaultValue;
		}

		private async Task DelayLoopAsync(CancellationToken token)
		{
			for (int remaining = _loopDelaySeconds; remaining > 0; remaining -= LOOP_STEP_SECONDS)
			{
				var delay = Math.Min(remaining, LOOP_STEP_SECONDS);
				await Task.Delay(delay * SECOND_DELAY_MS, token).ConfigureAwait(false);
			}
		}

		private string GenerateThreadId(int deviceId)
		{
			var threadId = Interlocked.Increment(ref _threadCounter);
			return string.Format(THREAD_ID_PATTERN, threadId, deviceId);
		}

		public void Dispose()
		{
			_cts?.Cancel();
			_cts?.Dispose();
		}
	}
}
