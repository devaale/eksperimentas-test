using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System.Threading.Tasks;

using Experiment.Core.Base;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Services;
using Experiment.Maui.Data;

namespace Experiment.Maui.ViewModels{
	public class StartupViewModel : ViewModelBase
	{
		#region Const
		const string TYPE_NAME = nameof(StartupViewModel);

		#endregion

		#region Attributes
		readonly ApiServices _ApiServices = new ApiServices();
		string _Status;

		#endregion

		#region Properties

		/// <summary>
		/// Not implemented as we don't have loaded ML words yet.
		/// Maybe betted to add some animation, instead of ML words
		/// </summary>
		public string Status
		{
			get => _Status;
			set => SetProperty(ref _Status, value);
		}

		public bool CanProceed { get; set; }

		#endregion

		#region Ctor
		public StartupViewModel()
		{
			CanProceed = true;
		}

		#endregion


		#region Helpers

		#endregion

		#region Methods

		/// <summary>
		/// Startup load itself
		/// 
		/// Here you can add various needed to load data for App during startup
		/// </summary>
		/// <returns></returns>
		public async Task LoadAsync()
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(LoadAsync));
			IsBusy = true;
			Debug.WriteLine(vLoc + "...");

			try
			{
				// FYI: Here put all preload operations during waiting
				// @TODO: This, I'm not sure is this best approach to do this

				await _ApiServices.UpdateWordsAsync();

				Debug.WriteLine(vLoc + ": END!");

			}
			catch (Exception ex)
			{

				CanProceed = false;

				Debug.WriteLine(string.Format("{0}, {1}", 
					vLoc, ex.Message));

				var errorMsg = string.Format(
					"The application failed to contact the server.\r\n\r\nSystem message: {0}", ex.Message);

				await Application.Current.MainPage.DisplayAlert(
					"Application startup",
					errorMsg,
					E.T("cancel"));
			}
			finally
			{
				IsBusy = false;
			}
		}

		#endregion

	}
}

