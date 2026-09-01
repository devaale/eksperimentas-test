using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Experiment.Data.Metadata;
using Newtonsoft.Json;

namespace Experiment.Data.Models{
    public class DatapointValue : IDatapointValue
	{
		#region Const
		public const string SERIES_NAME_PROPERTY = nameof(DatapointName);
		public const string DISPLAY_MEMBER_PROPERTY = nameof(Date);
		public const string VALUE_MEMBER_PROPERTY = nameof(Value);

		#endregion

		#region Attributes
		string _DatapointName;

		#endregion

		#region Properties
		public int Id { get; set; }
		public int DatapointId { get; set; }
		public DateTime Date { get; set; }
		public decimal Value { get; set; }

		/// <summary>
		/// Additive just for charts population.
		/// As we have comparison by years of the same datapoints.
		/// This helps to identify of what data part specific record is.
		/// </summary>
		public int Year { get; set; }

		/// <summary>
		/// Parent
		/// </summary>
		[JsonIgnore]
		public Datapoint Datapoint { get; set; }

		/**
		 * Following properties are not from back-end
		 */

		/// <summary>
		/// Addon of 
		/// </summary>
		[JsonIgnore]
		public string DatapointName
		{
			get
			{
				
				if(Datapoint != null)
				{
					return Datapoint.Name;
				}
				else if (!string.IsNullOrEmpty(_DatapointName))
				{
					return _DatapointName;
				}
				return string.Empty;
			}
			set => _DatapointName = value;
		}

		//public double ValueDbl { get => Convert.ToDouble(Value); }

		#endregion
	}
}