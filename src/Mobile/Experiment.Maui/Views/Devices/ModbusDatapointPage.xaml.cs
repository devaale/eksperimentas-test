using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Experiment.Core;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.ViewModels.Devices;

namespace Experiment.Maui.Views.Devices{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ModbusDatapointPage : ContentPage
	{
		//DatapointViewModel _Datapoint;

		public ModbusDatapointPage()
		{
			InitializeComponent();

			// ML UI Init
			Title = E.T("datapoint");
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if(BindingContext != null & BindingContext is DatapointViewModel)
			{
				var vm = BindingContext as DatapointViewModel;
				await vm.LoadAsync(this);
			}
		}
	}
}
