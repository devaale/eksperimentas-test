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
	public partial class LicensePurchaseChoosePaymentMethodPage : ContentPage
	{
		public LicensePurchaseViewModel Vm { get => BindingContext as LicensePurchaseViewModel; }
		public LicensePurchaseChoosePaymentMethodPage()
		{
			InitializeComponent();

			Title = E.T("choosePaymentMethod");
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
