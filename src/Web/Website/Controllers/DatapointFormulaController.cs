using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Description;

using Website.Data;
using Website.Models;

using M = Experiment.Data.Models;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/DatapointFormula")]
	public class DatapointFormulaController : ApiController
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		// GET: api/Datapoint
		public M.DatapointFormula[] GetDatapointFormulas(string lang)
		{
			var sql = "EXEC prcDatapointFormulaList @lang";
			var rawResult = db.Database.SqlQuery<M.DatapointFormula>(
				sql,
				new SqlParameter("@lang", lang));
			return rawResult.ToArray();
		}

		[Route("{formulaId}/PresetChains")]
		public IHttpActionResult GetPresetChains(int formulaId)
		{
			var sql = "SELECT [Order], [ExpectedDataPointName] FROM [dbo].[tblDatapointFormulaPresetChain] WHERE [FormulaId] = @formulaId ORDER BY [Order]";
			var result = db.Database.SqlQuery<M.DatapointFormulaPresetChain>(
				sql,
				new SqlParameter("@formulaId", formulaId));
			return Ok(result.ToArray());
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool DatapointFormulaExists(int id)
		{
			return db.DatapointFormulas.Count(e => e.Id == id) > 0;
		}
	}
}