using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using DevExpress.Maui.Charts;

// MVVM
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.ViewModels.Graph;

namespace Experiment.Maui.Views.Graph{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GraphChartGenericPage : ContentPage
	{
		const string TYPE_NAME = nameof(GraphChartGenericPage);

		public GraphChartGenericPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (BindingContext is GraphChartGenericViewModel)
			{
				var vm = BindingContext as GraphChartGenericViewModel;
				await vm.LoadAsync(this);
			}
		}
	}
}

