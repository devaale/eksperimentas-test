// Using SeriesAdapter instead of SeriesTemplateAdapter
// based on: https://docs.devexpress.com/MobileControls/400411/xamarin-forms/charts/getting-started/cartesian-chart-lesson
// Has issues with datapoint name recognition, in result shows single serie of all
// Possibly because it needs List<List<DatapointValue>> or separate collection for each serie, that it worked
// Like in example above, in link
//#define VER200
#define DXC_CHART_HINT	// Adds on tap hints and lines. This precompiler definition was added only to show related options for developer.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

using Newtonsoft.Json;
using DXC = DevExpress.Maui.Charts;

using Experiment.Core.Base;
using Experiment.Core.Enums;
using Experiment.Core.Enums;

// MVVM
using Experiment.Data.Enums;
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Metadata;
using Experiment.Maui.Services;
using Experiment.Maui.UI.Base;
using DevExpress.Maui.Charts;

namespace Experiment.Maui.UI.Controls{
	/// <summary>
	/// Our chart class-wrapper
	/// </summary>
	public class ChartControl : ChartControlBasis<object>
	{
		#region Const
		const string TYPE_NAME = nameof(ChartControl);
		const bool DEBUG = true;

		const int SERIES_MARKER_SIZE = 3;
		const int SERIES_MARKER_STROKE_TICKNESS = 1;

		#endregion

		#region Attributes
		protected DXC.ChartBaseView _ChartControl;

		#endregion

		#region Properties
		public bool IsPieChart { get => _ChartControl is DXC.PieChartView; }
		public bool IsXYChart { get => _ChartControl is DXC.ChartView; }

		protected DataTemplate Template
		{
			get
			{
				var vLoc = string.Format("{0}::{1}[GET]", TYPE_NAME, nameof(Template));

				DXC.XYSeries series = null;
				switch (ChartType)
				{
					default:
					case ChartType.Points:
						var pSer = new DXC.PointSeries();
						//pSer.Style = new DXC.PointSeriesStyle
						//{
						//	MarkerSize = SERIES_MARKER_SIZE,
						//};
						series = pSer;
						break;

					case ChartType.Line:
						var lSer = new DXC.LineSeries()
						{ 
							MarkersVisible = true,
						};
						lSer.Style = new DXC.LineSeriesStyle()
						{
							//Stroke = Color.Black,
							StrokeThickness = SERIES_MARKER_STROKE_TICKNESS,
							MarkerSize = SERIES_MARKER_SIZE,
							MarkerStyle = new DXC.MarkerStyle()
							{
								Fill = Colors.Gray,
							},
						};
						series = lSer;
						break;

					case ChartType.Area:
						var aSer = new DXC.AreaSeries()
						{
							MarkersVisible = true,
						};
						aSer.Style = new DXC.AreaSeriesStyle()
						{
							//Stroke = Color.Black,
							StrokeThickness = SERIES_MARKER_STROKE_TICKNESS,
							MarkerSize = SERIES_MARKER_SIZE,
							MarkerStyle = new DXC.MarkerStyle()
							{
								Fill = Colors.Gray,
							},
						};
						series = aSer;
						break;

					case ChartType.Bar:
						series = new DXC.BarSeries();
						break;

					//case ChartType.Pie:
					//	series = new DXC.PieSeries();
					//	break;

					//case ChartType.Donut:
					//	series = new DXC.DonutSeries();
					//	break;
				}

				//series.SetBinding(DXC.SeriesBase.DisplayNameProperty, new Binding("SeriesDataMemberValue"));	// Commented before 2023-07-04
#warning @TODO: We have chart series name issue, to which might be this related
				series.SetBinding(DXC.SeriesBase.DisplayNameProperty, new Binding(nameof(DXC.SeriesTemplateData.SeriesDataMemberValue))); // Worked to 2023-07-04 and probably after

#if DXC_CHART_HINT
				series.HintOptions = new DXC.SeriesCrosshairOptions()
				{
					PointTextPattern = "{S}: {V}",
				};
#endif
				Debug.WriteLineIf(DEBUG, string.Format("{0}, {1}", vLoc, JsonConvert.SerializeObject(series)));

				return new DataTemplate(() => series);
			}
		}
		#endregion

