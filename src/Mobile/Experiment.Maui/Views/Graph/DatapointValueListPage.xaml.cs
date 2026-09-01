using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Experiment.Core;
using Experiment.Core.Base;

using Experiment.Data.Enums;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.ViewModels.Graph;

namespace Experiment.Maui.Views.Graph{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DatapointValueListPage : ContentPage
	{
		const string TYPE_NAME = nameof(DatapointValueListPage);

		DatapointValueListViewModel _Vm;

		public DatapointValueListPage(DatapointValueListViewModel viewModel)
		{
			var vLoc = string.Format("{0}::{1}(viewModel)", TYPE_NAME, nameof(DatapointValueListPage));
			InitializeComponent();

			Validation.RequireValid(viewModel, vLoc + ", viewModel parameter is empty!");
			_Vm = viewModel;
			this.BindingContext = _Vm;
		}
	}
}
