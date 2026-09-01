using System;
using System.Collections.Generic;
using System.Data;

using Experiment.Core.BL.Data;
using Experiment.Core.Enums;
using Experiment.Core.IO;
using Experiment.Core.Metadata;
using Experiment.Data.Models;

namespace Experiment.FormulaCalculation.Service
{
	internal sealed class DatapointProvider : IDatapointProvider
	{
		readonly IExpSqlFactory _dbFactory;
		readonly ILogger _logger;

		public DatapointProvider(IExpSqlFactory dbFactory, ILogger logger)
		{
			_dbFactory = dbFactory;
			_logger = logger;
		}

		/// <summary>
		/// Returns from database datapoints,
		/// but also converts DataSet/DataTable to ORM Datapoint objects with datapoint chains
		/// </summary>
		/// <returns></returns>
		public List<Datapoint> GetDatapoints()
		{
			var vLoc = string.Format("{0}::{1}()", FormulaCalculationLogContext.TypeName, nameof(GetDatapoints));
			var vStep = nameof(GetDatapoints);

			var datapoints = new List<Datapoint>();

			try
			{
				vStep = nameof(ExpSql.GenerateFromDefaults);
				_logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));
				var db = _dbFactory.Create();

				vStep = nameof(ExpSql.GetVirtualDatapointsDs);
				_logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));
				var ds = db.GetVirtualDatapointsDs();

				vStep = "ds.Tables/init";
				_logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));
				DataTable datapointsTable = null;
				DataTable datapointChainsTable = null;

				if (ds != null)
				{
					if (ds.Tables.Count > 0)
					{
						datapointsTable = ds.Tables[0];
					}

					if (ds.Tables.Count > 1)
					{
						datapointChainsTable = ds.Tables[1];
					}
				}

				vStep = nameof(datapointsTable);
				_logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));
				if (datapointsTable != null)
				{
					foreach (DataRow row in datapointsTable.Rows)
					{
						var datapoint = new Datapoint()
						{
							Id = (int)row[nameof(Datapoint.Id)],
							DeviceId = (int)row[nameof(Datapoint.DeviceId)],
							Order = (int)row[nameof(Datapoint.Order)],
							Name = row[nameof(Datapoint.Name)].ToString(),
							Description = row[nameof(Datapoint.Description)].ToString(),

							AggregationDatepart = (DatePartOrInterval)(byte)row[nameof(Datapoint.IntervalDatepart)],
							IntervalDatepart = (DatePartOrInterval)(byte)row[nameof(Datapoint.IntervalDatepart)],
							DatapointFormulaId = null,
							LastFormulaCalcTime = (DateTime)row[nameof(Datapoint.LastFormulaCalcTime)],

							Chains = new List<DatapointFormulaChain>(),
						};

						if (!DBNull.Value.Equals(row[nameof(Datapoint.DatapointFormulaId)]))
							datapoint.DatapointFormulaId = (int)row[nameof(Datapoint.DatapointFormulaId)];

						datapoints.Add(datapoint);
					}
				}

				vStep = nameof(datapointChainsTable);
				_logger.WriteLine(5, string.Format("{0}, {1}", vLoc, vStep));
				if (datapointChainsTable != null)
				{
					foreach (var datapoint in datapoints)
					{
						var view = new DataView(datapointChainsTable);

						view.RowFilter = string.Format("{0} = {1}",
							nameof(DatapointFormulaChain.DatapointId), datapoint.Id);

						view.Sort = string.Format("{0} ASC",
							nameof(DatapointFormulaChain.Order));

						foreach (DataRowView v in view)
						{
							var chain = new DatapointFormulaChain()
							{
								Id = (int)v.Row[nameof(DatapointFormulaChain.Id)],
								DatapointId = (int)v.Row[nameof(DatapointFormulaChain.DatapointId)],
								Order = (int)v.Row[nameof(DatapointFormulaChain.Order)],
								RelatedDatapointId = (int)v.Row[nameof(DatapointFormulaChain.RelatedDatapointId)],
								Value = null,
							};

							if (!DBNull.Value.Equals(v.Row[nameof(DatapointFormulaChain.Value)]))
							{
								chain.Value = (decimal)v.Row[nameof(DatapointFormulaChain.Value)];
							}

							datapoint.Chains.Add(chain);
						}
					}
				}
			}
			catch (Exception ex)
			{
				var errorMsg = string.Format(
					"{0}, Failed at: {1}, with: {2}",
					vLoc, vStep, ex.Message);
				_logger.WriteLine(0, errorMsg);

				throw ex;
			}

			return datapoints;
		}
	}
}