		#region Events
		protected override void OnChartTypeChange(ChartType oldValue, ChartType newValue)
		{
			base.OnChartTypeChange(oldValue, newValue);
			UpdateChartType(oldValue, newValue);
		}

		protected override void OnChartSeriesChange(object oldValue, object newValue)
		{
			base.OnChartSeriesChange(oldValue, newValue);
			if (IsXYChart)
				UpdateXYChartSeries();
		}

		protected override void OnChartTitleChange(string oldValue, string newValue)
		{
			base.OnChartTitleChange(oldValue, newValue);
			UpdateChartTitle();
		}

		protected override void OnChartSeriesNameChange(string oldValue, string newValue)
		{
			base.OnChartSeriesNameChange(oldValue, newValue);
			UpdateChartSeriesName(oldValue, newValue);
		}

		protected override void OnChartSeriesDisplayMemberChange(string oldValue, string newValue)
		{
			base.OnChartSeriesDisplayMemberChange(oldValue, newValue);
			UpdateChartSeriesDisplayMember(oldValue, newValue);
		}

		protected override void OnChartSeriesValueMemberChange(string oldValue, string newValue)
		{
			base.OnChartSeriesValueMemberChange(oldValue, newValue);
			UpdateChartSeriesValueMember(oldValue, newValue);
		}

		protected override void OnChartMeasureUnitChange(DatePartOrInterval oldValue, DatePartOrInterval newValue)
		{
			base.OnChartMeasureUnitChange(oldValue, newValue);
			UpdateChartMeasureUnit(oldValue, newValue);
		}

		protected override void OnChartAggregationTypeChange(ChartAggregationType oldValue, ChartAggregationType newValue)
		{
			base.OnChartAggregationTypeChange(oldValue, newValue);
			UpdateChartAggregationType(oldValue, newValue);
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Updates chart type
		/// 
		/// Char type update or change may reinitialize whole chart
		/// </summary>
		protected void UpdateChartType(ChartType oldValue, ChartType newValue)
		{
			var vLoc = string.Format("{0}::{1}(ChartType oldValue={2}, ChartType newValue={3})",
				TYPE_NAME, nameof(UpdateChartType), oldValue, newValue);
			Debug.WriteLineIf(DEBUG, vLoc);

			// If this is new chart type, purge previous chart, if exists
			if (oldValue != newValue)
			{
				if (_ChartControl != null)
				{
					Children.Remove(_ChartControl);
					_ChartControl = null;
				}

				// Now checking chart type
				switch (ChartType)
				{
					default:
					case ChartType.Points:
					case ChartType.Line:
					case ChartType.Area:
					case ChartType.Bar:
						// Regular XYChart
						_ChartControl = CreateXYChart();
						break;

					case ChartType.Donut:
					case ChartType.Pie:
						// Round Chart
						_ChartControl = CreatePieChart();
						break;

						// Possible NONE chart specified
						//default:
						//	break;
				}

				if (_ChartControl != null)
					Children.Add(_ChartControl);
			}

			if (IsXYChart)
			{
				UpdateXYChartSeries();
				UpdateChartTitle();
			}
			else if (IsPieChart)    // Double check as this is type check, which will be false in case of wrong type or NULL
			{
				UpdatePieChartSeries();
				UpdateChartTitle();
			}
		}

		/// <summary>
		/// Creates and initialized regular XY Chart
		/// </summary>
		/// <returns></returns>
		protected DXC.ChartView CreateXYChart()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(CreateXYChart));
			Debug.WriteLineIf(DEBUG, vLoc);

			// Creating
			DXC.ChartView c = new DXC.ChartView();

			// Styling


