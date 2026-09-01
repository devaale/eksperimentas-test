using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Experiment.Core.Base;

// MVVM
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views.Graph;
using VM = Experiment.Maui.ViewModels.Graph;
using D = Experiment.Maui.Data;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.Views;
using Experiment.Maui.ViewModels.Devices;
using Experiment.Maui.Models;

namespace Experiment.Maui.ViewModels.Graph{
    public class GraphDevicesSelectViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(GraphDevicesSelectViewModel);

        #endregion

        #region Attributes
        readonly ApiServices _ApiServices = new ApiServices();

        #endregion

        #region Properties
        public ObservableCollection<Grouping<string, VisualDevice>> Devices { get; protected set; }

        public string LabelNext { get => E.T("next"); }

        #endregion

        #region Ctor 
        public GraphDevicesSelectViewModel()
        {
            // ML Init
            Title = E.T("select-devices");
            Devices = new ObservableCollection<Grouping<string, VisualDevice>>();
        }
        #endregion

        #region Delegates
        internal static string GetDeviceGroupingKey(VisualDevice d)
        {
            return d.ProtocolName;
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
                Devices.Clear();

                var devices = await _ApiServices.DeviceListAsync(
                    D.Settings.ObjectId.ToString());

                // Initialize their datapoint collections first
                foreach (var device in devices)
                {
                    if (device.Datapoints == null)
                    {
                        device.Datapoints = new List<M.Datapoint>();
                    }
                }

                Utils.GroupDevices(
                    Devices,
                    devices,
                    GetDeviceGroupingKey);

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
                    // Open Datapoints select page
                    var selDevs = new Dictionary<int, VisualDevice>();
                    foreach (var group in Devices)
                    {
                        foreach (var dev in group)
                        {
                            if (dev.Selected)
                            {
                                selDevs.Add(dev.Id, dev);
                            }
                        }
                    }

                    if (selDevs.Count > 0)
                    {
                        await Application.Current.MainPage.Navigation.PushAsync(
                            new V.GraphDatapointsSelectPage()
                            {
                                BindingContext = new GraphDatapointsSelectViewModel()
                                {
                                    Devices = selDevs,
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

