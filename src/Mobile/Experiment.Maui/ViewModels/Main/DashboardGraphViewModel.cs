using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using Microsoft.Maui.Controls;

using Experiment.Core.Base;
using Experiment.Core.Enums;
using Experiment.Core.Ui;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;
using Experiment.Data.Models;

using D = Experiment.Maui.Data;

using Experiment.Maui.Data;
using Experiment.Maui.Models;
using Experiment.Maui.Services;
using Experiment.Maui.UI.Controls;
using Experiment.Maui.ViewModels.Devices;

namespace Experiment.Maui.ViewModels.Main{
	public class DashboardGraphViewModel : ViewModelBase
	{
		#region Const
		const string TYPE_NAME = nameof(DashboardGraphViewModel);
		const bool DEBUG = true;

		const int HEIGHT_ZOOMED_IN = 380;
		const int HEIGHT_ZOOMED_OUT = HEIGHT_ZOOMED_IN / 3;

		#endregion

		#region Attributes
		readonly ApiServices _ApiServices = new ApiServices();
		byte _GraphId;
		DashboardViewModel _Parent;
		GroupedIntIdItem _SelectedItem;
		GenericChartParameters _Params = new GenericChartParameters()
		{
			SeriesDataMember = DatapointValue.SERIES_NAME_PROPERTY,
			ArgumentDataMember = DatapointValue.DISPLAY_MEMBER_PROPERTY,
			ValueMember = DatapointValue.VALUE_MEMBER_PROPERTY,
		};

		PickerHandler<NamedDbItem<ChartType>> _GraphTypes;
		PickerHandler<NamedDbItem<DatePartOrInterval>> _Intervals;
		PickerHandler<NamedDbItem<ChartAggregationType>> _Aggregations;
		//IEnumerable<DatapointValue> _Values = new ObservableCollection<DatapointValue>();

		IList<DatapointViewModel> _Datapoints;

	bool _IsVisible = true;
	bool _IsZoomed = false;
	int _ColumnSpan = 1;
	int _RowSpan = 1;
	int _GridRow;
	int _GridColumn;
	int _OriginalGridRow;
	int _OriginalGridColumn;

		string _LabelGraphType;
		string _LabelIntervals;
		string _LabelDifference;
		string _LabelAggregation;
		string _LabelSave;
		string _DatapointFilterText = string.Empty;

		/// <summary>Checked state for all datapoint ids while editing (list rows are rebuilt when filtering).</summary>
		readonly Dictionary<int, bool> _DatapointCheckedState = new Dictionary<int, bool>();

		#endregion

		#region Properties
		public DashboardViewModel Parent
		{
			get => _Parent;
			set
			{
				SetProperty(ref _Parent, value);

				OnPropertyChanged(nameof(ChartType));
			}
		}
		public ObservableCollection<Grouping<string, GroupedIntIdItem>> Items { get; set; }

		/// <summary>Filters the datapoint list by name only (case-insensitive substring).</summary>
		public string DatapointFilterText
		{
			get => _DatapointFilterText;
			set
			{
				if (SetProperty(ref _DatapointFilterText, value))
					RebuildGroupedDatapointsList();
			}
		}

		public GroupedIntIdItem SelectedItem
		{
			get => _SelectedItem;
			set => SetProperty(ref _SelectedItem, value);
		}

		/// <summary>
		/// GenericChartParams for GenericChartControl
		/// </summary>
		public GenericChartParameters Params
		{
			get => _Params;
			set => SetProperty(ref _Params, value);
		}

