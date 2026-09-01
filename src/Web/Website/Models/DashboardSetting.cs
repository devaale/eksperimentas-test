using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using Experiment.Core.Enums;
using Experiment.Data.Enums;
using D = Experiment.Data.Models;
using Experiment.Data.Metadata;

namespace Website.Models
{
	[Table("tblDashboardSetting")]
	public class DashboardSetting : IDashboardSetting
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[StringLength(128)]
		public string UserId { get; set; }

		public int? ObjectId { get; set; }

		[ForeignKey(nameof(UserId))]
		public virtual ApplicationUser User { get; set; }

		[Required]
		[DefaultValue(DateRange.Today)]
		public DateRange DateRange { get; set; }

		[Required]
		[DefaultValue(ChartType.Line)]
		public ChartType Graph1Type { get; set; }

		[Required]
		[DefaultValue(DatePartOrInterval.Hour)]
		public DatePartOrInterval Graph1Interval { get; set; }

		[Required]
		[DefaultValue(false)]
		public bool Graph1Difference { get; set; }

		[Required]
		[DefaultValue(ChartAggregationType.AverageValue)]
		public ChartAggregationType Graph1Aggregation { get; set; }

		[Required]
		[DefaultValue(ChartType.Line)]
		public ChartType Graph2Type { get; set; }

		[Required]
		[DefaultValue(DatePartOrInterval.Hour)]
		public DatePartOrInterval Graph2Interval { get; set; }

		[Required]
		[DefaultValue(false)]
		public bool Graph2Difference { get; set; }

		[Required]
		[DefaultValue(ChartAggregationType.AverageValue)]
		public ChartAggregationType Graph2Aggregation { get; set; }

		[Required]
		[DefaultValue(ChartType.Line)]
		public ChartType Graph3Type { get; set; }

		[Required]
		[DefaultValue(DatePartOrInterval.Hour)]
		public DatePartOrInterval Graph3Interval { get; set; }

		[Required]
		[DefaultValue(false)]
		public bool Graph3Difference { get; set; }

		[Required]
		[DefaultValue(ChartAggregationType.AverageValue)]
		public ChartAggregationType Graph3Aggregation { get; set; }

		[Required]
		[DefaultValue(ChartType.Line)]
		public ChartType Graph4Type { get; set; }

		[Required]
		[DefaultValue(DatePartOrInterval.Hour)]
		public DatePartOrInterval Graph4Interval { get; set; }

		[Required]
		[DefaultValue(false)]
		public bool Graph4Difference { get; set; }

		[Required]
		[DefaultValue(ChartAggregationType.AverageValue)]
		public ChartAggregationType Graph4Aggregation { get; set; }

		/// <summary>
		/// If to enable this one, you will have nasty error 
		/// Invalid column name 'DashboardSetting_Id'.
		/// 
		/// @see https://stackoverflow.com/a/20652952
		/// 
		/// As this collection should have logical tie between records,
		/// while in our case they all are based on UserId
		/// </summary>
		//public ICollection<DashboardDatapoint> Datapoints { get; set; }

		public DashboardSetting()
		{
			DateRange = DateRange.Today;

			Graph1Type = D.DashboardSetting.DEFAULT_GRAPH_TYPE;
			Graph1Interval = D.DashboardSetting.DEFAULT_GRAPH_INTERVAL;
			Graph1Difference = false;
			Graph1Aggregation = D.DashboardSetting.DEFAULT_GRAPH_AGGREGATION;

			Graph2Type = D.DashboardSetting.DEFAULT_GRAPH_TYPE;
			Graph2Interval = D.DashboardSetting.DEFAULT_GRAPH_INTERVAL;
			Graph2Difference = false;
			Graph2Aggregation = D.DashboardSetting.DEFAULT_GRAPH_AGGREGATION;

			Graph3Type = D.DashboardSetting.DEFAULT_GRAPH_TYPE;
			Graph3Interval = D.DashboardSetting.DEFAULT_GRAPH_INTERVAL;
			Graph3Difference = false;
			Graph3Aggregation = D.DashboardSetting.DEFAULT_GRAPH_AGGREGATION;

			Graph4Type = D.DashboardSetting.DEFAULT_GRAPH_TYPE;
			Graph4Interval = D.DashboardSetting.DEFAULT_GRAPH_INTERVAL;
			Graph4Difference = false;
			Graph4Aggregation = D.DashboardSetting.DEFAULT_GRAPH_AGGREGATION;
		}
	}
}
