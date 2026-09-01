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
using Experiment.Maui.ViewModels.Devices;
using Experiment.Core.Metadata;

/*
 More info about XAML grouping, I had issues with it:
	https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/listview/customizing-list-appearance

	GroupDisplayBinding="{Binding TypeDesc}"
	GroupShortNameBinding="{Binding TypeName}"
				VerticalOptions="CenterAndExpand" 
                HorizontalOptions="CenterAndExpand"

 
 */
namespace Experiment.Maui.Views.Devices{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DevicesPage : ContentPage
	{
		public DevicesPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if(BindingContext != null && BindingContext is ILoadableAsync)
			{
				await ((ILoadableAsync)BindingContext).LoadAsync(this);
			}
		}
	}
}