		/// <summary>
		/// Graph types
		/// </summary>
		public PickerHandler<NamedDbItem<ChartType>> GraphTypes
		{
			get
			{
				if (_GraphTypes == null)
				{
					_GraphTypes = new PickerHandler<NamedDbItem<ChartType>>(
						this,
						nameof(GraphType),
						nameof(NamedDbItem<ChartType>.Id));

					_GraphTypes.AddRange(new NamedDbItem<ChartType>[]
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

				return _GraphTypes;
			}
		}

		/// <summary>
		/// Selected Graph Type
		/// </summary>
		public ChartType GraphType
		{
			get
			{
				switch (_GraphId)
				{
					case 1:
						return Parent.DashboardSetting.Graph1Type;

					case 2:
						return Parent.DashboardSetting.Graph2Type;

					case 3:
						return Parent.DashboardSetting.Graph3Type;

					case 4:
						return Parent.DashboardSetting.Graph4Type;
				}

				return ChartType.None;
			}

			set
			{
				switch (_GraphId)
				{
					case 1:
						Parent.DashboardSetting.Graph1Type = value;
						break;

					case 2:
						Parent.DashboardSetting.Graph2Type = value;
						break;

					case 3:
						Parent.DashboardSetting.Graph3Type = value;
						break;

					case 4:
						Parent.DashboardSetting.Graph4Type = value;
						break;
				}

				Params.GraphType = value;

				OnPropertyChanged(nameof(GraphType));
			}
		}

		/// <summary>
		/// Intervals
		/// </summary>
		public PickerHandler<NamedDbItem<DatePartOrInterval>> Intervals
		{
			get
			{
				if (_Intervals == null)
				{
					_Intervals = new PickerHandler<NamedDbItem<DatePartOrInterval>>(
						this,
						nameof(Interval),
						nameof(NamedDbItem<DatePartOrInterval>.Id));

					_Intervals.AddRange(new NamedDbItem<DatePartOrInterval>[]
					{
						//new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.None, Name = E.T("none") },
						//new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Minute, Name = E.T("minute") },    // Minute was removed under Martynas demand
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Hour, Name = E.T("hour") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Day, Name = E.T("day") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Week, Name = E.T("week") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Month, Name = E.T("month") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Quarter, Name = E.T("quarter") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Year, Name = E.T("year") },
					});
				}

				return _Intervals;
			}
		}

		/// <summary>
		/// Selected Interval
		/// </summary>
		public DatePartOrInterval Interval
		{
			get
			{
				switch (_GraphId)
				{
					case 1:
						return Parent.DashboardSetting.Graph1Interval;

					case 2:
						return Parent.DashboardSetting.Graph2Interval;

					case 3:
						return Parent.DashboardSetting.Graph3Interval;

					case 4:
						return Parent.DashboardSetting.Graph4Interval;
				}

				return DashboardSetting.DEFAULT_GRAPH_INTERVAL;
			}

			set
			{
				switch (_GraphId)
				{
					case 1:
						Parent.DashboardSetting.Graph1Interval = value;
						break;

					case 2:
						Parent.DashboardSetting.Graph2Interval = value;
						break;

					case 3:
						Parent.DashboardSetting.Graph3Interval = value;
						break;

					case 4:
						Parent.DashboardSetting.Graph4Interval = value;
						break;
				}

				Params.Interval = value;

				// Notification of this property change
				OnPropertyChanged(nameof(Interval));
			}
		}

		/// <summary>
		/// Aggregation types
		/// </summary>
		public PickerHandler<NamedDbItem<ChartAggregationType>> Aggregations
		{
			get
			{
				if (_Aggregations == null)
				{
					_Aggregations = new PickerHandler<NamedDbItem<ChartAggregationType>>(
						this,
						nameof(Aggregation),
						nameof(NamedDbItem<ChartAggregationType>.Id));

					_Aggregations.AddRange(new NamedDbItem<ChartAggregationType>[]
					{
						//new NamedDbItem<ChartAggregationType>() { Id = ChartAggregationType.RealValue, Name = E.T("real-value") },
						new NamedDbItem<ChartAggregationType>() { Id = ChartAggregationType.AverageValue, Name = E.T("average-value") },
						new NamedDbItem<ChartAggregationType>() { Id = ChartAggregationType.SumValue, Name = E.T("sum") },
						new NamedDbItem<ChartAggregationType>() { Id = ChartAggregationType.MinimalValue, Name = E.T("minimum-value") },
						new NamedDbItem<ChartAggregationType>() { Id = ChartAggregationType.MaximumValue, Name = E.T("maximum-value") },
					});
				}

				return _Aggregations;
			}
		}

		/// <summary>
		/// @TODO: Implement!
		/// </summary>
		public ChartAggregationType Aggregation 
		{
			get
			{
				switch (_GraphId)
				{
					case 1:
						return Parent.DashboardSetting.Graph1Aggregation;

					case 2:
						return Parent.DashboardSetting.Graph2Aggregation;

					case 3:
						return Parent.DashboardSetting.Graph3Aggregation;

					case 4:
						return Parent.DashboardSetting.Graph4Aggregation;
				}

				return DashboardSetting.DEFAULT_GRAPH_AGGREGATION;
			}

			set
			{
				switch (_GraphId)
				{
					case 1:
						Parent.DashboardSetting.Graph1Aggregation = value;
						break;

					case 2:
						Parent.DashboardSetting.Graph2Aggregation = value;
						break;

					case 3:
						Parent.DashboardSetting.Graph3Aggregation = value;
						break;

					case 4:
						Parent.DashboardSetting.Graph4Aggregation = value;
						break;
				}

				Params.AggregationType = value;

				// Notification of this property change
				OnPropertyChanged(nameof(Aggregation));
			}
		}

		/// <summary>
		/// Difference ON/OFF
		/// </summary>
		public bool Difference
		{
			get
			{
				switch (_GraphId)
				{
					case 1:
						return Parent.DashboardSetting.Graph1Difference;

					case 2:
						return Parent.DashboardSetting.Graph2Difference;

					case 3:
						return Parent.DashboardSetting.Graph3Difference;

					case 4:
						return Parent.DashboardSetting.Graph4Difference;
				}

				return false;
			}

			set
			{
				switch (_GraphId)
				{
					case 1:
						Parent.DashboardSetting.Graph1Difference = value;
						break;

					case 2:
						Parent.DashboardSetting.Graph2Difference = value;
						break;

					case 3:
						Parent.DashboardSetting.Graph3Difference = value;
						break;

					case 4:
						Parent.DashboardSetting.Graph4Difference = value;
						break;
				}

				// Notification of this property change
				OnPropertyChanged(nameof(Difference));
			}
		}

		/*
		public IEnumerable<DatapointValue> Values
		{
			get
			{
				var vLoc = string.Format("{0}#{1}::{2}[GET]", TYPE_NAME, _GraphId, nameof(Values));
				//Debug.WriteLineIf(DEBUG, vLoc);

				return _Values;
			}
			set
			{
				var vLoc = string.Format("{0}#{1}::{2}[SET]", TYPE_NAME, _GraphId, nameof(Values));
				//Debug.WriteLineIf(DEBUG, vLoc);

				SetProperty(ref _Values, value);
			}
		}
		*/

		public bool IsVisible { get => _IsVisible; set => SetProperty(ref _IsVisible, value); }
	public bool IsZoomed
	{
		get => _IsZoomed;
		set
		{
			SetProperty(ref _IsZoomed, value);

			if (_IsZoomed)
			{
				GridRow = 0;
				GridColumn = 0;
				ColumnSpan = 2;
				RowSpan = 2;
			}
			else
			{
				GridRow = _OriginalGridRow;
				GridColumn = _OriginalGridColumn;
				ColumnSpan = 1;
				RowSpan = 1;
			}

			OnPropertyChanged(nameof(LabelZoom));
			OnPropertyChanged(nameof(RequestedHeight));
		}
	}

	public int ColumnSpan { get => _ColumnSpan; set => SetProperty(ref _ColumnSpan, value); }
	public int RowSpan { get => _RowSpan; set => SetProperty(ref _RowSpan, value); }
	public int GridRow { get => _GridRow; set => SetProperty(ref _GridRow, value); }
	public int GridColumn { get => _GridColumn; set => SetProperty(ref _GridColumn, value); }
		public int RequestedHeight { get => IsZoomed ? HEIGHT_ZOOMED_IN : HEIGHT_ZOOMED_OUT; }

		public byte GraphId => _GraphId;

		public string Name { get => string.Format(E.T("graphX"), _GraphId); }
		public string LabelZoom
		{
			get
			{
				var z = IsZoomed ? E.T("zoomOut") : E.T("zoomIn");
				return string.IsNullOrEmpty(z) ? z : char.ToUpperInvariant(z[0]) + z.Substring(1);
			}
		}
		public string LabelGraphType { get => _LabelGraphType; set => SetProperty(ref _LabelGraphType, value); }
		public string LabelInterval { get => _LabelIntervals; set => SetProperty(ref _LabelIntervals, value); }
		public string LabelDifference { get => _LabelDifference; set => SetProperty(ref _LabelDifference, value); }
		public string LabelAggregation { get => _LabelAggregation; set => SetProperty(ref _LabelAggregation, value); }
		public string LabelSave { get => _LabelSave; set => SetProperty(ref _LabelSave, value); }
		public bool LegendVisible
		{
			get => Params.LegendVisible;
			set
			{
				if (Params.LegendVisible != value)
				{
					Params.LegendVisible = value;
					OnPropertyChanged(nameof(LegendVisible));
					OnPropertyChanged(nameof(LegendVisibleIcon));
				}
			}
		}
		public ImageSource LegendVisibleIcon => ImageSource.FromFile(LegendVisible ? "visibility.svg" : "visibility_off.svg");
		#endregion

		#region Ctor
		public DashboardGraphViewModel(DashboardViewModel parent, byte graphId)
		{
			var vLoc = string.Format("{0}::{1}(DashboardViewModel parent, byte graphId={2})",
				TYPE_NAME, nameof(DashboardGraphViewModel), graphId);

			if (parent == null)
				throw new ArgumentNullException(string.Format("{0}, {1} Can't be NULL!", vLoc, nameof(parent)));

			Parent = parent;
			_GraphId = graphId;

			// Initialize grid position based on graph ID (2x2 grid layout)
			// Graph 1: Row 0, Col 0 | Graph 2: Row 0, Col 1
			// Graph 3: Row 1, Col 0 | Graph 4: Row 1, Col 1
			switch (graphId)
			{
				case 1:
					_OriginalGridRow = 0;
					_OriginalGridColumn = 0;
					break;
				case 2:
					_OriginalGridRow = 0;
					_OriginalGridColumn = 1;
					break;
				case 3:
					_OriginalGridRow = 1;
					_OriginalGridColumn = 0;
					break;
				case 4:
					_OriginalGridRow = 1;
					_OriginalGridColumn = 1;
					break;
			}
			_GridRow = _OriginalGridRow;
			_GridColumn = _OriginalGridColumn;

			Items = new ObservableCollection<Grouping<string, GroupedIntIdItem>>();
		}

		#endregion

		#region Delegates
		internal static string GetItemGroupingKey(GroupedIntIdItem i)
		{
			//var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(GetItemGroupingKey));
			//Debug.WriteLineIf(DEBUG, string.Format("{0} = {1}", vLoc, i.Group));
			if (i.Group != null)
				return i.Group;
			else
				return "(null)";
		}
		#endregion

		#region Helpers
		/// <summary>
		/// Init multilanguage
		/// </summary>
		void InitMultilang ()
		{
			var vLoc = string.Format("{0}#{1}::{2}()",
				TYPE_NAME, _GraphId, nameof(InitMultilang));
			Debug.WriteLineIf(DEBUG, vLoc);

			Title = string.Format(E.T("graphXOptions"), _GraphId);
			LabelGraphType = E.T("type");
			LabelInterval = E.T("interval");
			LabelDifference = E.T("difference");
			LabelAggregation = E.T("aggregation");
			LabelSave = E.T("save");

			OnPropertyChanged(nameof(LabelZoom));
		}


		/// <summary>
		/// Get only this graph datapoints
		/// 
		/// Filtering ONLY THIS GRAPH datapoints
		///
		/// We not assigning the result directly to Params.DataSource
		/// As then we would trigger the load of the chart, when DatapointValue(s) not loaded yet
		///
		/// Except you wish to implement whole mechanism, which updating chart when values latter added?
		/// </summary>
		/// <returns></returns>
		bool InitDatapoints()
		{
			var vLoc = string.Format("{0}#{1}::{2}()",
				TYPE_NAME, _GraphId, nameof(InitDatapoints));
			Debug.WriteLineIf(DEBUG, vLoc);

			_Datapoints = new List<DatapointViewModel>();

			foreach(var ddp in Parent.DashboardSetting.Datapoints.Where(d => d.GraphId == _GraphId))
			{
				_Datapoints.Add(new DatapointViewModel()
				{
					Id = ddp.DatapointId,
					Name = ddp.Name,
					Values = new List<DatapointValue>(),
				});
			}

			return _Datapoints != null;
		}

		/// <summary>
		/// Build data structure for specific Dashboard Graph settings
		/// Where possible to see datapoints GROUPED by devices and so on.
		/// </summary>
		void BuildGroupStructure()
		{
			var vLoc = string.Format("{0}#{1}::{2}()",
				TYPE_NAME, _GraphId, nameof(BuildGroupStructure));
			Debug.WriteLineIf(DEBUG, vLoc);

			RebuildGroupedDatapointsList();

			Debug.WriteLineIf(DEBUG, string.Format("{0}, Done!", vLoc));
		}

		void SyncCheckedStateFromItems()
		{
			if (Items == null)
				return;
			foreach (var group in Items)
			{
				foreach (var item in group)
					_DatapointCheckedState[item.Id] = item.Checked;
			}
		}

		bool IsDatapointChecked(int datapointId)
		{
			if (_DatapointCheckedState.TryGetValue(datapointId, out var stored))
				return stored;
			return _Datapoints != null && _Datapoints.Any(ds => ds.Id == datapointId);
		}

		bool MatchesDatapointFilter(GroupedIntIdItem dp)
		{
			var q = _DatapointFilterText?.Trim() ?? string.Empty;
			if (q.Length == 0)
				return true;
			var inv = CultureInfo.CurrentCulture.CompareInfo;
			return dp.Name != null && inv.IndexOf(dp.Name, q, CompareOptions.IgnoreCase) >= 0;
		}

		/// <summary>Rebuilds grouped list; preserves checkbox state in <see cref="_DatapointCheckedState"/>.</summary>
		void RebuildGroupedDatapointsList()
		{
			if (Parent?.Datapoints == null || Items == null)
				return;

			SyncCheckedStateFromItems();

			Items.Clear();

			var allDps = from dp in Parent.Datapoints
						 where MatchesDatapointFilter(dp)
						 select new GroupedIntIdItem()
						 {
							 Id = dp.Id,
							 Name = dp.Name,
							 Group = dp.Group,
							 Checked = IsDatapointChecked(dp.Id),
						 };

			Utils.Group(
				Items,
				allDps,
				GetItemGroupingKey);
		}

		/// <summary>
		/// @Deprecated stuff from LoadAsync
		/// Delete it
		/// </summary>
		void UnusedVoodoo()
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(UnusedVoodoo));
			/*
			// For each datapoint in datasource
			foreach (var dp in datapoints)
			{
				// Take only its DatapointValue(s)
				var values = from dv in Parent.Values
							 where dv.DatapointId == dp.Id
							 select dv;

				if (Difference)
				{
					var diffVals = new List<DatapointValue>();
					var lastVals = new Dictionary<int, DatapointValue>();

					foreach (var v in values)
					{
						if (lastVals.ContainsKey(v.DatapointId))
						{
							var nv = new DatapointValue()
							{
								Id = v.Id,
								DatapointId = v.DatapointId,
								Datapoint = v.Datapoint,
								Date = v.Date,
								Value = v.Value - lastVals[v.DatapointId].Value,
							};

							diffVals.Add(nv);

							// Next
							lastVals[v.DatapointId] = v;
						}
						else
						{
							// Next without initialization
							lastVals.Add(v.DatapointId, v);
						}
					} // foreach

					dp.Values = diffVals;

				} // if (Difference)
				else
				{
					// Regular values
					dp.Values = values.ToList();
				}

				Debug.WriteLineIf(DEBUG, string.Format("{0}, Datapoint(Id={1}, Name={2}, Count={3})", vLoc, dp.Id, dp.Name, dp.Values.Count));

			} // foreach (var dp in Params.DataSource)
			*/
		}

