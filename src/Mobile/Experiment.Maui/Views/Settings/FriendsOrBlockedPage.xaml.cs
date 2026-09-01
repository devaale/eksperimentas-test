using Experiment.Maui.ViewModels.Settings;
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

namespace Experiment.Maui.Views.Settings{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FriendsOrBlockedPage : ContentPage
	{

		protected FriendsOrBlockedViewModel Vm
		{
			get => (BindingContext is FriendsOrBlockedViewModel ? BindingContext as FriendsOrBlockedViewModel : null);
		}


		public FriendsOrBlockedPage()
		{
			InitializeComponent ();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if(Vm != null)
			{
				await Vm.LoadAsync();
			}
		}
	}
}
