using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Experiment.Core.Metadata;
using Experiment.Core.Data;
using Experiment.Core.BL.Data;

namespace Experiment.DeviceScanner.Data{
	public class DatapointsCollection
	{
		Dictionary<string, CacheProvider<List<Datapoint>>> _MainCollection;
		ExpSql _Db;
		ILogger _Logger;

		public DatapointsCollection(ExpSql db)
		{
			_Db = db;
			_MainCollection = new Dictionary<string, CacheProvider<List<Datapoint>>>();
		}

		public List<Datapoint> GetData(string deviceId)
		{
			if (!_MainCollection.ContainsKey(deviceId))
			{
				CacheProvider<List<Datapoint>> dataPoint = new CacheProvider<List<Datapoint>>(
					delegate()
					{
						return GetDataFromDb(deviceId);
					}
				);
				_MainCollection.Add(deviceId, dataPoint);
			}
			return _MainCollection[deviceId].GetData();
		}

		List<Datapoint> GetDataFromDb (string deviceId)
		{
			List<Datapoint> retVal = new List<Datapoint>();
			DataTable table = _Db.PhysicalDatapointListByDeviceId(deviceId);
			if(_Db.IsError)
			{
				_Logger.WriteLine(1, "DataPointsCollection::GetDataFromDb failed with msg: " + _Db.ErrorMsg);
			}

			foreach(DataRow r in table.Rows)
			{
				retVal.Add(new Datapoint(r));
			}

			return retVal;
		}
	}
}
