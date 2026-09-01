using Experiment.Maui.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Experiment.Maui.Views{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterPage : ContentPage
	{

		RegisterViewModel Vm
		{
			get => (BindingContext is RegisterViewModel ? BindingContext as RegisterViewModel : null);
		}

		public RegisterPage()
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
