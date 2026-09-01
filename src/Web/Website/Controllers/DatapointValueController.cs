//#define LEGACY_LINQ
//#define DUMP_DATA

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Description;

using EFC = Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.Identity;

using Newtonsoft.Json;

using Experiment.Core;
using Experiment.Core.Enums;

using Experiment.Data.Enums;
using M = Experiment.Data.Models;

using Website.Data;
using Website.Models;
using Microsoft.Ajax.Utilities;
using System.Web.UI.WebControls;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/DatapointValue")]
	public class DatapointValueController : ApiController
	{
		#region Const
		const string TYPE_NAME = nameof(DatapointValueController);

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

		private bool DatapointValueExists(int id)
		{
			return db.DatapointValues.Count(e => e.Id == id) > 0;
		}

		#endregion

		#region Methods
		/// <summary>
		/// @deprecated?
		/// 
		/// Currently (2023-07-21) uses only Dashboard Charts
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		[Route("Values")]
		public IQueryable<DatapointValue> PostValues (M.DatapointValueFilter filter)
		{
			var vLoc = string.Format("{0}::{1} (M.DatapointValueFilter filter)", TYPE_NAME, nameof(PostValues));

			if (filter == null)
				throw new ArgumentNullException(string.Format("{0}, {1} should be not NULL!", vLoc, nameof(filter)));

			// Don't execute this on front-end (mobile) side as mobile device may have wrong date
			// Only server always has good date, why Parse should be done exceptionally on the server
			filter.Parse();

			return db.DatapointValues
				.Where(dv =>
					filter.DatapointIds.Contains(dv.DatapointId) &&
					(dv.Date >= filter.DateFrom.Value &&
					dv.Date <= filter.DateTo))
				.OrderBy(dv => dv.DatapointId)
				.ThenBy(dv => dv.Date);
		}

		[Route("Search")]
		public IEnumerable<M.DatapointValue> PostSearch(M.ChartSearchParams chartParams)
		{
			var sql = "EXEC [prcDatapointValueList] @dateFrom, @dateTo, @datapointIds, @measureUnit, @aggregation, @type, @comparison";
			var result = db.Database.SqlQuery<M.DatapointValue>(
				sql,
				new SqlParameter("@dateFrom", chartParams.DateFrom),
				new SqlParameter("@dateTo", chartParams.DateTo),
				new SqlParameter("@datapointIds", chartParams.SqlParamDatapointIds),
				new SqlParameter("@measureUnit", chartParams.MeasureUnit.ToString()),
				new SqlParameter("@aggregation", chartParams.AggregationType.ToString()),
				new SqlParameter("@type", chartParams.ValueType),
				new SqlParameter("@comparison", chartParams.SqlParamComparisonYears)
			);

			return result;
		}

		[Route("Download")]
		public string PostDownload(M.ChartSearchParams chartParams)
		{
			var newId = Guid.NewGuid();
			var reportRequest = new ReportRequest()
			{
				Id = newId,
				UserId = User.Identity.GetUserId(),
				Type = ReportRequestType.Default,
				Params = JsonConvert.SerializeObject(chartParams),
			};

			db.ReportRequests.Add(reportRequest);
			db.SaveChanges();

			var url = Url.Link("Default", new { 
				Controller = "Experiment", 
				Action = "Download", 
				reportId = newId.ToString("N")
			});   // My own created Web Api 2.00 variant of it
			return url;
		}

		/*
		// GET: api/DatapointValue
		public IQueryable<DatapointValue> GetDatapointValues()
		{
			return db.DatapointValues;
		}

		// GET: api/DatapointValue/5
		[ResponseType(typeof(DatapointValue))]
		public IHttpActionResult GetDatapointValue(int id)
		{
			DatapointValue datapointValue = db.DatapointValues.Find(id);
			if (datapointValue == null)
			{
				return NotFound();
			}

			return Ok(datapointValue);
		}
		*/

		// PUT: api/DatapointValue/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutDatapointValue(int id, DatapointValue datapointValue)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != datapointValue.Id)
			{
				return BadRequest();
			}

			db.Entry(datapointValue).State = EntityState.Modified;

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!DatapointValueExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return StatusCode(HttpStatusCode.NoContent);
		}
		/*
		// POST: api/DatapointValue
		[ResponseType(typeof(DatapointValue))]
		public IHttpActionResult PostDatapointValue(DatapointValue datapointValue)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			db.DatapointValues.Add(datapointValue);
			db.SaveChanges();
		
			return CreatedAtRoute("DefaultApi", new { id = datapointValue.Id }, datapointValue);
		}

		// DELETE: api/DatapointValue/5
		[ResponseType(typeof(DatapointValue))]
		public IHttpActionResult DeleteDatapointValue(int id)
		{
			DatapointValue datapointValue = db.DatapointValues.Find(id);
			if (datapointValue == null)
			{
				return NotFound();
			}

			db.DatapointValues.Remove(datapointValue);
			db.SaveChanges();
		
			return Ok(datapointValue);
		}
		*/
		#endregion
	}
}