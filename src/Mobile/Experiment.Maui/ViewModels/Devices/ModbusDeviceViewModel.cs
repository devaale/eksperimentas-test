using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Microsoft.Maui.Controls;

using Experiment.Core.Base;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Services;

namespace Experiment.Maui.ViewModels.Devices{
    public class ModbusDeviceViewModel : ViewModelBase
    {
        #region Const

        const string TYPE_NAME = nameof(ModbusDeviceViewModel);

        #endregion

        #region Attributes


        #endregion

        #region Properties

        #endregion

        #region Ctor
        public ModbusDeviceViewModel()
        {

        }

        #endregion
    }
}