			// AxisY Cospetics
			//	<dxc:ChartView.AxisY>
			//		<dxc:NumericAxisY AlwaysShowZeroLevel = "False">
			//			<dxc:NumericAxisY.Style>
			//				<dxc:AxisStyle MajorTickmarksVisible = "True" MinorTickmarksVisible = "True"/>
			//			</ dxc:NumericAxisY.Style>
			//			<dxc:NumericAxisY.Title>
			//				<dxc:AxisTitle Text = "{Binding Title}"/>
			//			</ dxc:NumericAxisY.Title>
			//		</ dxc:NumericAxisY>
			//	</ dxc:ChartView.AxisY>
			c.AxisY = new DXC.NumericAxisY()
			{
				AlwaysShowZeroLevel = false,
				Style = new DXC.AxisStyle()
				{
					MajorTickmarksVisible = DXC.DefaultBoolean.True,
					MinorTickmarksVisible = DXC.DefaultBoolean.True,
				},
				Title = new DXC.AxisTitle()
				{
					Style = new DXC.TitleStyle()
					{
						TextStyle = new DXC.TextStyle()
						{
							Size = 16,
						},
					}
				},
				//#if VER200
				Label = new DXC.AxisLabel()
				{
					TextFormat = "#.#",
					Position = DXC.AxisLabelPosition.Inside,
				},
				//#endif
			};

			// Set minutes chart X axis precision
			// <dxc:ChartView.AxisX>
			//	<dxc:DateTimeAxisX MeasureUnit = "Minute" />
			// </dxc:ChartView.AxisX>
			c.AxisX = new DXC.DateTimeAxisX()
			{
				//LabelTextFormatter = new ChartAxisLabelTextFormatter(),
				MeasureUnit = Utils.ToDxcDateTimeMeasureUnit(ChartMeasureUnit),
				GridAlignment = Utils.ToDxcDateTimeMeasureUnit(ChartMeasureUnit),
				GridSpacing = 1,
				//AggregationType = DXC.AggregationType.Average,
			};

			// Legend - responsible for showng labels of series
			c.Legend = new DXC.Legend()
			{
				VerticalPosition = DXC.LegendVerticalPosition.TopOutside,
				HorizontalPosition = DXC.LegendHorizontalPosition.Center,
				Orientation = DXC.LegendOrientation.LeftToRight,
			};

#if DXC_CHART_HINT
			// Example: https://docs.devexpress.com/MobileControls/DevExpress.Maui.Charts.CrosshairHintBehavior.GroupHeaderTextPattern
			c.Hint = new DXC.Hint()
			{
				Enabled = true,
				ShowMode = DXC.HintShowMode.OnLongPress,
				Behavior = new DXC.CrosshairHintBehavior()
				{
					GroupHeaderVisible = true,
					GroupHeaderTextPattern = "{A$YYYY-mm-dd HH:mm}",
					ArgumentLabelVisible = true,
					ArgumentLineVisible = true,
					ValueLabelVisible = true,
					ValueLineVisible = true,
					HighlightPoint = true,
					MaxSeriesCount = 3,
				},
			};
#endif
			// https://docs.devexpress.com/MobileControls/400099/android/cartesian-chart/overview?v=19.1
			c.AxisXNavigationMode =  DXC.AxisNavigationMode.ScrollingAndZooming;
			//c.AxisYNavigationMode = DXC.AxisNavigationMode.None;


			return c;
		}

