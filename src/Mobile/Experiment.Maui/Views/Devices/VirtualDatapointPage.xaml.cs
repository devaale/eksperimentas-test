using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Experiment.Maui.ViewModels.Devices;

namespace Experiment.Maui.Views.Devices{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VirtualDatapointPage : ContentPage
	{
		public VirtualDatapointPage()
		{
			InitializeComponent();

			Title = E.T("virtualDatapoint");
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if(BindingContext != null && BindingContext is DatapointViewModel)
			{
				var vm = BindingContext as DatapointViewModel;
				await vm.LoadAsync(this);
			}
		}
	}
}
