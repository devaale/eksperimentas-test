#define DXC_CHART_HINT  // Adds on tap hints and lines. This precompiler definition was added only to show related options for developer.                                                                                                                   
//#define VER200
//#define VER300
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

// MVVM
using Experiment.Data.Enums;
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Metadata;
using Experiment.Maui.Services;
using Experiment.Maui.UI.Base;
using Experiment.Maui.Models;
using Experiment.Data.Models;
//using Microcharts;
using DevExpress.Maui.Charts;

namespace Experiment.Maui.UI.Controls{
	public class GenericChartControl : Grid
	{
		#region Const
		const string TYPE_NAME = nameof(GenericChartControl);
		const bool DEBUG = true;

		const int SERIES_MARKER_SIZE = 3;
		const int SERIES_MARKER_STROKE_TICKNESS = 1;

		#endregion

		#region Attributes
		protected DXC.ChartBaseView _Chart;

		#endregion

		#region Properties
		/// <summary>
		/// ChartControl where it should be assigned.
		/// In order to purge it, set null value.
		/// </summary>
		public DXC.ChartBaseView Chart
		{
			get => _Chart;
			set
			{
				// Only if something changed
				if (_Chart != value)
				{
					// If _ChartControl Not Null
					if (_Chart != null)
					{
						// Remove it
						Children.Remove(_Chart);

						if (_Chart is IDisposable)
						{
							((IDisposable)_Chart).Dispose();
						}
					}

					// Assign new _ChartControl
					_Chart = value;

					// If new _ChartControl is Not null
					if (_Chart != null)
					{
						// Add it to children
						Children.Add(_Chart);
					}
				}
			}
		}

		/// <summary>
		/// Is chart initialized at all or not equal NULL
		/// </summary>
		public bool IsChart { get => Chart != null; }

		/// <summary>
		/// Is this is Pie/Donut type chart
		/// </summary>
		public bool IsPieChart { get => IsChart && Chart is DXC.PieChartView; }

		/// <summary>
		/// Is this is X or Y chart type
		/// </summary>
		public bool IsXYChart { get => IsChart && Chart is DXC.ChartView; }

		#region Bindable

		/// <summary>
		/// Bindable ChartParametersProperty
		/// </summary>
		public static readonly BindableProperty ChartParametersProperty =
			BindableProperty.Create(nameof(ChartParameters), typeof(GenericChartParameters), typeof(GenericChartControl), null, propertyChanged: OnChartParametersChanged);
		/// <summary>
		/// ChartType
		/// </summary>
		public GenericChartParameters ChartParameters
		{
			get => (GenericChartParameters)GetValue(ChartParametersProperty);
			set => SetValue(ChartParametersProperty, value);
		}
		protected static void OnChartParametersChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var me = (GenericChartControl)bindable;
			me.OnChartParametersChange((GenericChartParameters)oldValue, (GenericChartParameters)newValue);
		}

		#endregion

		#endregion

		#region Ctor
		public GenericChartControl()
		{

		}

		#endregion

