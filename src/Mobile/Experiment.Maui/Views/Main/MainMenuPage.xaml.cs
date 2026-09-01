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
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.ViewModels.Main;

namespace Experiment.Maui.Views.Main{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainMenuPage : ContentPage
	{
		#region Ctor
		public MainMenuPage()
		{
			InitializeComponent();
		}

		#endregion

		#region Overrides
		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (BindingContext != null && BindingContext is MainMenuViewModel)
			{
				await ((MainMenuViewModel)BindingContext).LoadAsync(this);
			}
		}

		#endregion

	}
}
