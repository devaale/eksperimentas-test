using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

using Microsoft.AspNet.Identity;

using Experiment.Data.Models;

using Website.Data;
using Website.Models;
using System.Data.SqlClient;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/Situation")]
	public class SituationController : ApiController
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		[HttpGet]
		public Situation GetCurrentSituation()
		{
			var sql = "EXEC prcSituation @userId";
			var rawResult = db.Database.SqlQuery<Situation>(
				sql, new SqlParameter("@userId", User.Identity.GetUserId()));

			var collection = rawResult.ToList();
			if (collection.Count > 0)
				return collection[0];
			return null;
		}

	}
}
