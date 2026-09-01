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

using Experiment.Core;
using Experiment.Core.Base;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views.Graph;
using VM = Experiment.Maui.ViewModels.Graph;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.Views;
using Experiment.Maui.ViewModels.Devices;
using Experiment.Maui.Models;

namespace Experiment.Maui.ViewModels.Graph{
    public class GraphDatapointsSelectViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(GraphDatapointsSelectViewModel);

        #endregion

        #region Attributes
        readonly ApiServices _ApiServices = new ApiServices();

        Dictionary<int, VisualDevice> _Devices;

        #endregion

        #region Properties
        public Dictionary<int, VisualDevice> Devices
        {
            get => _Devices;
            set => SetProperty(ref _Devices, value);
        }

        public ObservableCollection<DatapointViewModel> Datapoints { get; protected set; }

        internal string DeviceIds
        {
            get
            {
                return string.Join(
                    Defaults.FIELD_SEPARATOR.ToString(),
                    _Devices.Select(o => o.Value.Id.ToString()));   // Linq Lambda
            }
        }
        public string LabelNext { get => E.T("next"); }

        #endregion

        #region Ctor 
        public GraphDatapointsSelectViewModel()
        {
            // ML Init
            Title = E.T("select-datapoints");
            Datapoints = new ObservableCollection<DatapointViewModel>();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Loads data asynchronous
        /// 
        /// Interesting styling @see https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/templates/data-templates/creating
        /// </summary>
        internal async Task LoadAsync(object sender)
        {
            var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(LoadAsync));
            IsBusy = true;

            try
            {
                Datapoints.Clear();
                var datapoints = await _ApiServices.DatapointListByDeviceIdsAsync(DeviceIds);

                foreach (var datapoint in datapoints)
                {
                    if (_Devices.ContainsKey(datapoint.DeviceId))
                    {
                        _Devices[datapoint.DeviceId].Datapoints.Add(datapoint);
                        Datapoints.Add(datapoint);
                    }

                }

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    vLoc,
                    E.T("err-list-load") + Environment.NewLine + Environment.NewLine + ex.Message,
                    E.T("ok"));
            }
            finally
            {
                IsBusy = false;
            }

        }

        #endregion

        #region Commands
        public ICommand NextCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var selDps = new List<DatapointViewModel>();
                    // Clean current datapoints, if such available
                    foreach (var dp in Datapoints)
                    {
                        if (dp.Selected)
                        {
                            selDps.Add(dp);
                        }
                    }

                    if (selDps.Count > 0)
                    {
                        await Application.Current.MainPage.Navigation.PushAsync(
                            new V.GraphSelectionPage()
                            {
                                BindingContext = new GraphSelectionViewModel()
                                {
                                    ChartParameters = new M.VisualChartSearchParams()
                                    {
                                        SelectedDatapoints = selDps,
                                    }
                                }
                            });
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            E.T("attention"),
                            E.T("nothing-selected"),
                            E.T("cancel"));
                    }
                });
            }
        }

        #endregion

    }
}

