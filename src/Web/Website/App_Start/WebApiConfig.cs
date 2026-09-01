#define ENABLE_MODIFICATIONS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

using Microsoft.Owin.Security.OAuth;

using Newtonsoft.Json.Serialization;

namespace Website.App_Start{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services
			// Configure Web API to use only bearer token authentication.
			config.SuppressDefaultHostAuthentication();
			config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

			// Use camel case for JSON data.
			config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

			// Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);

#if ENABLE_MODIFICATIONS
			// 2022-08-23 Added by A.G.
			// @see https://stackoverflow.com/questions/9847564/how-do-i-get-asp-net-web-api-to-return-json-instead-of-xml-using-chrome
			config.Formatters.JsonFormatter.SupportedMediaTypes
				.Add(new MediaTypeHeaderValue("text/html"));

#endif
		}
	}
}