		async Task LoadGraphData()
		{
			var vLoc = string.Format("{0}#{1}::{2}()",
				TYPE_NAME, _GraphId, nameof(LoadGraphData));
			Debug.WriteLineIf(DEBUG, vLoc);

			var @params = new VisualChartSearchParams()
			{
				ChartType = GraphType,
				CurrentDateRange = Parent.DashboardSetting.DateRange,
				DatapointIds = (from dp in _Datapoints select dp.Id).ToList(),
				MeasureUnit = Interval,
				AggregationType = Aggregation,
				ValueType = (Difference ? ChartValueType.Difference : ChartValueType.Value),
			};
			var values = await _ApiServices.ChartDatapointValues(@params);

			foreach(var dp in _Datapoints)
			{
				dp.Values = values.Where(v => v.DatapointId == dp.Id).ToList();
			}
		}

		/// <summary>
		/// Init Graph
		/// </summary>
		/// <param name="datapoints"></param>
		void InitGraph()
		{
			var vLoc = string.Format("{0}#{1}::{2}()",
				TYPE_NAME, _GraphId, nameof(InitGraph));
			Debug.WriteLineIf(DEBUG, vLoc);

			// GenericChartParams
			switch (_GraphId)
			{
				case 1:
					Params.Interval = Parent.DashboardSetting.Graph1Interval;
					Params.AggregationType = Parent.DashboardSetting.Graph1Aggregation;
					Params.GraphType = Parent.DashboardSetting.Graph1Type;
					break;

				case 2:
					Params.Interval = Parent.DashboardSetting.Graph2Interval;
					Params.AggregationType = Parent.DashboardSetting.Graph2Aggregation;
					Params.GraphType = Parent.DashboardSetting.Graph2Type;
					break;

				case 3:
					Params.Interval = Parent.DashboardSetting.Graph3Interval;
					Params.AggregationType = Parent.DashboardSetting.Graph3Aggregation;
					Params.GraphType = Parent.DashboardSetting.Graph3Type;
					break;

				case 4:
					Params.Interval = Parent.DashboardSetting.Graph4Interval;
					Params.AggregationType = Parent.DashboardSetting.Graph4Aggregation;
					Params.GraphType = Parent.DashboardSetting.Graph4Type;
					break;
			}
			Params.LegendVisible = true;
			OnPropertyChanged(nameof(LegendVisibleIcon));

			// Debug
			if (DEBUG)
			{
				Debug.WriteLineIf(DEBUG, string.Format("{0}, Control check of datapoints...", vLoc));
				foreach (var dp in _Datapoints)
				{
					Debug.WriteLineIf(DEBUG, string.Format("{0}, Id={1}, Name={2}, Values.Couunt={3}", vLoc, dp.Id, dp.Name, dp.Values.Count));
				}
			}

			// Assigning only checked for this GRAPH datapoints as its datasource
			Params.DataSource = _Datapoints;

			//Debug.WriteLineIf(DEBUG, string.Format("{0}, JSON: {1}", vLoc,JsonConvert.SerializeObject(Params)));

			//var debugInfo = new
			//{
			//	ChartSeriesName = DatapointValue.SERIES_NAME_PROPERTY,
			//	ChartSeriesDisplayMember = DatapointValue.DISPLAY_MEMBER_PROPERTY,
			//	ChartSeriesValueMember = DatapointValue.VALUE_MEMBER_PROPERTY,
			//	ChartTitle = Name,
			//	ChartType = GraphType,
			//	//ChartSeries = Values,
			//};
			//Debug.WriteLineIf(DEBUG, JsonConvert.SerializeObject(debugInfo));
			//Debug.WriteLineIf(DEBUG, Items.Count);
		}
		#endregion

