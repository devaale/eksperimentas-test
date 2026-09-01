using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Web.Http;
using System.Web.Http.Description;

using Microsoft.AspNet.Identity;

using M = Experiment.Data.Models;
using Experiment.Data.Metadata;
using Experiment.Data.Models;

using Website.Data;
using Website.Models;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/License")]
	public class LicenseController : ApiController
	{

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

		private bool LicenseExists(int id)
		{
			return db.Licenses.Count(e => e.Id == id) > 0;
		}

		#endregion

		#region Methods

		#endregion
	}
}