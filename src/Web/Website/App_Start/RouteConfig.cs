using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Website.App_Start{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				// @Martynas ordered to remove Home tab, but we left its code
				//defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
				defaults: new { controller = "Experiment", action = "Tree", id = UrlParameter.Optional }
			);
		}
	}
}
