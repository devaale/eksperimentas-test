using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Description;

using Microsoft.AspNet.Identity;

using Experiment.Core;
using M = Experiment.Data.Models;

using Website.Models;
using System.Data.Entity.Validation;
using Experiment.Core.Data;
using Microsoft.EntityFrameworkCore.Internal;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/Datapoint")]
	public class DatapointController : ApiController
	{
		#region Const
		const string TYPE_NAME = nameof(DatapointController);
		const bool DEBUG = true;
		const string SYSTEM_PREFIX = "System_";
		static readonly int[] SYSTEM_NORDPOOL_DATAPOINT_IDS = new[] { 177, 178, 2793, 2794 };
		static readonly int[] SYSTEM_IRRADIANCE_DATAPOINT_IDS = new[] { 2799, 2800, 2801, 2802, 2803, 2804 };
		static readonly int[] SYSTEM_SHARED_DATAPOINT_IDS = SYSTEM_NORDPOOL_DATAPOINT_IDS
			.Concat(SYSTEM_IRRADIANCE_DATAPOINT_IDS)
			.ToArray();

		#endregion

		#region Attributes
		private ApplicationDbContext db = new ApplicationDbContext();


		#endregion

		#region Helpers

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool DatapointExists(int id)
		{
			return db.Datapoints.Count(e => e.Id == id) > 0;
		}

		private bool HasAccessToObject(int objectId)
		{
			var userId = User.Identity.GetUserId();
			return db.Objects.Any(obj =>
				obj.Id == objectId &&
				(obj.UserId == userId || obj.Permissions.Any(op => op.FriendUserId == userId)));
		}

		private static Datapoint CreateDatapointListItem(Datapoint source)
		{
			return new Datapoint()
			{
				Id = source.Id,
				DeviceId = source.DeviceId,
				Order = source.Order,
				Name = FormatDatapointListName(source),
				Description = source.Description,
				MeasureUnit = source.MeasureUnit,
				DatapointType = source.DatapointType,
				RegisterAddress = source.RegisterAddress,
				RegisterType = source.RegisterType,
				FunctionCode = source.FunctionCode,
				Alias = source.Alias,
				Multiplier = source.Multiplier,
				Offset = source.Offset,
				ReadWrite = source.ReadWrite,
				DatapointFormulaId = source.DatapointFormulaId,
				IntervalDatepart = source.IntervalDatepart,
				AggregationDatepart = source.AggregationDatepart,
				LastFormulaCalcTime = source.LastFormulaCalcTime,
				DeviceProtocol = source.DeviceProtocol,
				Topic = source.Topic,
				Theme = source.Theme,
				ResourceUri = source.ResourceUri,
				Payload = source.Payload,
				Instance = source.Instance,
				BACnetObjectType = source.BACnetObjectType,
				BACnetPropertyId = source.BACnetPropertyId,
				BACnetFunctionCode = source.BACnetFunctionCode,
				BACnetDataType = source.BACnetDataType,
				Chains = source.Chains?
					.Select(chain => new DatapointFormulaChain()
					{
						Id = chain.Id,
						DatapointId = chain.DatapointId,
						Order = chain.Order,
						RelatedDatapointId = chain.RelatedDatapointId,
						Value = chain.Value,
					})
					.ToList(),
			};
		}

		private static string FormatDatapointListName(Datapoint datapoint)
		{
			var name = datapoint?.Name ?? string.Empty;
			if (datapoint == null)
			{
				return name;
			}

			if (SYSTEM_NORDPOOL_DATAPOINT_IDS.Contains(datapoint.Id))
			{
				return SYSTEM_PREFIX + name;
			}

			if (SYSTEM_IRRADIANCE_DATAPOINT_IDS.Contains(datapoint.Id))
			{
				var objectName = datapoint.Device?.Object?.Name;
				if (!string.IsNullOrWhiteSpace(objectName))
				{
					return string.Format("{0}{1}_{2}", SYSTEM_PREFIX, objectName, name);
				}

				return SYSTEM_PREFIX + name;
			}

			return name;
		}

		#endregion

		#region Methods
		/// <summary>
		/// Get specific object datapoints
		/// </summary>
		/// <param name="objectId"></param>
		/// <returns></returns>
		public IQueryable<Datapoint> GetDatapoints(int objectId)
		{
			if (!HasAccessToObject(objectId))
			{
				return Enumerable.Empty<Datapoint>().AsQueryable();
			}

			var datapoints = db.Datapoints
				.Where(dp =>
					dp.Device.ObjectId.Equals(objectId) ||
					SYSTEM_SHARED_DATAPOINT_IDS.Contains(dp.Id))
				.Include(dp => dp.Chains)
				.Include(dp => dp.Device)
				.Include(dp => dp.Device.Object)
				.ToList();

			return datapoints
				.GroupBy(dp => dp.Id)
				.Select(group => CreateDatapointListItem(group.First()))
				.OrderBy(dp => dp.Name)
				.AsQueryable();
		}

		/// <summary>
		/// Get specific devices datapoints
		/// </summary>
		/// <param name="deviceIds"></param>
		/// <returns></returns>
		public IQueryable<Datapoint> GetDatapoints(string deviceIds)
		{
			Validation.RequireValid(deviceIds, nameof(deviceIds));
			var ids = deviceIds.Split(Defaults.FIELD_SEPARATOR);

			return db.Datapoints
				.Where(dp => ids.Contains(dp.DeviceId.ToString()))
				.Include(dp => dp.Chains);
		}

		/// <summary>
		/// Get datapints by specific chart params
		/// </summary>
		/// <param name="chartParams"></param>
		/// <returns></returns>
		[Route("Chart")]
		public IEnumerable<M.Datapoint> PostChartDatapoints(M.ChartSearchParams chartParams)
		{
			var vLoc = string.Format("{0}::{1}({2} {3})",
				TYPE_NAME, nameof(GetDatapoints),
				nameof(M.ChartSearchParams), nameof(chartParams));

			Dictionary<int, M.Datapoint> retVal = null;

			if (chartParams != null)
			{
				// Retrieving EF datapoints as Dictionary
				retVal = db.Datapoints
					.Where(dp => chartParams.DatapointIds.Contains(dp.Id))
					.ToDictionary(
						dp => dp.Id,
						dp => new M.Datapoint()
						{
							Id = dp.Id,
							Name = dp.Name
						});

				// Now retrieving all their values, which after this need to assign to each dp
				var sql = "EXEC [prcDatapointValueList] @dateFrom, @dateTo, @datapointIds, @measureUnit, @aggregation, @type, @comparison";
				var datapointValues = db.Database.SqlQuery<M.DatapointValue>(
					sql,
					new SqlParameter("@dateFrom", chartParams.DateFrom),
					new SqlParameter("@dateTo", chartParams.DateTo),
					new SqlParameter("@datapointIds", chartParams.SqlParamDatapointIds),
					new SqlParameter("@measureUnit", chartParams.MeasureUnit.ToString()),
					new SqlParameter("@aggregation", chartParams.AggregationType.ToString()),
					new SqlParameter("@type", chartParams.ValueType),
					new SqlParameter("@comparison", chartParams.SqlParamComparisonYears)
				);

				foreach (var dv in datapointValues)
				{
					if (retVal.ContainsKey(dv.DatapointId))
					{
						var dp = retVal[dv.DatapointId];
						if (dp.Values == null)
						{
							dp.Values = new List<M.DatapointValue>();
						}

						dp.Values.Add(new M.DatapointValue()
						{
							Id = dv.Id,
							DatapointId = dv.DatapointId,
							Date = dv.Date,
							Value = dv.Value,
						});
					}
					else
					{
						Debug.WriteLine(string.Format(
							"{0}, Missing Datapoint(Id={1}) for DatapointValue(Id={2}",
							vLoc, dv.DatapointId, dv.Id));
					}
				}
			}
			return retVal.Values;
		}

		//// POST: api/Datapoint/ByDevices
		///// <summary>
		///// @deprecated
		///// </summary>
		///// <param name="form"></param>
		///// <returns></returns>
		//[Route("ByDevices")]
		//public IQueryable<Datapoint> PostByDevices(FormDataCollection form)
		//{
		//    string devices = form.Get("ids");
		//
		//    IList<string> deviceIds = 
		//        new List<string>(devices.Split(Defaults.FIELD_SEPARATOR));
		//
		//    return db.Datapoints.Where(
		//        datapoint => deviceIds.Contains(datapoint.DeviceId.ToString()));
		//}

		// GET: api/Datapoint/5
		[ResponseType(typeof(Datapoint))]
		public IHttpActionResult GetDatapoint(int id)
		{
			Datapoint datapoint = db.Datapoints.Find(id);
			if (datapoint == null)
			{
				return NotFound();
			}

			db.Entry(datapoint).Collection(nameof(Datapoint.Chains)).Load();

			return Ok(datapoint);
		}

		// PUT: api/Datapoint/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutDatapoint(int id, Datapoint datapoint)
		{
			var vLoc = string.Format("{0}::{1}(int id={2}, Datapoint datapoint)",
				TYPE_NAME, nameof(PutDatapoint), id);
			Debug.WriteLineIf(DEBUG, vLoc);

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != datapoint.Id)
			{
				return BadRequest();
			}

			// Find device
			Device device = db.Devices.Find(datapoint.DeviceId);
			if (device == null)
			{
				return NotFound();
			}

			// Find object
			EObject obj = db.Objects.Find(device.ObjectId);
			if (obj == null)
			{
				return NotFound();
			}

			// Check that user updated only own datapoints
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(obj.UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			using (var transaction = db.Database.BeginTransaction())
			{
				try
				{
					// Disconnecting datapoint from its chains
					// As after datapoint update chains will be updated
					var fChains = datapoint.Chains;	// Getting front-end chains
					datapoint.Chains = null;

					// Update datapoint
					Debug.WriteLineIf(DEBUG, string.Format("{0}, Update datapoint: Id=({1}), Name=({2})...",
						vLoc, datapoint.Id, datapoint.Name));
					db.Entry(datapoint).State = EntityState.Modified;
					db.SaveChanges();

					// If we have front-end chains at all
					if(fChains != null)
					{
						// Getting back-end chains
						var bChains = db.DatapointFormulaChains.Where(c => c.DatapointId == datapoint.Id).ToArray();
						
						// Scanning them, do they still exist in 
						foreach (var bChain in bChains)
						{
							// Cleaning if in front-end chains it doesn't exist.
							if (!fChains.Any(fc => fc.Id == bChain.Id))
							{
								db.DatapointFormulaChains.Remove(bChain);
								db.SaveChanges();
							}
						}

						// Now updating-adding still existing ones
						foreach(var fChain in fChains)
						{
							var saved = false;
							if (fChain.Id != 0)
							{
								Debug.WriteLineIf(DEBUG, string.Format("{0}, Modified, Id={1}",
									vLoc, fChain.Id));
								var bChain = db.DatapointFormulaChains.Find(fChain.Id);
								if(bChain != null)
								{
									db.Entry(bChain).CurrentValues.SetValues(fChain);
									//db.Entry(fChain).State = EntityState.Modified;
									db.SaveChanges();
									saved = true;
								}
							}

							if (!saved)
							{
								Debug.WriteLineIf(DEBUG, string.Format("{0}, Add, Id={1}",
									vLoc, fChain.Id));
								db.DatapointFormulaChains.Add(fChain);
								db.SaveChanges();
							}
						}
					}


					/*
					// Update datapoint
					Debug.WriteLineIf(DEBUG, string.Format("{0}, Update datapoint: Id=({1}), Name=({2})...",
						vLoc, datapoint.Id, datapoint.Name));
					db.Entry(datapoint).State = EntityState.Modified;
					db.SaveChanges();

					// Updating Datapoint Chains
					Debug.WriteLineIf(DEBUG, string.Format("{0}, Updating Datapoint Chains...",
						vLoc));
					if (datapoint.Chains != null)
					{
						// Update datapoint chains
						foreach (var chain in datapoint.Chains)
						{
							if (chain.Id != 0)
							{
								Debug.WriteLineIf(DEBUG, string.Format("{0}, Modified, Id={1}",
									vLoc, chain.Id));
								db.Entry(chain).State = EntityState.Modified;
							}
							else
							{
								Debug.WriteLineIf(DEBUG, string.Format("{0}, Add, Id={1}",
									vLoc, chain.Id));
								db.DatapointFormulaChains.Add(chain);
							}
						}
					}
					db.SaveChanges();
					*/
					transaction.Commit();
				}
				//catch (DbUpdateConcurrencyException)
				catch (Exception ex)
				{
					transaction.Rollback();

					if (!DatapointExists(id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
			}

			return StatusCode(HttpStatusCode.NoContent);
		}

		// POST: api/Datapoint
		[ResponseType(typeof(Datapoint))]
		public IHttpActionResult PostDatapoint(Datapoint datapoint)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Find device
			Device device = db.Devices.Find(datapoint.DeviceId);
			if (device == null)
			{
				return NotFound();
			}

			// Find object
			EObject obj = db.Objects.Find(device.ObjectId);
			if (obj == null)
			{
				return NotFound();
			}

			// Check that user updated only own datapoints
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(obj.UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.Datapoints.Add(datapoint);
			db.SaveChanges();

			return CreatedAtRoute("DefaultApi", new { id = datapoint.Id }, datapoint);
		}

		// DELETE: api/Datapoint/5
		[ResponseType(typeof(Datapoint))]
		public IHttpActionResult DeleteDatapoint(int id)
		{
			// Find datapoint
			Datapoint datapoint = db.Datapoints.Find(id);
			if (datapoint == null)
			{
				return NotFound();
			}

			// Find device
			Device device = db.Devices.Find(datapoint.DeviceId);
			if (device == null)
			{
				return NotFound();
			}

			// Find object
			EObject obj = db.Objects.Find(device.ObjectId);
			if (obj == null)
			{
				return NotFound();
			}

			// Check that user updated only own datapoints
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(obj.UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.Datapoints.Remove(datapoint);
			db.SaveChanges();

			return Ok(datapoint);
		}

		/// <summary>
		/// Returns specified object grouped datapoints, where device is group name
		/// </summary>
		/// <param name="objectId"></param>
		/// <returns></returns>
		[Route("GroupedDatapoints")]
		public IEnumerable<M.GroupedIntIdItem> GetDashboardDatapoints(int objectId)
		{
			var retVal = new List<M.GroupedIntIdItem>();

			// find an object
			var obj = db.Objects
				.Where(o => o.Id == objectId)
				.Include(o => o.Permissions)
				.FirstOrDefault();

			var ok = obj != null;	// Check is object okay
			ok &= obj.Permissions != null;	// and its permissions loaded/initialized

			if(ok)
			{
				// retrieve logged in userId
				var userId = User.Identity.GetUserId();

				// Check do user has right to work with specific object
				var hasRight = obj.UserId.Equals(userId);
				if (!hasRight)
					hasRight = obj.Permissions.Any(op => op.FriendUserId.Equals(userId));

				if (hasRight)
				{
					// Object datapoints
					var datapoints = db.Datapoints
						.Where(dp => dp.Device.ObjectId.Equals(objectId))
						.Include(dp => dp.Device)
						.OrderBy(dp => dp.Device.Name)
						.ThenBy(dp => dp.Name);

					foreach (var dp in datapoints)
					{
						retVal.Add(new M.GroupedIntIdItem()
						{
							Id = dp.Id,
							Name = dp.Name,
							Group = dp.Device.Name,
						});
					}
				}
			}

			return retVal;
		}
		#endregion

	}
}
