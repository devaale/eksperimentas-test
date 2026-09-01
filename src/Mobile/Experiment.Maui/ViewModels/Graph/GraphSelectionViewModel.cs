//#define DEPRECATED
#define SET_DEFAULT_CHART_TYPE

using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Newtonsoft.Json;

using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Experiment.Core;
using Experiment.Core.Enums;
using Experiment.Core.Base;
using Experiment.Core.Data;
using Experiment.Core.Metadata;
using Experiment.Core.Ui;

// MVVM
using Experiment.Data.Enums;
using Experiment.Data.Models;
using V = Experiment.Maui.Views.Graph;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Enums;
using Experiment.Maui.Services;
using Experiment.Maui.Views;
using Experiment.Maui.Models;
using Experiment.Maui.UI.Controls;
using Experiment.Maui.Views.Test;
using Experiment.Maui.ViewModels.Test;

namespace Experiment.Maui.ViewModels.Graph{
	public class GraphSelectionViewModel : ViewModelBase
	{
		#region Const
		const string TYPE_NAME = nameof(GraphSelectionViewModel);
		const bool DEBUG = true;

		#endregion

		#region Attributes
		readonly ApiServices _ApiServices = new ApiServices();
		VisualChartSearchParams _ChartParameters;   // 2023-03-01

		PickerHandler<NamedDbItem<DateRange>> _DateRanges;
		PickerHandler<NamedDbItem<DatePartOrInterval>> _MeasureUnits;
		PickerHandler<NamedDbItem<ChartAggregationType>> _AggregationTypes;
		PickerHandler<NamedDbItem<ChartValueType>> _ValueTypes;
		PickerHandler<NamedDbItem<ChartType>> _ChartTypes;

		List<CheckedItem<int>> _ComparisonYears;

		#endregion

		#region Properties
		public VisualChartSearchParams ChartParameters
		{
			get => _ChartParameters;
			set => SetProperty(ref _ChartParameters, value);
		}

		/// <summary>
		/// Date ranges
		/// </summary>
		public PickerHandler<NamedDbItem<DateRange>> DateRanges
		{
			get
			{
				if (_DateRanges == null)
				{
					_DateRanges = new PickerHandler<NamedDbItem<DateRange>>(
						ChartParameters,
						nameof(VisualChartSearchParams.CurrentDateRange),
						nameof(NamedDbItem<DateRange>.Id));

					_DateRanges.AddRange(new NamedDbItem<DateRange>[]
					{
						new NamedDbItem<DateRange>() { Id = DateRange.Today, Name = E.T("today") },
						new NamedDbItem<DateRange>() { Id = DateRange.ThisWeek, Name = E.T("this-week") },
						new NamedDbItem<DateRange>() { Id = DateRange.ThisMonth, Name = E.T("this-month") },
						new NamedDbItem<DateRange>() { Id = DateRange.ThisQuarter, Name = E.T("this-quarter") },
						new NamedDbItem<DateRange>() { Id = DateRange.ThisYear, Name = E.T("this-year") },
						new NamedDbItem<DateRange>() { Id = DateRange.Last24Hours, Name = E.T("last24hours") },
						new NamedDbItem<DateRange>() { Id = DateRange.Last7Days, Name = E.T("last7days") },
						new NamedDbItem<DateRange>() { Id = DateRange.Last12Months, Name = E.T("last12months") },
					});
				}

				return _DateRanges;
			}
		}

		/// <summary>
		/// Measure units
		/// </summary>
		public PickerHandler<NamedDbItem<DatePartOrInterval>> MeasureUnits
		{
			get
			{
				if (_MeasureUnits == null)
				{
					_MeasureUnits = new PickerHandler<NamedDbItem<DatePartOrInterval>>(
						ChartParameters,
						nameof(VisualChartSearchParams.MeasureUnit),
						nameof(NamedDbItem<DatePartOrInterval>.Id));

					_MeasureUnits.AddRange(new NamedDbItem<DatePartOrInterval>[]
					{
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Minute, Name = E.T("minute") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Hour, Name = E.T("hour") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Day, Name = E.T("day") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Week, Name = E.T("week") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Month, Name = E.T("month") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Quarter, Name = E.T("quarter") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Year, Name = E.T("year") },
					});
				}

				return _MeasureUnits;
			}
		}

		/// <summary>
		/// Aggregation types
		/// </summary>
		public PickerHandler<NamedDbItem<ChartAggregationType>> AggregationTypes
		{
			get
			{
				if (_AggregationTypes == null)
				{
					_AggregationTypes = new PickerHandler<NamedDbItem<ChartAggregationType>>(
						ChartParameters,
						nameof(VisualChartSearchParams.AggregationType),
						nameof(NamedDbItem<ChartAggregationType>.Id));

					_AggregationTypes.AddRange(new NamedDbItem<ChartAggregationType>[]
					{
						new NamedDbItem<ChartAggregationType>() { Id = ChartAggregationType.RealValue, Name = E.T("real-value") },
						new NamedDbItem<ChartAggregationType>() { Id = ChartAggregationType.AverageValue, Name = E.T("average-value") },
						new NamedDbItem<ChartAggregationType>() { Id = ChartAggregationType.SumValue, Name = E.T("sum") },
						new NamedDbItem<ChartAggregationType>() { Id = ChartAggregationType.MinimalValue, Name = E.T("minimum-value") },
						new NamedDbItem<ChartAggregationType>() { Id = ChartAggregationType.MaximumValue, Name = E.T("maximum-value") },
					});
				}

				return _AggregationTypes;
			}
		}

