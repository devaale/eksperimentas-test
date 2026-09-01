using System;
using System.Threading.Tasks;

using Experiment.Maui.Models;
using Experiment.Maui.ViewModels.Devices;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Experiment.Maui.Views.Devices{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VirtualDatapointChainsPage : ContentPage
	{
		public VirtualDatapointChainsPage ()
		{
			InitializeComponent ();
		}

		async void OnPickRelatedDatapoint(object sender, EventArgs e)
		{
			if (sender is not BindableObject b || b.BindingContext is not VisualDatapointFormulaChain chain)
				return;
			if (BindingContext is not DatapointViewModel vm)
				return;

			await vm.OpenRelatedDatapointPickerAsync(this, chain);
		}
	}
}