		#region Methods

		public ICommand ToggleLegendCommand
		{
			get
			{
				return new Command(() =>
				{
					LegendVisible = !LegendVisible;
				});
			}
		}

		public async Task LoadAsync(object sender)
		{
			var vLoc = string.Format("{0}#{1}::{2}(object sender)", TYPE_NAME, _GraphId, nameof(LoadAsync));
			var step = "Start";
			Debug.WriteLineIf(DEBUG, vLoc);

			try
			{
				IsBusy = true;

				_DatapointCheckedState.Clear();
				_DatapointFilterText = string.Empty;
				OnPropertyChanged(nameof(DatapointFilterText));

				// Multilanguage
				step = nameof(InitMultilang);
				InitMultilang();

				// Get only this graph datapoints
				step = nameof(InitDatapoints);
				if (InitDatapoints())
				{
					// Init checked datapoints UI structure
					step = nameof(BuildGroupStructure);
					BuildGroupStructure();

					// Load from db
					step = nameof(LoadGraphData);
					await LoadGraphData();

					// Init graph (chart)
					step = nameof(InitGraph);
					InitGraph();
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(DEBUG, string.Format("{0}, Step: {1}, Msg: {2}, at: {3}]", 
					vLoc, step, ex.Message, ex.StackTrace));
			}
			finally
			{
				IsBusy = false;
			}
		}


		#endregion

		#region Commands
		public ICommand SaveCommand
		{
			get
			{
				return new Command(async () =>
				{
					var vLoc = string.Format("{0}#{1}::{2}", TYPE_NAME, _GraphId, nameof(SaveCommand));

					try
					{

						if (Parent.DashboardSetting != null)
						{
							SyncCheckedStateFromItems();

							// Re-cast to more linq friendly type
							var datapoints = new List<DashboardDatapoint>(Parent.DashboardSetting.Datapoints);

							// Remove all with this _GraphId
							datapoints.RemoveAll(dp => dp.GraphId == _GraphId);

							// Add selections for this graph (all ids — filter may hide rows)
							foreach (var dp in Parent.Datapoints)
							{
								if (IsDatapointChecked(dp.Id))
								{
									datapoints.Add(new DashboardDatapoint()
									{
										DatapointId = dp.Id,
										GraphId = _GraphId,
									});
								}
							}

							Parent.DashboardSetting.Datapoints = datapoints;

							var result = await _ApiServices.PostDashboardSettingAsync(Parent.DashboardSetting);
							if (result.IsSuccessStatusCode)
							{
								// Close this settings dialogue, but only if no errors
								await Application.Current.MainPage.Navigation.PopAsync();
							}
							else
							{
								await Application.Current.MainPage.DisplayAlert(
								   vLoc,
								   string.Format("{0} [1]", E.T("err-op")),
								   E.T("ok"));
							}
						}
					}

					catch (Exception ex)
					{
						await Application.Current.MainPage.DisplayAlert(
						   string.Format("{0} [2]", E.T("err-op")),
						   ex.Message,
						   E.T("ok"));
					}

					finally
					{

					}
				});
			}
		}

		#endregion
	}
}

