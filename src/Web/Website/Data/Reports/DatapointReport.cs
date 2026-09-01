using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Experiment.Core;
using M = Experiment.Data.Models;

using Website.Models;

namespace Website.Data.Reports{
	public class DatapointReport
	{
		protected class DatapointReportItem
		{
			/// <summary>
			/// Datapoint itself
			/// </summary>
			public M.Datapoint Datapoint { get; protected set;  }

			public List<M.DatapointValue> Values { get; protected set; }

			public DatapointReportItem(M.Datapoint datapoint)
			{
				if (datapoint == null)
					throw new ArgumentNullException(nameof(datapoint));

				// Structure prevents modification of variables, which shouldn't be modified
				Datapoint = datapoint;
				Values = new List<M.DatapointValue>();
			}
		}

		ApplicationDbContext Db;
		IEnumerable<M.DatapointValue> Values;
		Dictionary<string, DatapointReportItem> Datapoints = new Dictionary<string, DatapointReportItem>();

		public DatapointReport(
			ApplicationDbContext context,
			IEnumerable<M.DatapointValue> values)
		{
			if(context == null)
				throw new ArgumentNullException(nameof(context));

			if(values == null)
				throw new ArgumentNullException(nameof(values));

			Db = context;
			Values = values;

			// Scanning datapoints and collecting their values
			foreach(var dv in Values)
			{
				var key = string.Format("{0}-{1}",
					dv.DatapointId,
					dv.Year);

				if (!Datapoints.ContainsKey(key))
				{
					var dp = Db.Datapoints.Find(dv.DatapointId);

					// Calculating year of data
					var year = DateTime.Now.Year - dv.Year;
					var name = string.Empty;

					if(dv.Year == 0)
					{
						name = dp.Name;
					} else if (string.IsNullOrEmpty(dp.Name))
					{
						name = year.ToString();
					}
					else
					{
						name = string.Format("{0} ({1})",
							dp.Name.Substring(0, Defaults.MAX_GRAPH_COMP_DP_NAME_LEN),
							year);
					}


					Datapoints.Add(key, new DatapointReportItem(new M.Datapoint
					{
						Id = dp.Id,
						Name = name,
					}));
				}

				Datapoints[key].Values.Add(dv);
			}

			// Now we made collections of datapoints only with their values in chronological order.
			// As DB procedure ordered everything by date, but not datapoint and date.
		}

		public IEnumerable<string> GetLines()
		{
			var go = true;
			string lineStr = string.Empty;

			// First CSV headers
			foreach (var dp in Datapoints.Values)
			{
				// Single datapoint takes two columns
				// To keep consistency of structure, need to fill them with values or corresponding amount of tabs
				lineStr += dp.Datapoint.Name + "\t\t";
			}
			yield return lineStr;

			// While we need confirmed go
			var line = 0;
			while (go)
			{
				lineStr = string.Empty;

				foreach (var dri in Datapoints.Values)
				{
					// If some date/value already was added
					if (lineStr.Length > 0)
						lineStr += "\t";

					// If it has data for specific line
					if (dri.Values.Count > line)
					{
						// Adding it
						lineStr += string.Format("{0}\t{1}",
							dri.Values[line].Date.ToString(Defaults.DEFAULT_DATETIME_FORMAT),
							dri.Values[line].Value);
					}
					else
					{
						// If no then only empty tab
						// As only one Tab separator added in case of Date/tValue (see above)
						lineStr += "\t";
					}
				}

				yield return lineStr;

				// The next line
				line++;
				// Go if at least some datapoint still has so many lines
				go = Datapoints.Values.Any(dp => dp.Values.Count > line);
			}

		}
	}
}