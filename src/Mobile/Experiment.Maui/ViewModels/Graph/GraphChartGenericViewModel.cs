//#define BUILD_ALL_STRUCTURE  //Enable if is needed that Devices->Datapoints->Values had DatapointValue arrays
//#define DUMP_DATA

using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using DevExpress.Maui.Charts;

using Experiment.Core;
using Experiment.Core.Base;
using Experiment.Data.Metadata;
using Experiment.Data.Models;

// MVVM
using Experiment.Data.Enums;
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views.Graph;
using VM = Experiment.Maui.ViewModels.Graph;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.Views;
using Experiment.Maui.Models;
using Experiment.Maui.ViewModels.Devices;
using Experiment.Maui.UI.Controls;

namespace Experiment.Maui.ViewModels.Graph{
    public class GraphChartGenericViewModel : ViewModelBase
    {
        #region Constants
        const string TYPE_NAME = nameof(GraphChartGenericViewModel);

        #endregion

        #region Attributes
        VisualChartSearchParams _ChartParameters;
        GenericChartParameters _GenericParams;

        List<List<DatapointValue>> _DatapointValueNavigationList = new List<List<DatapointValue>>();
        List<DatapointValue> _CurrentDatapointValueNavigationEntity;
        DatapointValue _SelectedItem;

        /**
		 * For properties
		 */
        bool _CanGoBack = false;
        bool _CanGoForward = false;

        #endregion

        #region Properties

        public VisualChartSearchParams ChartParameters
        {
            get => _ChartParameters;
            set => SetProperty(ref _ChartParameters, value);
        }

		public GenericChartParameters GenericParams
        {
            get => _GenericParams;
            set => SetProperty(ref _GenericParams, value);
        }

		public bool CanGoBack
        {
            get => _CanGoBack;
            set => SetProperty(ref _CanGoBack, value);
        }

        public bool CanGoForward
        {
            get => _CanGoForward;
            set => SetProperty(ref _CanGoForward, value);
        }

        public List<DatapointValue> CurrentDatapointValueNavigationEntity
        {
            get => _CurrentDatapointValueNavigationEntity;
            set => SetProperty(ref _CurrentDatapointValueNavigationEntity, value);
        }

        internal int CurrentDatapointValueNavigationIndex;

        public DatapointValue SelectedItem
        {
            get => _SelectedItem;
            set
            {
                SetProperty(ref _SelectedItem, value);

                if (value != null)
                {
                    ShowDatapointValuesList();
                }
            }
        }

        public string LabelBack { get => E.T("back"); }
        public string LabelForward { get => E.T("forward"); }

        #endregion

        #region Ctor
        public GraphChartGenericViewModel()
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
            Debug.WriteLine(vLoc);
            IsBusy = true;

            try
            {
                BuildNavigationStructure();

                Title = string.Format("{0} chart for {1} to {2}",
                    ChartParameters.ChartType.ToString(),
                    ChartParameters.DateFrom.ToString(Defaults.DEFAULT_DATE_FORMAT),
                    ChartParameters.DateTo.ToString(Defaults.DEFAULT_DATE_FORMAT));

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    vLoc,
                    E.T("err-op") + Environment.NewLine + Environment.NewLine + ex.Message,
                    E.T("ok"));

            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Builds navigation structure for table below chart
        /// </summary>
        protected void BuildNavigationStructure()
        {
            var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(BuildNavigationStructure));
            Debug.WriteLine(vLoc);
            _DatapointValueNavigationList.Clear();

            var proceedList = new List<bool>();
            var proceed = true;
            var index = 1;

            while (proceed)
            {
                proceedList.Clear();
                var list = new List<DatapointValue>();

                foreach (var dp in ChartParameters.PopulatedDatapoints)
                {
                    DatapointValue dv;
                    if (dp.Values.Count - index > -1)
                    {
                        dv = dp.Values.ElementAt(dp.Values.Count - index);
                        proceedList.Add(true);
                    }
                    else
                    {
                        dv = new DatapointValue()
                        {
                            DatapointName = "N/A",
                        };
                        proceedList.Add(false);
                    }
                    list.Add(dv);
                }

                proceed = proceedList.Any(i => i == true);
                if (proceed)
                {
                    _DatapointValueNavigationList.Add(list);
                    index++;
                }
            } // While

            CurrentDatapointValueNavigationIndex = 0;
            UpdateDatapointValueNavigationEntity();
        }

        void UpdateDatapointValueNavigationEntity()
        {
            var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(UpdateDatapointValueNavigationEntity));
            Debug.WriteLine(string.Format("{0}, Current index: {1}", vLoc, CurrentDatapointValueNavigationIndex));

            if (_DatapointValueNavigationList.Count > 0)
            {
                CurrentDatapointValueNavigationEntity = _DatapointValueNavigationList[CurrentDatapointValueNavigationIndex];
            }

            CanGoForward = CurrentDatapointValueNavigationIndex > 0;
            CanGoBack = CurrentDatapointValueNavigationIndex < _DatapointValueNavigationList.Count - 1;
        }

        internal async void ShowDatapointValuesList()
        {
            var vLoc = string.Format("{0}::{1}({2})", TYPE_NAME, nameof(ShowDatapointValuesList), SelectedItem.DatapointName);
            Debug.WriteLine(vLoc);

            if (SelectedItem.Datapoint is IDatapoint)
            {
                var vm = new DatapointValueListViewModel(
                    (IDatapoint)SelectedItem.Datapoint,
                    CurrentDatapointValueNavigationIndex);

                await Application.Current.MainPage.Navigation.PushAsync(
                    new V.DatapointValueListPage(vm)
                );
            }
            else
            {
                Debug.WriteLine(vLoc + ", selected DatapointValue has no (VM.DatapointViewModel) Datapoint member initialized!");
            }

        }

        #endregion

        #region Methods

        #endregion

        #region Commands
        public ICommand GoForwardCommand
        {
            get
            {
                return new Command(async () =>
                {
                    CurrentDatapointValueNavigationIndex--;
                    UpdateDatapointValueNavigationEntity();
                });
            }
        }

        public ICommand GoBackwardCommand
        {
            get
            {
                return new Command(async () =>
                {
                    CurrentDatapointValueNavigationIndex++;
                    UpdateDatapointValueNavigationEntity();
                });
            }
        }
        #endregion

    }
}


