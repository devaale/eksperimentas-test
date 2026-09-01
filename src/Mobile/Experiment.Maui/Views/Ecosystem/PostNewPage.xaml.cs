using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels.Ecosystem;

using Experiment.Maui.Data;
using Experiment.Maui.Services;

namespace Experiment.Maui.Views.Ecosystem{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PostNewPage : ContentPage
	{

		internal VM.PostNewViewModel Vm { get => BindingContext as VM.PostNewViewModel; }

		public PostNewPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			await Vm.LoadAsync();
		}
	}
}
