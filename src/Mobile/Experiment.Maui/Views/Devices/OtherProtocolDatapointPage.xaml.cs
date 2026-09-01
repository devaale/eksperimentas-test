using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Experiment.Maui.ViewModels.Devices;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

namespace Experiment.Maui.Views.Devices{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OtherProtocolDatapointPage : ContentPage
    {
        //DatapointViewModel _Datapoint;
        public OtherProtocolDatapointPage()
        {
            InitializeComponent();

			// ML UI Init
			Title = E.T("datapoint");
        }

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (BindingContext != null & BindingContext is DatapointViewModel)
			{
				var vm = BindingContext as DatapointViewModel;
				await vm.LoadAsync(this);
			}
		}
	}
}

