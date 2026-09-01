using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Website.Data;
using Website.Models;

namespace Website.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}
	}
}