		/// <summary>
		/// Creates and initialized pie chart
		/// 
		/// Developed according to sample: 
		/// https://docs.devexpress.com/MobileControls/400410/xamarin-forms/charts/getting-started/pie-chart-lesson
		/// </summary>
		/// <returns></returns>
		protected DXC.PieChartView CreatePieChart()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(CreatePieChart));
			Debug.WriteLineIf(DEBUG, vLoc);

			DXC.PieChartView c = new DXC.PieChartView();

			// Add a Legend to the Pie Chart
			c.Legend = new DXC.Legend()
			{
				VerticalPosition = DXC.LegendVerticalPosition.Center,
				HorizontalPosition = DXC.LegendHorizontalPosition.RightOutside,
				Orientation = DXC.LegendOrientation.TopToBottom,
			};

			// Enable the Pie Chart Tooltips
			c.Hint = new DXC.PieHint()
			{
				Enabled = true,
			};

			return c;
		}

		/// <summary>
		/// Updates chart series data
		/// </summary>
		protected void UpdateXYChartSeries()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(UpdateXYChartSeries));
			Debug.WriteLineIf(DEBUG, vLoc);

			// If there's no chart, nowhere to apply series
			if (_ChartControl == null)
				return;

			var c = (DXC.ChartView)_ChartControl;

#if VER200
			// <dxc:LineSeries DisplayName="{Binding GdpValueForUSA.CountryName}">
			//		<dxc:LineSeries.Data>
			//			<dxc:SeriesDataAdapter DataSource = "{Binding GdpValueForUSA.Values}" ArgumentDataMember = "Year">
			//				<dxc:ValueDataMember Type = "Value" Member = "Value" />
			//			</dxc:SeriesDataAdapter>
			//		</dxc:LineSeries.Data>
			// </dxc:LineSeries>

			DXC.XYSeries series = null;
			switch (ChartType)
			{
				default:
				case ChartType.Points:
					series = new DXC.PointSeries();
					break;

				case ChartType.Line:
					series = new DXC.LineSeries();
					break;

				case ChartType.Area:
					series = new DXC.AreaSeries();
					break;

				case ChartType.Bar:
					series = new DXC.BarSeries();
					break;
			}

			if (series != null)
			{
				// Datapoint name (NOT WORKING)
				// Possibly that datasource should be given as List<List<DatapointValue>>
				// instead of List<DatapointValue> for each serie different correspondingly
				// Related to sample: https://docs.devexpress.com/MobileControls/400411/xamarin-forms/charts/getting-started/cartesian-chart-lesson
				series.SetBinding(DXC.Series.DisplayNameProperty, new Binding(ChartSeriesName));

				var sda = new DXC.SeriesDataAdapter()
				{
					DataSource = ChartSeries,
					ArgumentDataMember = ChartSeriesDisplayMember,
				};
				sda.ValueDataMembers.Add(new DXC.ValueDataMember() { Type = DXC.ValueType.Value, Member = ChartSeriesValueMember });
				series.Data = sda;

				c.Series.Clear();
				c.Series.Add(series);
			}

#else
			var sta = new DXC.SeriesTemplateAdapter()
			{
				DataSource = ChartSeries,
				SeriesTemplate = Template,
				SeriesDataMember = ChartSeriesName,
				ArgumentDataMember = ChartSeriesDisplayMember,
			};

			// <dxc:SeriesTemplateAdapter.ValueDataMembers>
			//		< dxc:ValueDataMember Type = "Value" Member = "Value" />
			// </ dxc:SeriesTemplateAdapter.ValueDataMembers >
			sta.ValueDataMembers.Add(new DXC.ValueDataMember()
			{
				Type = DXC.ValueType.Value,
				Member = ChartSeriesValueMember
			});

			// Assign data template to XY type Chart (line, points, area and so on)
			c.SeriesDataTemplate = sta;
