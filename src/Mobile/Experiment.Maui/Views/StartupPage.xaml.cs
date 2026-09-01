using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Services;
using Experiment.Maui.Data;

namespace Experiment.Maui.Views{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StartupPage : ContentPage
	{
		#region Const
		const string TYPE_NAME = nameof(StartupPage);

		#endregion


		#region Attributes
		bool _FirstStart = true;

		#endregion

		#region Properties
		VM.StartupViewModel Vm { get => BindingContext as VM.StartupViewModel; }

		#endregion

		#region Ctor
		public StartupPage()
		{
			InitializeComponent();
		}

		#endregion

		#region Overrides

		protected override async void OnAppearing()
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(OnAppearing));
			base.OnAppearing();

			Debug.WriteLine(vLoc);
			if(_FirstStart)
			{
				_FirstStart = false;
				Debug.WriteLine(vLoc +": First!");
				await StartupLoadAsync();
			}
		}

		#endregion

		#region Tasks

		/// <summary>
		/// Loads data (like multilingual words) from back-end
		/// </summary>
		/// <returns></returns>
		async Task StartupLoadAsync()
		{
			var vLoc = TYPE_NAME + "::" + nameof(StartupLoadAsync);
			Debug.WriteLine(vLoc + "...");

			// Starting load
			await Vm.LoadAsync();
			// Loading process
			while (Vm.IsBusy)
			{
				Task.Delay(250).Wait();
			}

			Debug.WriteLine(vLoc + ", " + nameof(Vm.CanProceed) + "=" + Vm.CanProceed);
			if (Vm.CanProceed)
			{
				var currentPage = Navigation.NavigationStack.LastOrDefault();
				await Workflow.Startup();
				Navigation.RemovePage(currentPage);
			}
			else
			{
				// App quit
				//Application.Current.Quit();
				Process.GetCurrentProcess().Kill();
			}
			
		}
		#endregion
	}
}
