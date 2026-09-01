using Experiment.Maui.ViewModels.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Experiment.Maui.Views.Main{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DashboardGraphPage : ContentPage
	{
		public DashboardGraphPage()
		{
			InitializeComponent();
		}

		#region Overrides
		protected override async void OnAppearing()
		{
			if(BindingContext != null)
			{
				if(BindingContext is DashboardGraphViewModel)
				{
					await ((DashboardGraphViewModel)BindingContext).LoadAsync(this);
				}
			}
		}

		#endregion
	}
}
