using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Experiment.Core.Base;
using D = Experiment.Maui.Data;

using Experiment.Maui.Services;
using Experiment.Maui.Data;
using Experiment.Maui.Views;

namespace Experiment.Maui.ViewModels{
	public class LoginViewModel : ViewModelBase
	{
		ApiServices _ApiServices = new ApiServices();

		public string Username { get; set; }
		public string Password { get; set; }

		public string LabelUsername { get; protected set; }
		public string LabelPassword { get; protected set; }
		public string LabelLogin { get; protected set; }
		public string LabelForgotPassword { get; protected set; }
		public string LabelRegister { get; protected set; }
		public string LabelShare { get; protected set; }
		public string LabelContactUs { get; protected set; }

		public LoginViewModel()
		{
			Title = E.T("login");
			LabelUsername = E.T("email");
			LabelPassword = E.T("password");
			LabelLogin = E.T("login");
			LabelForgotPassword = E.T("forgot-password");
			LabelRegister = E.T("register");
			LabelShare = E.T("share");
			LabelContactUs = E.T("contact-us");
		}


		public ICommand LoginCommand
		{
			get
			{
				return new Command(async () =>
				{
					IsBusy = true;

					var json = await _ApiServices.LoginAsync(Username, Password);

					D.Settings.DeserializeLoginToken(json);

					IsBusy = false;

					if(D.Settings.IsLoggedIn)
					{
						// Previous LoginPage cleanning up itself MainMenuPage::OnAppearing, what is workaround
						await Workflow.Startup();
					}
					else
					{
						await Application.Current.MainPage.DisplayAlert(
							E.T("err-login-failed"),
							E.T("err-wrong-usrpas"),
							E.T("ok"));
					}
				});
			}
		}

		public ICommand RegisterCommand
		{
			get
			{
				return new Command(async () =>
				{
					// Open Registration page
					await Application.Current.MainPage.Navigation.PushAsync(
						new RegisterPage());
				});
			}
		}

		public ICommand ForgotPasswordCommand
		{
			get
			{
				return new Command(async () =>
				{
					// Open Registration page
					await Application.Current.MainPage.Navigation.PushAsync(
						new ForgotPasswordPage());
				});
			}
		}
	}
}