		#region Helpers
		protected virtual void OnChartParametersChange(GenericChartParameters oldVal, GenericChartParameters newVal)
		{
			var vLoc = string.Format("{0}::{1}({2} oldVal={3}, {2} newVal={4})",
				TYPE_NAME, nameof(OnChartParametersChange), nameof(GenericChartParameters), oldVal, newVal);

			if (newVal != null)
			{
				var initRetVal = newVal.Init(this);
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
			DXC.ChartView c = new DXC.ChartView()
			{
				BackgroundColor = Color.FromArgb("#479f78"), // Not working
			};

			// Styling
			UpdateAxisX(c);

			// AxisY Cosmetics
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
				//Title = new DXC.AxisTitle()
				//{
				//	Style = new DXC.TitleStyle()
				//	{
				//		TextStyle = new DXC.TextStyle()
				//		{
				//			Size = 16,
				//		},
				//	}
				//},
				//#if VER200
				Label = new DXC.AxisLabel()
				{
					TextFormat = "#.#",
					Position = DXC.AxisLabelPosition.Inside,
				},
				//#endif
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
			return c;
		}

		/// <summary>
		/// Update /re-create x axis
		/// </summary>
		public void UpdateAxisX(DXC.ChartView chart)
		{
			if (chart == null)
				return;

			//// Set minutes chart X axis precision
			//// <dxc:ChartView.AxisX>
			////	<dxc:DateTimeAxisX MeasureUnit = "Minute" />
			//// </dxc:ChartView.AxisX>
			//chart.AxisX = new DXC.DateTimeAxisX()
			//{
			//	//LabelTextFormatter = new ChartAxisLabelTextFormatter(),
			//	MeasureUnit = Utils.ToDxcDateTimeMeasureUnit(ChartParameters.MeasureUnit),
			//	GridAlignment = Utils.ToDxcDateTimeMeasureUnit(ChartParameters.MeasureUnit),
			//	GridSpacing = 1,
			//	AggregationType = aggregationType.Value,
			//};

			var mu = Utils.ToDxcDateTimeMeasureUnit(ChartParameters.Interval);
			var at = Utils.ToDxcAggregationType(ChartParameters.AggregationType);

			// By default it chart.AxisX is NULL
			// Create DateTimeAxisX
			if (chart.AxisX == null)
			{
				chart.AxisX = new DXC.DateTimeAxisX()
				{
					MeasureUnit = mu,
					GridAlignment = mu,
					GridSpacing = 1,    // This never changes so far?
					AggregationType = at,
				};
			}
			else
			{
				//chart.AxisX.LabelTextFormatter = new ChartAxisLabelTextFormatter();	// This one was commented in original version

				if (chart.AxisX is DXC.DateTimeAxisX)
				{
					var x = chart.AxisX as DXC.DateTimeAxisX;

					if (x.MeasureUnit != mu)
						x.MeasureUnit = mu;

					if (x.GridAlignment != mu)
						x.GridAlignment = mu;

					if (x.AggregationType != at)
						x.AggregationType = at;

					// This was implemented like this, to avoid triggering of any redundant funtionality via setters
				}
			}

			// https://docs.devexpress.com/MobileControls/400099/android/cartesian-chart/overview?v=19.1
			if (chart.AxisXNavigationMode != DXC.AxisNavigationMode.ScrollingAndZooming)
			{
				chart.AxisXNavigationMode = DXC.AxisNavigationMode.ScrollingAndZooming;
				//c.AxisYNavigationMode = DXC.AxisNavigationMode.None;
			}
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
		/// Uses only XY Chart
		/// </summary>
		/// <returns></returns>
		protected DataTemplate CreateTemplate()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(CreateTemplate));

			DXC.XYSeries series = null;
			switch (ChartParameters.GraphType)
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

#warning @TODO: Suspicious binding
			//series.SetBinding(DXC.SeriesBase.DisplayNameProperty, new Binding("SeriesDataMemberValue"));
			series.SetBinding(DXC.SeriesBase.DisplayNameProperty,
				new Binding(nameof(DXC.SeriesTemplateData.SeriesDataMemberValue)));

#if DXC_CHART_HINT
			series.HintOptions = new DXC.SeriesCrosshairOptions()
			{
				PointTextPattern = "{S}: {V}",
			};
#endif
			Debug.WriteLineIf(DEBUG, string.Format("{0}, {1}", vLoc, JsonConvert.SerializeObject(series)));

			return new DataTemplate(() => series);
		}

		/// <summary>
		/// Updates chart series data
		/// </summary>
		protected void UpdateXYChartSeries()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(UpdateXYChartSeries));
			Debug.WriteLineIf(DEBUG, vLoc);

			// If there's no chart, nowhere to apply series
			//if (!IsChart)
			if (!IsXYChart)
				return;

			var c = (DXC.ChartView)Chart;

			// This was used long time ago
#if VER200
			// <dxc:LineSeries DisplayName="{Binding GdpValueForUSA.CountryName}">
			//		<dxc:LineSeries.Data>
			//			<dxc:SeriesDataAdapter DataSource = "{Binding GdpValueForUSA.Values}" ArgumentDataMember = "Year">
			//				<dxc:ValueDataMember Type = "Value" Member = "Value" />
			//			</dxc:SeriesDataAdapter>
			//		</dxc:LineSeries.Data>
			// </dxc:LineSeries>

			DXC.XYSeries series = null;
			switch (ChartParameters.ChartType)
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
				//series.SetBinding(DXC.Series.DisplayNameProperty, new Binding(ChartParameters.SeriesDataMember));
				series.SetBinding(DXC.Series.DisplayNameProperty, ChartParameters.SeriesDataMember);

				var sda = new DXC.SeriesDataAdapter()
				{
					DataSource = ChartParameters.DataSource,
					ArgumentDataMember = ChartParameters.ArgumentDataMember,
				};
				sda.ValueDataMembers.Add(new DXC.ValueDataMember() { Type = DXC.ValueType.Value, Member = ChartParameters.ValueMember });
				series.Data = sda;

				c.Series.Clear();
				c.Series.Add(series);
			}

