using Experiment.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Experiment.Maui.Views.Devices{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DeviceTopicsPage : ContentPage
	{
		public DeviceTopicsPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (BindingContext != null && BindingContext is ILoadableAsync)
			{
				await ((ILoadableAsync)BindingContext).LoadAsync(this);
			}
		}

	}
}
