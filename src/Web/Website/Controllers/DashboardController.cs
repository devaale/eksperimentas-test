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
using Experiment.Core.Enums;
using Experiment.Data.Enums;
using M = Experiment.Data.Models;

using Website.Models;
using System.ComponentModel.Design;
using Experiment.Core.Data;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/Dashboard")]
	public class DashboardController : ApiController
	{
		#region Attributes
		private ApplicationDbContext db = new ApplicationDbContext();

		#endregion

		#region Methods

		/// <summary>
		/// Returns current user DashboardSetting
		/// </summary>

		/// <summary>
		/// Returns current user DashboardSetting for specific object
		/// </summary>
		/// <param name="objectId">The object ID to get settings for</param>
		/// <returns></returns>
		[Route("DashboardSetting")]
		[ResponseType(typeof(M.DashboardSetting))]
		public IHttpActionResult GetDashboardSetting(int objectId)
		{
			// retrieve logged in userId
			var userId = User.Identity.GetUserId();

			// Retrieve specific user's DashboardSettings for this object (composite key: UserId + ObjectId)
			var bSettings = db.DashboardSettings.FirstOrDefault(x => x.ObjectId == objectId && x.UserId == userId);

			// If DashboardSetting for this user + object unavailable
			if (bSettings == null)
			{
				// Create it
				bSettings = new DashboardSetting()
				{
					UserId = userId,
					ObjectId = objectId,
				};
				db.DashboardSettings.Add(bSettings);
				db.SaveChanges();
			}

			// Prepare FRONT-END DashboardSetting (retVal)
			// They are different. Front-end one has datapoints collection, while backend one, not.
			var fSettings = new M.DashboardSetting()
			{
				UserId = userId,
				ObjectId = objectId,
				DateRange = bSettings.DateRange,
				Datapoints = new List<M.DashboardDatapoint>(),

				Graph1Type = bSettings.Graph1Type,
				Graph1Interval = bSettings.Graph1Interval,
				Graph1Difference = bSettings.Graph1Difference,
				Graph1Aggregation = bSettings.Graph1Aggregation,

				Graph2Type = bSettings.Graph2Type,
				Graph2Interval = bSettings.Graph2Interval,
				Graph2Difference = bSettings.Graph2Difference,
				Graph2Aggregation = bSettings.Graph2Aggregation,

				Graph3Type = bSettings.Graph3Type,
				Graph3Interval = bSettings.Graph3Interval,
				Graph3Difference = bSettings.Graph3Difference,
				Graph3Aggregation = bSettings.Graph3Aggregation,

				Graph4Type = bSettings.Graph4Type,
				Graph4Interval = bSettings.Graph4Interval,
				Graph4Difference = bSettings.Graph4Difference,
				Graph4Aggregation = bSettings.Graph4Aggregation,
			};

			// Retrieve specific user dashboard datapoints for this object
			// Optimized: Use projection to avoid Include and reduce data transfer
			var bDashDatapoints = db.DashboardDatapoints
				.Where(ds => userId.Equals(ds.UserId) && ds.ObjectId == objectId)
				.Select(ds => new
				{
					ds.Id,
					ds.DatapointId,
					ds.GraphId,
					DatapointName = ds.Datapoint.Name
				})
				.ToList();

			foreach (var dashDp in bDashDatapoints)
			{
				fSettings.Datapoints.Add(new M.DashboardDatapoint()
				{
					Id = dashDp.Id,
					Name = dashDp.DatapointName,
					DatapointId = dashDp.DatapointId,
					GraphId = dashDp.GraphId,
					//UserId = ddp.UserId,
				});
			}
			return Ok(fSettings);
		}

		/// <summary>
		/// Update DashboardSetting using front-end DashboardSetting, which includes as well datapoints (different table).
		/// ObjectId is taken from the fds model.
		/// </summary>
		/// <param name="fds"></param>
		/// <returns></returns>
		public IHttpActionResult PostDashboardSetting(M.DashboardSetting fds)
		{
			var userId = User.Identity.GetUserId();
			var objectId = fds.ObjectId;
			DashboardSetting bds = null;

			using (var transaction = db.Database.BeginTransaction())
			{
				try
				{
					// Find specific user's DashboardSetting for this object (composite key)
					bds = db.DashboardSettings
						.FirstOrDefault(x => x.UserId == userId && x.ObjectId == objectId);

					// If it found
					if(bds != null)
					{
						// Updating it
						bds.UserId = userId;
						bds.ObjectId = objectId;
						bds.DateRange = fds.DateRange;

						bds.Graph1Type = fds.Graph1Type;
						bds.Graph1Interval = fds.Graph1Interval;
						bds.Graph1Difference = fds.Graph1Difference;
						bds.Graph1Aggregation = fds.Graph1Aggregation;

						bds.Graph2Type = fds.Graph2Type;
						bds.Graph2Interval = fds.Graph2Interval;
						bds.Graph2Difference = fds.Graph2Difference;
						bds.Graph2Aggregation = fds.Graph2Aggregation;

						bds.Graph3Type = fds.Graph3Type;
						bds.Graph3Interval = fds.Graph3Interval;
						bds.Graph3Difference = fds.Graph3Difference;
						bds.Graph3Aggregation = fds.Graph3Aggregation;

						bds.Graph4Type = fds.Graph4Type;
						bds.Graph4Interval = fds.Graph4Interval;
						bds.Graph4Difference = fds.Graph4Difference;
						bds.Graph4Aggregation = fds.Graph4Aggregation;

						db.Entry(bds).State = EntityState.Modified;
					}
					else
					{
						// It not found, creating it
						bds = new DashboardSetting()
						{
							UserId = userId,
							ObjectId = objectId,
						};
						db.DashboardSettings.Add(bds);
					}
					db.SaveChanges();

					// DashboardDatapoints - filter by both UserId and ObjectId
					var savedDps = db.DashboardDatapoints.Where(d => userId.Equals(d.UserId) && d.ObjectId == objectId);

					// First remove all for this user + object
					foreach(var dp in savedDps)
					{
						db.DashboardDatapoints.Remove(dp);
						db.SaveChanges();
					}

					// Now add only needed ones
					foreach(var dp in fds.Datapoints)
					{
						db.DashboardDatapoints.Add(new DashboardDatapoint()
						{
							UserId = userId,
							ObjectId = objectId,
							DatapointId = dp.DatapointId,
							GraphId = dp.GraphId,
						});
					}
					db.SaveChanges();

					transaction.Commit();
				}
				catch(Exception ex)
				{
					transaction.Rollback();
					throw ex;
				}
				finally
				{

				}
			}
			return CreatedAtRoute("DefaultApi", new { UserId = bds.UserId, ObjectId = bds.ObjectId }, bds);
		}

		/*
		 @deprecated & never used (delete it!)
		[Route("DashboardData")]
		[ResponseType(typeof(M.DashboardData))]
		public IHttpActionResult GetDashboardData()
		{
			var userId = User.Identity.GetUserId();
			var retVal = new M.DashboardData();
			var settings = db.DashboardSettings.Find(userId);

			if(settings != null)
			{
				//settings.DateRange

			}

			return Ok(retVal);
		}
		*/
		#endregion
	}
}