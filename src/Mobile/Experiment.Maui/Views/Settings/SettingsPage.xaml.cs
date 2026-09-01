using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Services;

namespace Experiment.Maui.Views.Settings{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage, INotifyPropertyChanged
	{
		#region Attributes
		readonly ApiServices _ApiServices = new ApiServices();
		#endregion

		#region Properties

		#endregion

		#region Ctor
		public SettingsPage()
		{
			InitializeComponent();
		}

		#endregion

		#region Overrides

		#endregion

		#region Helpers

		#endregion

	}
}
