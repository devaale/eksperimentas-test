using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;

namespace Website.App_Start{
	public class BundleConfig
	{
		// For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
				"~/Scripts/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
				"~/Scripts/jquery.unobtrusive*",
				"~/Scripts/jquery.validate*"));

			bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
				"~/Scripts/knockout-{version}.js",
				"~/Scripts/knockout.validation.js"));

			bundles.Add(new ScriptBundle("~/bundles/app").Include(
				"~/Scripts/sammy-{version}.js",
				"~/Scripts/app/common.js",
				"~/Scripts/app/app.datamodel.js",
				"~/Scripts/app/app.viewmodel.js",
				"~/Scripts/app/home.viewmodel.js",
				"~/Scripts/app/_run.js"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
				"~/Scripts/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
				"~/Scripts/bootstrap.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
				 "~/Content/bootstrap.css",
				 "~/Content/site.css",
				 "~/Content/Element.css",
				 "~/Content/experiment.css",
				 "~/Content/themes/default/style.css")	// jstree
			);

			/**
			 * Added exclusively for Experiment project
			 */
			// Experiment required dependencies
			bundles.Add(new ScriptBundle("~/bundles/experiment-req")
				.Include(
					"~/Scripts/app/common.js"
					,"~/Scripts/jstree.js"
					, "~/Scripts/experiment-req/Element.js"
					, "~/Scripts/experiment-req/ContentEngine.js"
					, "~/Scripts/experiment-req/Controls.js"
				)
//				.IncludeDirectory("~/Scripts/req/", "*.js", true)
			);

			// This should be last, after all libs loaded
			// @see https://www.tutorialsteacher.com/mvc/scriptbundle-mvc
			bundles.Add(new ScriptBundle("~/bundles/experiment")
				.IncludeDirectory("~/Scripts/experiment/", "*.js", true));
		}
	}
}
