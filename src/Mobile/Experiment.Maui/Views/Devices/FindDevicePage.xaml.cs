using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels.Devices;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using System.Xml.Linq;
using Experiment.Maui.ViewModels.Devices;

namespace Experiment.Maui.Views.Devices{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FindDevicePage : ContentPage
	{
		public FindDevicePage()
		{
			InitializeComponent();
		}
    }
}

