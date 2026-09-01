using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;

using Experiment.Core.Base;
using Experiment.Core.Enums;
using Experiment.Core.Ui;

using Experiment.Data.Enums;
using Experiment.Data.Models;

using D = Experiment.Maui.Data;
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views.Main;

using Experiment.Maui.Data;
using Experiment.Maui.Enums;
using Experiment.Maui.Models;
using Experiment.Maui.Services;
using Experiment.Maui.Views.Main;

namespace Experiment.Maui.ViewModels.Main{
	public class DashboardViewModel : ViewModelBase
	{
		#region Const
		const string TYPE_NAME = nameof(DashboardViewModel);
		const bool DEBUG = true;

		#endregion

		#region Attributes
		readonly ApiServices _ApiServices = new ApiServices();
		List<GroupedIntIdItem> _Datapoints;
		DashboardSetting _DashboardSetting;

		PickerHandler<NamedDbItem<DateRange>> _DateRanges;

		ObservableCollection<M.Object> _Objects;
		M.Object _SelectedObject;
		string _LabelObject;

		DashboardGraphViewModel _Graph1;
		DashboardGraphViewModel _Graph2;
		DashboardGraphViewModel _Graph3;
		DashboardGraphViewModel _Graph4;

		readonly ObservableCollection<DashboardGraphViewModel> _CarouselGraphs = new ObservableCollection<DashboardGraphViewModel>();
		DashboardGraphViewModel _SelectedCarouselGraph;

		string _LabelDateRange;
		string _LabelSettings;
		string _LabelChartSettings;
		string _LabelGeneralSettings;
		string _LabelClose;

		#endregion

		#region Properties
		public List<GroupedIntIdItem> Datapoints { get => _Datapoints; set => SetProperty(ref _Datapoints, value); }
		public DashboardSetting DashboardSetting
		{
			get => _DashboardSetting;
			set
			{
				SetProperty(ref _DashboardSetting, value);

				OnPropertyChanged(nameof(CanControl));
				OnPropertyChanged(nameof(SelectedDateRange));
			}
		}
		public List<DatapointValue> Values { get; set; }

		public DashboardGraphViewModel Graph1 { get => _Graph1; set => SetProperty(ref _Graph1, value); }
		public DashboardGraphViewModel Graph2 { get => _Graph2; set => SetProperty(ref _Graph2, value); }
		public DashboardGraphViewModel Graph3 { get => _Graph3; set => SetProperty(ref _Graph3, value); }
		public DashboardGraphViewModel Graph4 { get => _Graph4; set => SetProperty(ref _Graph4, value); }

		/// <summary>
		/// Only for for foreach processing of Graphs
		/// </summary>
		public IEnumerable<DashboardGraphViewModel> Graphs
		{
			get
			{
				yield return Graph1;
				yield return Graph2;
				yield return Graph3;
				yield return Graph4;
			}
		}

		/// <summary>Graph panes for the dashboard carousel (one full-screen chart at a time).</summary>
		public ObservableCollection<DashboardGraphViewModel> CarouselGraphs => _CarouselGraphs;

		public DashboardGraphViewModel SelectedCarouselGraph
		{
			get => _SelectedCarouselGraph;
			set
			{
				if (SetProperty(ref _SelectedCarouselGraph, value))
					OnPropertyChanged(nameof(SelectedCarouselGraphId));
			}
		}

		/// <summary>For bindings when <see cref="SelectedCarouselGraph"/> is briefly null.</summary>
		public byte SelectedCarouselGraphId => SelectedCarouselGraph?.GraphId ?? 1;

		/// <summary>
		/// Date ranges
		/// </summary>
		public PickerHandler<NamedDbItem<DateRange>> DateRanges
		{
			get => _DateRanges;
			set => SetProperty(ref _DateRanges, value);
		}
		public DateRange SelectedDateRange
		{
			get
			{
				if (DashboardSetting != null)
				{
					return DashboardSetting.DateRange;
				}
				return DateRange.None;
			}
			set
			{
				if (DashboardSetting != null)
				{
					var changed = DashboardSetting.DateRange != value;
					DashboardSetting.DateRange = value;

					if (changed)
					{
						DatePartOrInterval? interval = null;

						switch(value)
						{
							case DateRange.Last24Hours:
							case DateRange.Today:
								interval = DatePartOrInterval.Hour;
								break;

							case DateRange.Last7Days:
							case DateRange.ThisWeek:
								interval = DatePartOrInterval.Day;
								break;

							case DateRange.ThisMonth:
								interval = DatePartOrInterval.Week;
								break;

							case DateRange.ThisQuarter:
								interval = DatePartOrInterval.Month;
								break;

							case DateRange.ThisYear:
								interval = DatePartOrInterval.Quarter;
								break;
						}

						if(interval.HasValue)
						{
							foreach (var g in Graphs)
							{
								g.Interval = interval.Value;
							}
						}

						UpdateCurrentChartInterval();
					}
				}
			}
		}

