using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Experiment.Core.BL.Data;

using Experiment.Core.Metadata;
using Experiment.Core.Data;

using Experiment.DeviceScanner.Data;
using Experiment.DeviceScanner.Services;
using Newtonsoft.Json;

namespace Experiment.DeviceScanner{
	public class ApiScanner
	{
		#region Constants
		const string TYPE_NAME = nameof(ApiScanner);

		const int THREAD_TIMEOUT = 60; // secs

		#endregion

		#region Attributes
		ScanThreadStateObject _State;
		ILogger _Logger;
		ExpSql _Db;
		ApiService _ApiService;

		#endregion

		#region Ctor

		internal ApiScanner(ScanThreadStateObject state, ILogger logger)
		{
			_State = state;
			_Logger = logger;
			_ApiService = new ApiService(logger);

			var vLoc = string.Format("{0}/{1}::{2}", _State.DebugThreadId, TYPE_NAME, nameof(ApiScanner));
			_Logger.WriteLine(5, vLoc);

			_Db = ExpSql.GenerateFromDefaults(logger);
		}

		#endregion

		#region Methods

		internal async void Start()
		{
			var vLoc = string.Format("{0}/{1}::{2}", _State.DebugThreadId, TYPE_NAME, nameof(Start));
			var stage = "Start";
			_Logger.WriteLine(5, vLoc);

			try
			{
				stage = string.Format("{0}::{1}", nameof(_ApiService), nameof(ApiService.GetAllInfo));
				var json = await _ApiService.GetAllInfo(_State.Device);
				if (!string.IsNullOrEmpty(json))
				{
					stage = nameof(_Db.ScanDataJsonUpdate);
					_Logger.WriteLine(5, string.Format("{0}/{1}", vLoc, stage));
					int affected = _Db.ScanDataJsonUpdate(_State.Device.Id, json);

					stage = "DONE!";
					_Logger.WriteLine(5, string.Format("{0}/{1}", vLoc, stage));
				}
				else
				{
					stage = "NO DATA";
					_Logger.WriteLine(4, string.Format("{0}/{1}", vLoc, stage));
				}
			}
			catch(Exception ex)
			{
				_Logger.WriteLine(0, string.Format("{0}/{1}/{2}", vLoc, stage, ex.Message));
			}
		}


		#endregion
	}
}