#endif
			// This was used to 2023-07
#if VER300
			var sta = new DXC.SeriesTemplateAdapter()
			{
				//DataSource = ChartSeries,
				DataSource = ChartParameters.DataSource,
				SeriesTemplate = CreateTemplate(),
				//SeriesDataMember = ChartSeriesName,
				SeriesDataMember = ChartParameters.SeriesDataMember,
				//ArgumentDataMember = ChartSeriesDisplayMember,
				ArgumentDataMember = ChartParameters.ArgumentDataMember,
			};

			// <dxc:SeriesTemplateAdapter.ValueDataMembers>
			//		< dxc:ValueDataMember Type = "Value" Member = "Value" />
			// </ dxc:SeriesTemplateAdapter.ValueDataMembers >
			sta.ValueDataMembers.Add(new DXC.ValueDataMember()
			{
				Type = DXC.ValueType.Value,
				//Member = ChartSeriesValueMember,
				Member = ChartParameters.ValueMember,
			});

			// Assign data template to XY type Chart (line, points, area and so on)
			c.SeriesDataTemplate = sta;
#endif

#if !VER200 && !VER300
			// This variant requires Datapoints with initialized Values collections
			c.Series.Clear();
			if (ChartParameters.DataSource != null)
			{
				foreach (var dp in ChartParameters.DataSource)
				{
					XYSeries serie = null;
					switch (ChartParameters.GraphType)
					{
						default:
						case ChartType.Points:
							serie = new DXC.PointSeries();
							break;

						case ChartType.Line:
							serie = new DXC.LineSeries();
							break;

						case ChartType.Area:
							serie = new DXC.AreaSeries();
							break;

						case ChartType.Bar:
							serie = new DXC.BarSeries();
							break;
					}

					// Series or Datapoint name, instead of DatapointValue.SERIES_NAME_PROPERTY
					serie.DisplayName = dp.Name;

					var sda = new SeriesDataAdapter()
					{
						// ArgumentDataMember eg. Date / DatapointValue.DISPLAY_MEMBER_PROPERTY
						ArgumentDataMember = ChartParameters.ArgumentDataMember,
						DataSource = dp.Values,
					};

					sda.ValueDataMembers.Add(new DXC.ValueDataMember()
					{
						Type = DXC.ValueType.Value,
						// ValueMember eg. Value / DatapointValue.VALUE_MEMBER_PROPERTY
						Member = ChartParameters.ValueMember,
					});

					serie.Data = sda;
					c.Series.Add(serie);

				} // foreach (var dp in ChartParameters.DataSource)

			} // if (ChartParameters.DataSource != null)
