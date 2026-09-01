#define DOWNLOAD_ASYNC

using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

using M=Experiment.Data.Models;

using Website.Data.Reports;
using Website.Models;
using Experiment.Core;
using System.Globalization;
using Microsoft.Ajax.Utilities;
using System.Drawing;

namespace Website.Controllers
{
	/// <summary>
	/// Experiment controller
	/// </summary>
	[Authorize]
	public class ExperimentController : Controller
	{
		private readonly ApplicationDbContext db = new ApplicationDbContext();

		public ActionResult Tree()
		{
			//var eac = new ExperimentApiController();
			//var result = eac.Tree();	// TEST (redundant)
			return View();
		}
		public ActionResult Reports()
		{
			//var eac = new ExperimentApiController();
			//var result = eac.Tree();	// TEST (redundant)
			return View();
		}
		public ActionResult Users()
		{
			//var eac = new ExperimentApiController();
			//var result = eac.Tree();	// TEST (redundant)
			return View();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reportId"></param>
		/// <returns></returns>
		[AllowAnonymous]
		public ActionResult Download(Guid reportId)
		{
			// Searching for report request in DB
			var reportRequest = db.ReportRequests.Find(reportId);
			M.ChartSearchParams chartParams = null;
			if(reportRequest != null)
			{
				if(!string.IsNullOrEmpty(reportRequest.Params))
				{
					try
					{
						chartParams = JsonConvert.DeserializeObject<M.ChartSearchParams>(reportRequest.Params);
					}
					catch
					{

					}
				}
			}

			// Depends on which result we got.
			if(chartParams == null)
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);

			// Retrieve data from DB				
			var sql = "EXEC [prcDatapointValueList] @dateFrom, @dateTo, @datapointIds, @measureUnit, @aggregation, @type, @comparison";
			var dbValues = db.Database.SqlQuery<M.DatapointValue>(
				sql,
				new SqlParameter("@dateFrom", chartParams.DateFrom),
				new SqlParameter("@dateTo", chartParams.DateTo),
				new SqlParameter("@datapointIds", chartParams.SqlParamDatapointIds),
				new SqlParameter("@measureUnit", chartParams.MeasureUnit.ToString()),
				new SqlParameter("@aggregation", chartParams.AggregationType.ToString()),
				new SqlParameter("@type", chartParams.ValueType),
				new SqlParameter("@comparison", chartParams.SqlParamComparisonYears)
			);

			// CSV
			var ms = new MemoryStream();
			var sw = new StreamWriter(ms);
			var report = new DatapointReport(db, dbValues);

			foreach(var line in report.GetLines())
			{
				sw.WriteLine(line);
			}

			// Flushing stream writer
			sw.Flush();

			// BASED ON (not sure is this best way) https://stackoverflow.com/a/62087449
#if DOWNLOAD_ASYNC
			ms.Seek(0, SeekOrigin.Begin);
			FileStreamResult result = new FileStreamResult(ms, "application/octet-stream")
			{
				FileDownloadName = string.Format(
					"{0}_{1}.csv",
					DateTime.Now.ToString(Defaults.DEFAULT_DATETIME_FORMAT_FILE),
					reportId.ToString("N")
				),
			};
#else
			FileContentResult result = new FileContentResult(ms.GetBuffer(), "text/csv")
			{
				FileDownloadName = string.Format("{0}.csv", reportId.ToString("N")),
			};
#endif
			return result;

		}
	}
}
