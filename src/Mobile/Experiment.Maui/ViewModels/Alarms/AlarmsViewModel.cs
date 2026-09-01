using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Timer = System.Timers.Timer;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Experiment.Core;
using Experiment.Core.Base;
using Experiment.Data.Enums;
using Experiment.Core.Ui;

// MVVM
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;
using D = Experiment.Maui.Data;

using Experiment.Maui.Data;
using Experiment.Maui.Models;
using Experiment.Maui.Services;
using Experiment.Maui.Views;
using Experiment.Maui.Enums;

namespace Experiment.Maui.ViewModels.Alarms{
    public class AlarmsViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(AlarmsViewModel);
        const bool DEBUG = true;
        const int TIMER_DELAY = 30 * 1000;

        #endregion

        #region Attributes
        readonly ApiServices _ApiServices = new ApiServices();
        Timer _Timer;

        /// <summary>
        /// All loaded items
        /// </summary>
        List<VisualAlgorithm> _AllItems = new List<VisualAlgorithm>();
        /// <summary>
        /// Only filtered items, which will be bound
        /// </summary>
        List<VisualAlgorithm> _Items = new List<VisualAlgorithm>();

        bool _IsRefreshing;

        #endregion

        #region Properties

        /// <summary>
        /// Only filtered items, which will be bound
        /// </summary>
        public List<VisualAlgorithm> Items
        {
            get => _Items;
            set => SetProperty(ref _Items, value);
        }

        public bool IsRefreshing
        {
            get => _IsRefreshing;
            set => SetProperty(ref _IsRefreshing, value);
        }

        #endregion

        #region Ctor
        public AlarmsViewModel()
        {
            Title = E.T("alarms");
            _Timer = new Timer(TIMER_DELAY);
            _Timer.Elapsed += new ElapsedEventHandler(TimerHandler);
            _Timer.AutoReset = true;
        }

        #endregion

        #region Events
        protected async void TimerHandler(object sender, ElapsedEventArgs e)
        {
            var vLoc = string.Format("{0}::{1}(object sender, ElapsedEventArgs e)", TYPE_NAME, nameof(TimerHandler));
            Debug.WriteLine(vLoc);

            await LoadAsync();
        }

        #endregion

        #region Helpers

        #endregion

        #region Methods
        public async Task LoadAsync()
        {
			var vLoc = string.Format("{0}::{1}()",
				TYPE_NAME, nameof(LoadAsync));
            Debug.WriteLineIf(DEBUG, vLoc);
			
            try
			{

                if (IsRefreshing)
                    return;

                _Timer.Enabled = false;
                IsRefreshing = true;
                //IsBusy = true;

                // Retrieving alarms
                // 2023-12-20 Items = await _ApiServices.AlarmsListAsync(D.Settings.ObjectId.ToString(), 1m);
				Items = await _ApiServices.AlarmsListAsync(D.Settings.ObjectId);

				// Set as Read all alarms
				var result = await _ApiServices.AlarmsReadAsync(D.Settings.ObjectId);

				//PopulateData();
				_Timer.Enabled = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}, {1}", vLoc, ex.Message));
            }
            finally
            {
                IsRefreshing = false;
                //IsBusy = false;
            }
        }

        #endregion

        #region Commands

        #endregion

    }
}

