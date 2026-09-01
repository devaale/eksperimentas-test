using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;

using Microsoft.Maui.Controls;

using Experiment.Core.Base;

using Experiment.Maui.Services;

namespace Experiment.Maui.ViewModels{
	public class ForgotPasswordViewModel : ViewModelBase
	{
		#region Attributes
		ApiServices _ApiServices = new ApiServices();
		string _Email;

		#endregion

		#region Properties
		public string Email
		{
			get => _Email;
			set
			{
				SetProperty(ref _Email, value);

				OnPropertyChanged(nameof(CanBeSend));
			}
		}

		public bool CanBeSend
		{
			get
			{
				if (string.IsNullOrEmpty(Email))
					return false;

				// Email validation
				Regex regex = new Regex(
					@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
				Match match = regex.Match(Email);
				return match.Success;
			}
		}

		public string LabelSubmit { get => E.T("submit"); }
		public string LabelEmail { get => E.T("email"); }
		public string LabelEnterYourEmail { get => E.T("enterYourEmail"); }

		#endregion

		#region Command

		public ICommand ForgotPasswordCommand
		{
			get
			{
				return new Command(async () =>
				{
					var result = await _ApiServices.ForgotPasswordAsync(Email);
					if(result.IsSuccessStatusCode)
					{
						// checkYourEmail
						await Application.Current.MainPage.DisplayAlert(
							E.T("forgot-password"),
							E.T("checkYourEmail"),
							E.T("ok"));
					}
					else
					{
						// somethingWentWrong
						await Application.Current.MainPage.DisplayAlert(
							E.T("forgot-password"),
							E.T("somethingWentWrong"),
							E.T("cancel"));
					}

					await Application.Current.MainPage.Navigation.PopAsync();
				});
			}
		}

		#endregion
	}
}