		public bool CanControl { get => DashboardSetting != null && !IsBusy; }

		public override bool IsBusy
		{
			get => base.IsBusy;
			set
			{
				base.IsBusy = value;

				OnPropertyChanged(nameof(CanControl));
			}
		}

		public string LabelDateRange { get => _LabelDateRange; set => SetProperty(ref _LabelDateRange, value); }
		public string LabelSettings { get => _LabelSettings; set => SetProperty(ref _LabelSettings, value); }
		public string LabelChartSettings { get => _LabelChartSettings; set => SetProperty(ref _LabelChartSettings, value); }

		/// <summary>
		/// Objects for the object picker (dashboard scope).
		/// </summary>
		public ObservableCollection<M.Object> Objects
		{
			get => _Objects;
			set => SetProperty(ref _Objects, value);
		}

		/// <summary>
		/// Selected object in the picker. When changed, persists Settings/Dictionaries and reloads dashboard.
		/// </summary>
		public M.Object SelectedObject
		{
			get => _SelectedObject;
			set
			{
				var previousId = D.Settings.ObjectId;
				SetProperty(ref _SelectedObject, value);
				if (value != null && value.Id != previousId)
				{
					D.Settings.ObjectId = value.Id;
					D.Dictionaries.Instance.CurrentObject = value;
					_ = ReloadAfterObjectChangeAsync();
				}
			}
		}

		public string LabelObject { get => _LabelObject; set => SetProperty(ref _LabelObject, value); }
		public string LabelGeneralSettings { get => _LabelGeneralSettings; set => SetProperty(ref _LabelGeneralSettings, value); }
		public string LabelClose { get => _LabelClose; set => SetProperty(ref _LabelClose, value); }

		#endregion

		#region Ctor

		public DashboardViewModel()
		{
			Title = E.T("status");
		}
		#endregion

		#region Helpers

		static string CapFirst(string s)
		{
			if (string.IsNullOrEmpty(s)) return s;
			return char.ToUpperInvariant(s[0]) + s.Substring(1);
		}

		/// <summary>
		/// Update user's dashboard settings interval
		/// 
		/// Maybe as well worth to save it to db
		/// </summary>
		/// <returns></returns>
		async Task UpdateCurrentChartInterval()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(UpdateCurrentChartInterval));
			Debug.WriteLine(vLoc);

