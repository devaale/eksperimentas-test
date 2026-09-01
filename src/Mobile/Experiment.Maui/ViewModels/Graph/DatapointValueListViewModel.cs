using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Core;
using Experiment.Core.Base;
using Experiment.Data.Metadata;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.ViewModels.Devices;

namespace Experiment.Maui.ViewModels.Graph{
    public class DatapointValueListViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(DatapointValueListViewModel);

        #endregion

        #region Attributes

        IDatapoint _Datapoint;
        int _SelectedIndex;

        #endregion

        #region Properties

        public IDatapoint Datapoint
        {
            get => _Datapoint;
            protected set => SetProperty(ref _Datapoint, value);
        }

        #endregion

        #region Ctor
        private DatapointValueListViewModel()
        {

        }

        internal DatapointValueListViewModel(
			IDatapoint datapoint, int selectedIndex)
            : this()
        {
            var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(DatapointValueListViewModel));

            Validation.RequireValid(datapoint, vLoc + ", datapoint parameter is empty!");
            Datapoint = datapoint;
            _SelectedIndex = selectedIndex;
        }

        #endregion
    }
}
