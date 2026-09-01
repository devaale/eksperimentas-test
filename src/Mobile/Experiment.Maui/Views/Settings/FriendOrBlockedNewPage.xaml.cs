using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Experiment.Maui.ViewModels.Settings;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

namespace Experiment.Maui.Views.Settings{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FriendOrBlockedNewPage : ContentPage
	{

		protected FriendOrBlockedNewViewModel Vm { get => BindingContext as FriendOrBlockedNewViewModel; }

		public FriendOrBlockedNewPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			Vm.OnAppearing();
		}
	}
}
