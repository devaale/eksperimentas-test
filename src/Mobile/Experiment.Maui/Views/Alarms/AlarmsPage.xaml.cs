using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

// MVVM
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.ViewModels.Alarms;

namespace Experiment.Maui.Views.Alarms{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AlarmsPage : ContentPage
	{
		protected AlarmsViewModel Vm { get => BindingContext as AlarmsViewModel; }
		public AlarmsPage()
		{
			InitializeComponent();
		}
		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (Vm != null)
			{
				await Vm.LoadAsync();
			}
		}
	}
}
