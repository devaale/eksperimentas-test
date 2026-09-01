using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Experiment.Core;
using Experiment.Core.Base;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using System.Drawing;
using Experiment.Maui.Models;

namespace Experiment.Maui.ViewModels.Devices{
    public class DatapointsListViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(DatapointsListViewModel);

        #endregion

        #region Attributes
        readonly ApiServices _ApiServices = new ApiServices();

        VisualDevice _Device;

        #endregion

        #region Properties
        public ObservableCollection<DatapointViewModel> Datapoints { get; protected set; }

        #endregion

        #region Ctor 
        public DatapointsListViewModel()
        {
            // ML Init
            Title = E.T("app-name");
        }

        public DatapointsListViewModel(VisualDevice device)
            : this()
        {
            if (device == null && device.Id < 1)
                throw new ArgumentException(E.T("nothing-selected"));

            _Device = device;
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
                var datapoints = await _ApiServices.DatapointListByDeviceIdsAsync(_Device.Id.ToString());

                foreach (var datapoint in datapoints)
                {
                    Datapoints.Add(datapoint);
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
    }
}