		/// <summary>
		/// Value types
		/// </summary>
		public PickerHandler<NamedDbItem<ChartValueType>> ValueTypes
		{
			get
			{
				if (_ValueTypes == null)
				{
					_ValueTypes = new PickerHandler<NamedDbItem<ChartValueType>>(
						ChartParameters,
						nameof(VisualChartSearchParams.ValueType),
						nameof(NamedDbItem<ChartValueType>.Id));

					_ValueTypes.AddRange(new NamedDbItem<ChartValueType>[]
					{
						new NamedDbItem<ChartValueType>() { Id = ChartValueType.Value, Name = E.T("value") },
						new NamedDbItem<ChartValueType>() { Id = ChartValueType.Difference, Name = E.T("difference") },
					});
				}

				return _ValueTypes;
			}
		}

		/// <summary>
		/// Chart types
		/// </summary>
		public PickerHandler<NamedDbItem<ChartType>> ChartTypes
		{
			get
			{
				if (_ChartTypes == null)
				{
					_ChartTypes = new PickerHandler<NamedDbItem<ChartType>>(
						ChartParameters,
						nameof(VisualChartSearchParams.ChartType),
						nameof(NamedDbItem<ChartType>.Id));

					_ChartTypes.AddRange(new NamedDbItem<ChartType>[]
					{
						//new NamedDbItem<ChartType>() { Id = ChartType.None, Name = E.T("none") },
						new NamedDbItem<ChartType>() { Id = ChartType.Points, Name = E.T("points") },
						new NamedDbItem<ChartType>() { Id = ChartType.Line, Name = E.T("line") },
						new NamedDbItem<ChartType>() { Id = ChartType.Area, Name = E.T("area") },
						new NamedDbItem<ChartType>() { Id = ChartType.Bar, Name = E.T("bar") },
						//new NamedDbItem<ChartType>() { Id = ChartType.Pie, Name = E.T("pie") },
						//new NamedDbItem<ChartType>() { Id = ChartType.Donut, Name = E.T("donut") },
					});
				}

				return _ChartTypes;
			}
		}

		public override string Title
		{
			get
			{
				if (ChartParameters.SelectedDatapoints == null)
				{
					return E.T("select-datapoints");
				}
				else
				{
					return string.Format(E.T("x-selected"), ChartParameters.SelectedDatapoints.Count);
				}
			}
		}
		public string LabelDate { get => E.T("date"); }
		public string LabelFrom { get => E.T("from"); }
		public string LabelTo { get => E.T("to"); }
		public string LabelChartType { get => E.T("type"); }
		public string LabelMeasureUnit { get => E.T("interval"); }
		public string LabelDateRange { get => E.T("date-range"); }
		public string LabelAggregation { get => E.T("aggregation"); }
		public string LabelValue { get => E.T("value"); }
		public string LabelComparison { get => E.T("comparison"); }
		public string LabelChart { get => E.T("chart"); }
		public string LabelDownload { get => E.T("download"); }
		#endregion

		#region Static
		public IEnumerable<CheckedItem<int>> ComparisonYears
		{
			get
			{
				if (_ComparisonYears == null)
				{
					_ComparisonYears = (from item in Enumerable.Range(DateTime.Now.Year - Defaults.MAX_GRAPH_COMP_YEARS, Defaults.MAX_GRAPH_COMP_YEARS)
										select new CheckedItem<int>()
										{
											Checked = false,
											Id = item,
											Name = item.ToString(),
										}).ToList();
				}
				return _ComparisonYears;
			}
		}

		#endregion

		#region Ctor

		public GraphSelectionViewModel()
		{
		}

		#endregion