			try
			{
				IsBusy = true;

				DashboardSetting.ObjectId = D.Settings.ObjectId;
				var result = await _ApiServices.PostDashboardSettingAsync(DashboardSetting);
				if (!result.IsSuccessStatusCode)
				{
					await Application.Current.MainPage.DisplayAlert(
					   vLoc,
					   string.Format("{0} [1]", E.T("err-op")),
					   E.T("ok"));
				}
				else
				{
					await LoadAsync(this);
				}
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert(
				   vLoc,
				   string.Format("{0} {1}", E.T("err-op"), ex.Message),
				   E.T("ok"));
			}
			finally
			{
				IsBusy = false;
			}
		}

		async Task ReloadAfterObjectChangeAsync()
		{
			try
			{
				await LoadAsync(this);
				// MainPage (TabbedPage) may be root or the current page of a NavigationPage
				var root = Application.Current?.MainPage;
				var mainPage = root as MainPage
					?? (root as NavigationPage)?.CurrentPage as MainPage;
				if (mainPage?.BindingContext is MainViewModel mainVm)
					await mainVm.LoadAsync(null);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(string.Format("{0}::{1}, {2}\r\n{3}", TYPE_NAME, nameof(ReloadAfterObjectChangeAsync), ex.Message, ex.StackTrace));
			}
		}
		#endregion

		#region Methods
		public async Task LoadAsync(object sender)
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(LoadAsync));

			try
			{
				Debug.WriteLine(vLoc);

				IsBusy = true;

				// Re-assignment of ML words every load because it is part of Main menu, where language may change in settings
				Title = E.T("status");
				LabelDateRange = E.T("date-range");
				LabelSettings = CapFirst(E.T("settings"));
				LabelChartSettings = "Chart Settings";
				LabelObject = E.T("object");
				LabelGeneralSettings = CapFirst(E.T("filters"));
				LabelClose = E.T("close");

				// Load objects for picker (reuse from Dictionaries)
				var objects = await D.Dictionaries.Instance.GetObjects(true);
				Objects = objects ?? new ObservableCollection<M.Object>();

				// Load selected object datapoints 
				Datapoints = await _ApiServices.GetGroupedDatapointsAsync(D.Settings.ObjectId);

				// Load DashboardSetting for current object
				DashboardSetting = await _ApiServices.DashboardSettingLoadAsync(D.Settings.ObjectId);

				// After DashboardSetting initialization
				// ChartIntervals 
				if (DashboardSetting == null)
				{
					throw new Exception(string.Format("{0}, {1}::{2} is NULL!", vLoc, TYPE_NAME, nameof(DashboardSetting)));
				}
				else
				{
					// Date ranges for UI
					if (DateRanges == null)
					{
						DateRanges = new PickerHandler<NamedDbItem<DateRange>>(
							this,   // if here was not this, neeeded to re-initialize this on every load
							nameof(SelectedDateRange),
							nameof(NamedDbItem<DateRange>.Id));

						DateRanges.AddRange(new NamedDbItem<DateRange>[]
						{
							new NamedDbItem<DateRange>() { Id = DateRange.Today, Name = E.T("today") },
							new NamedDbItem<DateRange>() { Id = DateRange.ThisWeek, Name = E.T("this-week") },
							new NamedDbItem<DateRange>() { Id = DateRange.ThisMonth, Name = E.T("this-month") },
							new NamedDbItem<DateRange>() { Id = DateRange.ThisQuarter, Name = E.T("this-quarter") },
							new NamedDbItem<DateRange>() { Id = DateRange.ThisYear, Name = E.T("this-year") },
							new NamedDbItem<DateRange>() { Id = DateRange.Last24Hours, Name = E.T("last24hours") },
							new NamedDbItem<DateRange>() { Id = DateRange.Last7Days, Name = E.T("last7days") },
							//new NamedDbItem<DateRange>() { Id = DateRange.Last12Months, Name = E.T("last12months") },
						});
					} // if (DateRanges == null)


					// Init only once
					if (Graph1 == null)
					{
						Graph1 = new DashboardGraphViewModel(this, 1);
					}

					if (Graph2 == null)
					{
						Graph2 = new DashboardGraphViewModel(this, 2);
					}

					if (Graph3 == null)
					{
						Graph3 = new DashboardGraphViewModel(this, 3);
					}

					if (Graph4 == null)
					{
						Graph4 = new DashboardGraphViewModel(this, 4);
					}

                    // Load all graphs in parallel for better performance
                    await Task.WhenAll(
                        Graph1.LoadAsync(this),
                        Graph2.LoadAsync(this),
                        Graph3.LoadAsync(this),
                        Graph4.LoadAsync(this)
                    );

					// Single-graph carousel: reset legacy zoom grid state and fill carousel
					foreach (var g in Graphs)
					{
						g.IsZoomed = false;
						g.IsVisible = true;
					}

					var keepId = SelectedCarouselGraph?.GraphId;
					_CarouselGraphs.Clear();
					_CarouselGraphs.Add(Graph1);
					_CarouselGraphs.Add(Graph2);
					_CarouselGraphs.Add(Graph3);
					_CarouselGraphs.Add(Graph4);
					SelectedCarouselGraph = keepId is byte id && id >= 1 && id <= 4
						? _CarouselGraphs.FirstOrDefault(g => g.GraphId == id) ?? Graph1
						: Graph1;
					OnPropertyChanged(nameof(CarouselGraphs));
                }

				// Sync picker selection with current object (no reload in setter when id matches)
				SelectedObject = Objects?.FirstOrDefault(o => o.Id == D.Settings.ObjectId);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(string.Format("{0}, {1}\r\n{2}", vLoc, ex.Message, ex.StackTrace));
			}
			finally
			{
				IsBusy = false;
			}
		}
		#endregion

		#region Commands
		public ICommand OpenFiltersCommand
		{
			get
			{
				return new Command(async (param) =>
				{
					if (param is Page page)
					{
						var popup = new V.DashboardGeneralSettingsPopup { BindingContext = this };
						await page.ShowPopupAsync(popup);
					}
				});
			}
		}

		public ICommand ChartOptionsCommand
		{
			get
			{
				return new Command(async (param) =>
				{
					if (param != null)
					{
						byte graphId = 0;
						DashboardGraphViewModel vm = null;

						if (byte.TryParse(param.ToString(), out graphId))
						{
							switch (graphId)
							{
								case 1:
									vm = Graph1;
									break;

								case 2:
									vm = Graph2;
									break;

								case 3:
									vm = Graph3;
									break;

								case 4:
									vm = Graph4;
									break;
							}

							await Application.Current.MainPage.Navigation.PushAsync(
								new V.DashboardGraphPage()
								{
									BindingContext = vm,
								});
						}
					}
				});
			}
		}

		#endregion
	}
}

