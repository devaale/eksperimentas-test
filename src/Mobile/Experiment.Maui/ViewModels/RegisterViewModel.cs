#define SHOW_USER_POPUP

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;
using Microsoft.Maui.Controls;

using Experiment.Core;
using Experiment.Core.Base;
using Experiment.Core.Ui;
using Experiment.Data.Models;

using Experiment.Maui.Data;
using Experiment.Maui.Models;
using Experiment.Maui.Services;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;

namespace Experiment.Maui.ViewModels{
	public class RegisterViewModel : ViewModelBase
	{
		#region Const 
		const string TYPE_NAME = nameof(RegisterViewModel);

		#endregion

		#region Attributes
		readonly ApiServices _ApiServices = new ApiServices();
		RegisterBindingModel _Item;
		PickerHandler<Language> _Languages;
		string _Message;

		#endregion

		#region Properties
		public RegisterBindingModel Item
		{
			get => _Item;
			set => SetProperty(ref _Item, value);
		}
		public PickerHandler<Language> Languages
		{
			get
			{
				if (_Languages == null)
					_Languages = new PickerHandler<Language>(Item, nameof(Language), nameof(Language.Code));

				return _Languages;
			}
		}

		public string Message
		{
			get => _Message;
			set => SetProperty(ref _Message, value);
		}

		public string LabelName { get; protected set; }
		public string LabelEmail { get; protected set; }
		public string LabelPassword { get; protected set; }
		public string LabelConfirmPassword { get; protected set; }
		public string LabelRegister { get; protected set; }
		public string LabelCmbLanguageTitle { get; protected set; }
		public bool IsValid
		{
			get
			{
				// Empty fields validation
				var missed = new List<string>();
				if (string.IsNullOrEmpty(Item.Name))
					missed.Add(LabelName);

				if (string.IsNullOrEmpty(Item.Email))
					missed.Add(LabelEmail);

				if (string.IsNullOrEmpty(Item.Password))
					missed.Add(LabelPassword);

				if (string.IsNullOrEmpty(Item.ConfirmPassword))
					missed.Add(LabelConfirmPassword);

				if (missed.Count > 0)
				{
					Message = string.Format(
						"{0}:\r\n\r\n{1}", E.T("fillFields"), string.Join("\r\n", missed));
					return false;
				}

				// Min length of username
				Debug.WriteLine("Name: " + Item.Name);
				if (Item.Name.Length < Defaults.MIN_USERNAME_LENGTH)
				{
					Message = String.Format(
						E.T("shortUsername1"), 
						Defaults.MIN_USERNAME_LENGTH);
					return false;
				}

				// Email validation
				Regex regex = new Regex(
					@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
				Match match = regex.Match(Item.Email);
				if (!match.Success)
				{
					Message = E.T("enterValidEmail");
					return false;
				}

				// Password length 
				// At least one uppercase char
				// At least one lowercase char
				// At least one symbol
				// At least one digit
				if (Item.Password.Length < Defaults.MIN_PASSWORD_LENGTH ||
					!Item.Password.Any(char.IsUpper) ||
					!Item.Password.Any(char.IsLower) ||
					!Item.Password.Any(char.IsSymbol) ||
					!Item.Password.Any(char.IsNumber))
				{
					Message = String.Format(E.T("passwordReq"), Defaults.MIN_PASSWORD_LENGTH);
					return false;
				}
					
				// Password match
				if (!Item.Password.Equals(Item.ConfirmPassword))
				{
					Message = E.T("passwordMismatch");
					return false;
				}

				Message = String.Empty;
				return true;
			}
		}

		#endregion

		#region Ctor
		public RegisterViewModel()
		{
			Item = new RegisterBindingModel(this);

			// ML
			Title = E.T("register");
			LabelName = E.T("publicName");
			LabelEmail = E.T("email");
			LabelCmbLanguageTitle = E.T("language");
			LabelPassword = E.T("password");
			LabelConfirmPassword = E.T("passwordRepeat");
			LabelRegister = E.T("register");
		}

		#endregion

		#region Methods
		/// <summary>
		/// Called from Page class
		/// </summary>
		/// <returns></returns>
		public async Task LoadAsync()
		{
			var languages = await Dictionaries.Instance.GetLanguages(false);
			Languages.Clear();
			Languages.AddRange(languages);
		}

		/// <summary>
		/// Called by item
		/// </summary>
		public void ItemChanged()
		{
			OnPropertyChanged(nameof(IsValid));
		}

		#endregion

		#region Commands

		public ICommand RegisterCommand
		{
			get
			{
				return new Command(async () =>
				{
					var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(RegisterCommand));

					try
					{
						Message = E.T("registering...");
						IsBusy = true;

						var result = await _ApiServices.RegisterAsync(Item);

						string msgTitle, msgMessage;

						if (result.IsSuccessStatusCode)
						{
							Message = E.T("done");
							msgTitle = E.T("done");
							msgMessage = E.T("registered-success");
							Item = new RegisterBindingModel(this);
						}
						else
						{
							Message = E.T("retry-later") + Environment.NewLine + result.ReasonPhrase;
							msgTitle = E.T("err-registration");
							msgMessage = E.T("retry-later") + Environment.NewLine + result.ReasonPhrase;
						}
#if SHOW_USER_POPUP
						await Application.Current.MainPage.DisplayAlert(
							msgTitle, msgMessage, E.T("ok"));
#endif
					}
					catch (Exception ex)
					{
						await Application.Current.MainPage.DisplayAlert(
							vLoc,
							Environment.NewLine + Environment.NewLine + ex.Message,
							E.T("cancel"));
					}
					finally
					{
						IsBusy = false;
					}
					

				});
			}
		}

		#endregion
	}
}