		#region Helpers
		/// <summary>
		/// Chart data loading
		/// </summary>
		/// <returns></returns>
		internal async Task LoadAsync(object sender)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(LoadAsync));
			if (sender != this)
				return;
			var stage = "Init";
			IsBusy = true;

			try
			{
				// Retrieve datapoint values from back-end
				stage = "Any Checked?";
				bool anyChecked = ComparisonYears.Any(y => y.Checked == true);

				stage = string.Format("{0}.{1}", nameof(ChartParameters), nameof(ChartParameters.ComparisonYears));
				ChartParameters.ComparisonYears = (from y in ComparisonYears
												   where y.Checked == true
												   select y.Id).ToList();

				stage = string.Format("{0}.{1}", nameof(ChartParameters), nameof(ChartParameters.DatapointValues));
				ChartParameters.DatapointValues = await _ApiServices.ChartDatapointValues(ChartParameters);

				// This is needed as without it DatapointValues have no DatapointName
				stage = nameof(BuildStructure) + "()";
				BuildStructure();
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert(
					string.Format("{0}, {1}", vLoc, stage),
					E.T("err-op") + Environment.NewLine + Environment.NewLine + ex.Message,
					E.T("ok"));
			}
			finally
			{
				IsBusy = false;
			}
		}

		/// <summary>
		/// Initializes whole proper structure Device->Datapoint->Value
		/// </summary>
		protected void BuildStructure()
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(BuildStructure));
			var dic = new Dictionary<string, Datapoint>();

			// Has years comparison
			// What means that records contain not only 0 year items
			var yearComp = ChartParameters.DatapointValues.Any(dv => dv.Year != 0);

			foreach (var dv in ChartParameters.DatapointValues)
			{
				var key = string.Format("{0}-{1}",
					dv.DatapointId,
					dv.Year);

				// If key wasn't found
				if (!dic.ContainsKey(key))
				{
					// Find existing selected datapoints to copy some info
					var oDp = ChartParameters.SelectedDatapoints.FirstOrDefault(
						dp => dp.Id == dv.DatapointId);

					// If its Datapoint found
					if (oDp != null)
					{
						var name = oDp.Name;
						if (yearComp)
						{
							// Calculating year of data
							var year = DateTime.Now.Year - dv.Year;

							if (!string.IsNullOrEmpty(oDp.Name))
							{
								if (oDp.Name.Length > Defaults.MAX_GRAPH_COMP_DP_NAME_LEN)
								{
									name = string.Format("{0}.. ",
										oDp.Name.Substring(0, Defaults.MAX_GRAPH_COMP_DP_NAME_LEN));
								}
								else
								{
									name = oDp.Name + " ";
								}
							}
							name += string.Format("({0})", year);
						}

						dic.Add(key, new Datapoint()
						{
							Id = oDp.Id,
							Name = name,
							Values = new List<DatapointValue>(),
						});
					}
				}

				// If key found after all
				// Could be that still not found for some cause
				// Such cases we skip
				if (dic.ContainsKey(key))
				{
					var dp = dic[key];
					if (dp != null)
					{
						dv.Datapoint = dp;
						dp.Values.Add(dv);
					}
				} // if (dic.ContainsKey(key))

			} // foreach(var dv in ChartParameters.DatapointValues)

			ChartParameters.PopulatedDatapoints = dic.Values;

			// Worked until 2023-08-04
			//foreach(var dp in ChartParameters.SelectedDatapoints)
			//{
			//    dp.Values = ChartParameters.DatapointValues.Where(dv => dv.DatapointId == dp.Id).ToList();
			//}
			// Worked until 2023-08-04
		}

		#endregion

		#region Methods
		#endregion

		#region Commands
		public ICommand ChartCommand
		{
			get
			{
				return new Command(async () =>
				{
					await LoadAsync(this);
					await Application.Current.MainPage.Navigation.PushAsync(

#if TEST1
                        new ChartSeriesFromDataPage()
                        {
                            BindingContext = new ChartSeriesFromDataViewModel()
                            {
								SeriesData = ChartParameters.DatapointValues,
							}
						}
#else
						new V.GraphChartGenericPage()
						{
							BindingContext = new GraphChartGenericViewModel()
							{
								ChartParameters = ChartParameters,
								// Newly added since 2023-07-04 as part of series name issues experiments
								GenericParams = new GenericChartParameters()
								{
									Title = ChartParameters.Title,
									GraphType = ChartParameters.ChartType,
									Interval = ChartParameters.MeasureUnit,
									SeriesDataMember = DatapointValue.SERIES_NAME_PROPERTY,
									ArgumentDataMember = DatapointValue.DISPLAY_MEMBER_PROPERTY,
									ValueMember = DatapointValue.VALUE_MEMBER_PROPERTY,
									DataSource = ChartParameters.PopulatedDatapoints,
								},
								//ChartSeries = DatapointValues,
							}
						}
#endif
					);
				});
			}
		}

		public ICommand DownloadCommand
		{
			get
			{
				return new Command(async () =>
				{
					var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(DownloadCommand));
					var stage = "Start";
					try
					{
						stage = nameof(_ApiServices.ChartDatapointValuesDownload);
						string url = await _ApiServices.ChartDatapointValuesDownload(ChartParameters);
						Debug.WriteLine(string.Format("{0}, DOCUMENT URL:{1}", vLoc, url));

						stage = "URL => URI";

						// @see https://stackoverflow.com/a/7581824
						Uri uri;
						if (Uri.TryCreate(url, UriKind.Absolute, out uri) &&
							(uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
						{
							stage = string.Format("{0}.{1}({2})", nameof(Launcher), nameof(Launcher.OpenAsync), url);
							await Launcher.OpenAsync(new Uri(url));
						}
					}
					catch (Exception ex)
					{
						await Application.Current.MainPage.DisplayAlert(
							vLoc,
							string.Format("{0}\r\nStage: {1}", ex.Message, stage),
							E.T("cancel"));
					}
				});
			}
		}
		#endregion


	}
}



