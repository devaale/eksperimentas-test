using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.ViewModels.Graph;

namespace Experiment.Maui.Views.Graph{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GraphDevicesSelectPage : ContentPage
	{
		public GraphDevicesSelectPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (BindingContext is GraphDevicesSelectViewModel)
			{
				var vm = BindingContext as GraphDevicesSelectViewModel;
				await vm.LoadAsync(this);
			}
		}
	}
}
