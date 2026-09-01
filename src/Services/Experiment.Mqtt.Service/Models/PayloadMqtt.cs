using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Experiment.Core;
using Experiment.Core.Metadata;

namespace Experiment.Mqtt.Service.Models{
	public class PayloadMqtt
	{
		const bool DO_REGEXP = false;

		/// <summary>
		/// Rule #1 lowercase all!
		/// </summary>
		static readonly Dictionary<string, decimal> KnownStrings = new Dictionary<string, decimal>()
		{
			{ "true", 1m },
			{ "on", 1m },
			{ "false", 0m },
			{ "off", 0m },
		};

		static public Dictionary<string, decimal> Parse(
			ILogger logger, object o)
		{
			Validation.RequireValid(logger, nameof(logger));
			Validation.RequireValid(o, nameof(o));

			var col = new Dictionary<string, decimal>();
			var path = string.Empty;

			Parse(logger, col, path, o);

			return col;
		}

		static public void Parse(
			ILogger logger, 
			Dictionary<string, decimal> col, 
			string path,
			object o)
		{
			decimal value = 0;

			// If o is null, return
			if (o == null)
				return;

			// If this is Newtonsoft.Json.Linq.JObject
			if (o is JObject)
			{
				var jo = (JObject)o;
				foreach (var x in jo)
				{
					// Path
					string currentPath = path;
					if (!string.IsNullOrEmpty(currentPath))
						currentPath += "/";
					currentPath += x.Key;

					// Can be numeric or eg. JObject value
					Parse(logger, col, currentPath, x.Value);
				}
				return;
			}

			var oStr = o.ToString();

			// If this is regular numeric/decimal text
			if (decimal.TryParse(oStr, out value))
			{
				// Yes
				//_Logger.WriteLine(5, $"Regular value: {payload} => {value}");
				col.Add(path, value);
				return;
			}

			if (TryToParseKnownStrings(oStr, out value))
			{
				col.Add(path, value);
				return;
			}

			try
			{
				var result = JsonConvert.DeserializeObject(oStr);
				if(result is Newtonsoft.Json.Linq.JObject)// && jObject != null)
				{
					Parse(logger, col, path, result);
				}
				return;
			}
			catch(Exception ex)
			{

			}

			if (DO_REGEXP == true)
			{
				// No, trying regexp
				var valueStr = Regex.Replace(oStr, "[^0-9.]", "");
				if (decimal.TryParse(valueStr, out value))
				{
					// yes
					logger.WriteLine(5, $"Regex {oStr} => {valueStr}");
					col.Add(path, value);
					return;
				}
			}
		}

		static bool TryToParseKnownStrings(string s, out decimal result)
		{
			var retVal = false;
			result = -1;

			if (string.IsNullOrEmpty(s))
				return retVal;

			if(KnownStrings.ContainsKey(s.ToLower()))
			{
				result = KnownStrings[s.ToLower()];
				retVal = true;
			}

			return retVal;
		}
	}
}