#endif
		}

		/// <summary>
		/// Unused
		/// </summary>
		protected void UpdatePieChartSeries()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(UpdatePieChartSeries));
			Debug.WriteLineIf(DEBUG, vLoc);

			var c = (DXC.PieChartView)Chart;
			DXC.PieSeries series;

			switch (ChartParameters.GraphType)
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
				LabelDataMember = ChartParameters.SeriesDataMember,
				//ValueDataMember = ChartSeriesValueMember,
				ValueDataMember = ChartParameters.ValueMember,
			};

			c.Series.Add(series);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Updates chart type
		/// 
		/// Char type update or change may reinitialize whole chart
		/// </summary>
		public bool UpdateChartType()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(UpdateChartType));
			Debug.WriteLineIf(DEBUG, vLoc);

			// Now checking chart type
			switch (ChartParameters.GraphType)
			{
				default:
					break;

				case ChartType.Points:
				case ChartType.Line:
				case ChartType.Area:
				case ChartType.Bar:
					// Regular XYChart
					Chart = CreateXYChart();
					break;

				case ChartType.Donut:
				case ChartType.Pie:
					// Round Chart
					Chart = CreatePieChart();
					break;

					// Possible NONE chart specified
					//default:
					//	break;
			}

			if (IsChart)
			{
				UpdateSeries();
				UpdateTitle();
			}

			return IsChart;
		}

		/// <summary>
		/// Update chart series of any type (entry point)
		/// </summary>
		/// <returns></returns>
		public bool UpdateSeries()
		{
			if (IsXYChart)
			{
				UpdateXYChartSeries();
				return true;
			}
			else if (IsPieChart)    // Double check as this is type check, which will be false in case of wrong type or NULL
			{
				UpdatePieChartSeries();
				return true;
			}

			return false;
		}

		public void UpdateAxisX()
		{
			if (IsXYChart)
			{
				UpdateAxisX((DXC.ChartView)Chart);
			}
		}

		/// <summary>
		/// Updates chart legend visibility.
		/// </summary>
		public void UpdateLegendVisibility()
		{
			if (!IsChart || ChartParameters == null)
				return;

			if (ChartParameters.LegendVisible)
			{
				if (Chart.Legend == null)
				{
					if (IsXYChart)
					{
						((DXC.ChartView)Chart).Legend = new DXC.Legend()
						{
							VerticalPosition = DXC.LegendVerticalPosition.TopOutside,
							HorizontalPosition = DXC.LegendHorizontalPosition.Center,
							Orientation = DXC.LegendOrientation.LeftToRight,
						};
					}
					else if (IsPieChart)
					{
						((DXC.PieChartView)Chart).Legend = new DXC.Legend()
						{
							VerticalPosition = DXC.LegendVerticalPosition.Center,
							HorizontalPosition = DXC.LegendHorizontalPosition.RightOutside,
							Orientation = DXC.LegendOrientation.TopToBottom,
						};
					}
				}
			}
			else
			{
				Chart.Legend = null;
			}
		}

		/// <summary>
		/// Updates chart title
		/// 
		/// UpdateChartTitle will be triggered every time when title will be changed.
		/// </summary>
		public void UpdateTitle()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(UpdateTitle));
			Debug.WriteLineIf(DEBUG, vLoc);

			if (IsXYChart)
			{
				var c = (DXC.ChartView)Chart;
				// AxisY created in CreateXYChart()
				// We don't do anything with AxisY itself
				if (c.AxisY != null)
				{
					// Is title parameter is empty?
					if(string.IsNullOrEmpty(ChartParameters.Title))
					{
						// Yes, but title initialized
						if(c.AxisY.Title != null)
						{
							// Cleaning it
							c.AxisY.Title = null;
						}
					}
					else 
					{
						if(c.AxisY.Title == null)
						{
							c.AxisY.Title = new DXC.AxisTitle()
							{
								Style = new DXC.TitleStyle()
								{
									TextStyle = new DXC.TextStyle()
									{
										Size = 16,
									},
								}
							};
						}

						c.AxisY.Title.Text = ChartParameters.Title;
					}
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
		/// Updates chart series value member (eg. date)
		/// </summary>
		public void UpdateArgumentDataMember()
		{
			var vLoc = string.Format("{0}::{1}(string oldValue={2}, string newValue={3})",
				TYPE_NAME, nameof(UpdateArgumentDataMember));
			Debug.WriteLineIf(DEBUG, vLoc);

			if (IsXYChart)
			{
				var c = (DXC.ChartView)Chart;
#if VER200
				if (c.Series.Count > 0)
				{
					var series = (DXC.XYSeries)c.Series[0];
					var sda = (DXC.SeriesDataAdapter)series.Data;
					sda.ArgumentDataMember = ChartSeriesDisplayMember;
				}
#else
				if (c.SeriesDataTemplate != null)
				{
					c.SeriesDataTemplate.ArgumentDataMember = ChartParameters.ArgumentDataMember;
				}
#endif
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void UpdateValueMember()
		{
			var vLoc = string.Format("{0}::{1}(string oldValue={2}, string newValue={3})",
				TYPE_NAME, nameof(UpdateValueMember));
			Debug.WriteLineIf(DEBUG, vLoc);

			if (IsXYChart)
			{
				var c = (DXC.ChartView)Chart;
#if VER200
				var sd = (DXC.XYSeries)c.Series[0];
				var sda = (DXC.SeriesDataAdapter)sd.Data;

				sda.ValueDataMembers[0].Member = ChartSeriesValueMember;
#else
				if (c.SeriesDataTemplate != null)
				{
					if (c.SeriesDataTemplate.ValueDataMembers.Count > 0)
					{
						c.SeriesDataTemplate.ValueDataMembers[0].Member = ChartParameters.ValueMember;
					}
				}
#endif
			}
		}

		#endregion
	}
}


