using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Experiment.Maui.ViewModels.Control;

namespace Experiment.Maui.Views.Control{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AlgorithmPage : ContentPage
	{
		protected AlgorithmViewModel Vm
		{
			get => (BindingContext is AlgorithmViewModel ? BindingContext as AlgorithmViewModel : null);
		}

		public AlgorithmPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (Vm != null)
			{
				await Vm.LoadAsync();
			}
		}
	}
}
