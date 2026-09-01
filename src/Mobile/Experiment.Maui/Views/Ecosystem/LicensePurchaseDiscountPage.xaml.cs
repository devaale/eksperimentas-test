using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Experiment.Maui.ViewModels.Ecosystem;

namespace Experiment.Maui.Views.Ecosystem{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LicensePurchaseDiscountPage : ContentPage
	{
		public LicensePurchaseViewModel Vm { get => BindingContext as LicensePurchaseViewModel; }
		public LicensePurchaseDiscountPage()
		{
			InitializeComponent();

			Title = E.T("discountApply");
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
