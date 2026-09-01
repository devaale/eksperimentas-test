using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Experiment.Maui.ViewModels.Main;

namespace Experiment.Maui.Views.Main{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DashboardPage : ContentPage
	{
		public DashboardPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (BindingContext != null && BindingContext is DashboardViewModel)
			{
				await ((DashboardViewModel)BindingContext).LoadAsync(this);
			}
		}

	}
}
