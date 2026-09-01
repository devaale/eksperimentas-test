using Experiment.Maui.ViewModels.Ecosystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Experiment.Maui.Views.Ecosystem{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LicensePurchaseTermPage : ContentPage
	{
		public LicensePurchaseViewModel Vm { get => BindingContext as LicensePurchaseViewModel; }
		public LicensePurchaseTermPage()
		{
			InitializeComponent();
			Title = E.T("choosePeriod");
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (Vm != null)
			{
				await Vm.LoadAsync(this);
			}
		}

	}
}
