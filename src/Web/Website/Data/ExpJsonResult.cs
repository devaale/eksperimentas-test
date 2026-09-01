using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json;

namespace Website.Data
{
    /// <summary>
    /// Warning: This class is not used
    /// 
    /// Serializer for Json.NET
    /// @url https://www.newtonsoft.com/json
    /// 
    /// Based on example:
    /// @see http://james.newtonking.com/archive/2008/10/16/asp-net-mvc-and-json-net
    /// </summary>
	public class ExpJsonResult : ActionResult
	{
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public object Data { get; set; }

        public JsonSerializerSettings SerializerSettings { get; set; }
        public Formatting Formatting { get; set; }

        public ExpJsonResult()
        {
            SerializerSettings = new JsonSerializerSettings();
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType)
              ? ContentType
              : "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data != null)
            {
                JsonTextWriter writer = new JsonTextWriter(response.Output) { Formatting = Formatting };

                JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, Data);

                writer.Flush();
            }
        }
    }
}