#endif
			//UpdateChartSeriesName(null, null);			// Initialized in => SeriesDataMember = ChartSeriesName,
			//UpdateChartSeriesDisplayMember(null, null);	// Initialized in => ArgumentDataMember = ChartSeriesDisplayMember
			//UpdateChartSeriesValueMember(null, null);		// initialized in => sta.ValueDataMembers.Add(new DXC.ValueDataMember() { Type = DXC.ValueType.Value, Member = ChartSeriesValueMember });
		}

		protected void UpdatePieChartSeries()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(UpdatePieChartSeries));
			Debug.WriteLineIf(DEBUG, vLoc);

			var c = (DXC.PieChartView)_ChartControl;
			DXC.PieSeries series;

			switch (ChartType)
			{
				default:
				case ChartType.Pie:
					series = new DXC.PieSeries();
					break;

				case ChartType.Donut:
					series = new DXC.DonutSeries();
					break;
			}

			series.Data = new DXC.PieSeriesDataAdapter()
			{
				//DataSource = ChartSeries,
				DataSource = new List<M.DatapointValue>()
				{
					new M.DatapointValue()
					{
						Datapoint = new M.Datapoint() { Name = "Alfa" },
						Value = 100,
						Date = DateTime.Now,
					},

					new M.DatapointValue()
					{
						Datapoint = new M.Datapoint() { Name = "Beta" },
						Value = 150,
						Date = DateTime.Now,
					},

					new M.DatapointValue()
					{
						Datapoint = new M.Datapoint() { Name = "Zeta" },
						Value = 200,
						Date = DateTime.Now,
					},
				},
				LabelDataMember = ChartSeriesName,
				ValueDataMember = ChartSeriesValueMember,
			};

			c.Series.Add(series);
		}

		/// <summary>
		/// Updates chart title
		/// 
		/// UpdateChartTitle will be triggered every time when title will be changed.
		/// </summary>
		protected void UpdateChartTitle()
		{
			var vLoc = string.Format("{0}::{1}()",TYPE_NAME, nameof(UpdateChartTitle));
			Debug.WriteLineIf(DEBUG, vLoc);

			// If we have no chart, no action
			if (_ChartControl == null)
				return;

			if (IsXYChart)
			{
				var cv = (DXC.ChartView)_ChartControl;
				if (cv.AxisY != null)
				{
					if (cv.AxisY.Title == null)
					{
						cv.AxisY.Title = new DXC.AxisTitle();
					}
					cv.AxisY.Title.Text = ChartTitle;
				}

				// <dxc:NumericAxisY.Title>
				// < dxc:AxisTitle Text = "{Binding Title}" />
				// </ dxc:NumericAxisY.Title >
				//var aTitle = new DXC.AxisTitle();
				//aTitle.SetBinding(DXC.AxisTitle.TextProperty, new Binding(ChartTitle));
				//cv.AxisY.Title = aTitle;
			}
		} // UpdateChartTitle


		/// <summary>
		/// Updates chart series display member (eg. DatapointName)
		/// </summary>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		protected void UpdateChartSeriesName(string oldValue, string newValue)
		{
			var vLoc = string.Format("{0}::{1}(string oldValue={2}, string newValue={3})",
				TYPE_NAME, nameof(UpdateChartSeriesName), oldValue, newValue);
			Debug.WriteLineIf(DEBUG, vLoc);

			if (_ChartControl == null)
				return;

			if (IsXYChart)
			{
				var c = (DXC.ChartView)_ChartControl;
#if VER200
				if (c.Series.Count > 0)
				{
					var series = (DXC.XYSeries)c.Series[0];
					// @TODO: VER200, This part not working, not assigning somehow datapoint name
					if (string.IsNullOrEmpty(ChartSeriesName))
					{
						series.RemoveBinding(DXC.SeriesBase.DisplayNameProperty);
					}
					else
					{
						series.SetBinding(DXC.SeriesBase.DisplayNameProperty, new Binding(ChartSeriesName));
					}
					//series.DisplayName = ChartSeriesName;
				}

#else
				if (string.IsNullOrEmpty(ChartSeriesName))
				{
					if (!string.IsNullOrEmpty(c.SeriesDataTemplate.SeriesDataMember))
					{
						c.SeriesDataTemplate.SeriesDataMember = null;
					}
				}
				else
				{
					c.SeriesDataTemplate.SeriesDataMember = ChartSeriesName;
				}
#endif
			}
		} // UpdateChartSeriesName

		/// <summary>
		/// Updates chart series value member (eg. date)
		/// </summary>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		protected void UpdateChartSeriesDisplayMember(string oldValue, string newValue)
		{
			var vLoc = string.Format("{0}::{1}(string oldValue={2}, string newValue={3})",
				TYPE_NAME, nameof(UpdateChartSeriesDisplayMember), oldValue, newValue);
			Debug.WriteLineIf(DEBUG, vLoc);

			if (_ChartControl == null)
				return;

			if (IsXYChart)
			{
				var c = (DXC.ChartView)_ChartControl;
#if VER200
				if (c.Series.Count > 0)
				{
					var series = (DXC.XYSeries)c.Series[0];
					var sda = (DXC.SeriesDataAdapter)series.Data;
					sda.ArgumentDataMember = ChartSeriesDisplayMember;
				}
#else
				if (string.IsNullOrEmpty(ChartSeriesDisplayMember))
				{
					if (!string.IsNullOrEmpty(c.SeriesDataTemplate.ArgumentDataMember))
						c.SeriesDataTemplate.ArgumentDataMember = null;
				}
				else
				{
					c.SeriesDataTemplate.ArgumentDataMember = ChartSeriesDisplayMember;
				}
#endif
			}
		} // UpdateChartSeriesDisplayMember

		protected void UpdateChartSeriesValueMember(string oldValue, string newValue)
		{
			var vLoc = string.Format("{0}::{1}(string oldValue={2}, string newValue={3})",
				TYPE_NAME, nameof(UpdateChartSeriesValueMember), oldValue, newValue);
			Debug.WriteLineIf(DEBUG, vLoc);

			var c = (DXC.ChartView)_ChartControl;
#if VER200
			var sd = (DXC.XYSeries)c.Series[0];
			var sda = (DXC.SeriesDataAdapter)sd.Data;

			sda.ValueDataMembers[0].Member = ChartSeriesValueMember;
#else
			c.SeriesDataTemplate.ValueDataMembers[0].Member = ChartSeriesValueMember;
#endif
		} // UpdateChartSeriesValueMember

		protected void UpdateChartMeasureUnit(DatePartOrInterval oldValue, DatePartOrInterval newValue)
		{
			var vLoc = string.Format("{0}::{1}(DatePartOrInterval oldValue={2}, DatePartOrInterval newValue={3})",
				TYPE_NAME, nameof(UpdateChartMeasureUnit), oldValue, newValue);
			Debug.WriteLineIf(DEBUG, vLoc);

			if (IsXYChart)
			{
				var c = (DXC.ChartView)_ChartControl;
				DXC.DateTimeAxisX axisX;

				if (c.AxisX is DXC.DateTimeAxisX)
				{
					axisX = (DXC.DateTimeAxisX)c.AxisX;
					axisX.MeasureUnit = Utils.ToDxcDateTimeMeasureUnit(ChartMeasureUnit);
					axisX.GridAlignment = Utils.ToDxcDateTimeMeasureUnit(ChartMeasureUnit);
				}
				else
				{
					c.AxisX = axisX = new DXC.DateTimeAxisX()
					{
						MeasureUnit = Utils.ToDxcDateTimeMeasureUnit(ChartMeasureUnit),
						GridAlignment = Utils.ToDxcDateTimeMeasureUnit(ChartMeasureUnit),
						GridSpacing = 1,
					};
				}

				var aggregationType = Utils.ToDxcAggregationType(ChartAggregationType);
				// @see https://docs.devexpress.com/MobileControls/DevExpress.Maui.Charts.DateTimeAxisX.AggregationType
				axisX.AggregationType = Utils.ToDxcAggregationType(ChartAggregationType);
			}
		}

		protected void UpdateChartAggregationType(ChartAggregationType oldValue, ChartAggregationType newValue)
		{
			if (IsXYChart)
			{
				var c = (DXC.ChartView)_ChartControl;
				if(c.AxisX != null && c.AxisX is DXC.DateTimeAxisX)
				{
					var axis = c.AxisX as DXC.DateTimeAxisX;
					axis.AggregationType = Utils.ToDxcAggregationType(ChartAggregationType);
				}
			}
		}


		#endregion
	}
}


