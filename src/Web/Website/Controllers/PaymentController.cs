using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;

using Website.Data;
using Website.Models;

namespace Website.Controllers
{
	/// <summary>
	/// Experiment controller
	/// </summary>
	[Authorize]
	public class PaymentController : Controller
	{
		[AllowAnonymous]
		public ActionResult Accept()
		{
			return View();
		}

		[AllowAnonymous]
		public ActionResult Cancel()
		{
			return View();
		}
	}
}