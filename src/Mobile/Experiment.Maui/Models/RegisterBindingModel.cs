using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Experiment.Core;
using Experiment.Core.Base;
using Experiment.Data.Metadata;

using Experiment.Maui.ViewModels;

namespace Experiment.Maui.Models{
    public class RegisterBindingModel : ViewModelBase, IUser
	{
		//public string Name { get; set; }
		//public string Email { get; set; }
		//public string Password { get; set; }
		//public string ConfirmPassword { get; set; }
		//public string Language { get; set; }

		#region Attributes

		protected string _Name;
		protected string _Email;
		protected string _Password;
		protected string _ConfirmPassword;
		protected string _Language;
		protected int _Tokens;

		protected RegisterViewModel _Vm;

		#endregion

		#region Properties
		public string Name
		{
			get => _Name;
			set => SetProperty(ref _Name, value);
		}
		public string Email
		{
			get => _Email;
			set => SetProperty(ref _Email, value);
		}
		public string Password
		{
			get => _Password;
			set => SetProperty(ref _Password, value);
		}
		public string ConfirmPassword
		{
			get => _ConfirmPassword;
			set => SetProperty(ref _ConfirmPassword, value);
		}
		public string Language
		{
			get => _Language;
			set => SetProperty(ref _Language, value);
		}
		public int Tokens
		{
			get => _Tokens;
			set => SetProperty(ref _Tokens, value);
		}

		#endregion

		#region Ctor
		public RegisterBindingModel()
		{
			Name = string.Empty;
			Email = string.Empty;
			Password = string.Empty;
			ConfirmPassword = string.Empty;
			Language = Defaults.DEFAULT_LANGUAGE;
			Tokens = 0;
		}

		public RegisterBindingModel(RegisterViewModel vm)
			: this()
		{
			_Vm = vm;
		}

		#endregion

		#region Overrides
		protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			if(_Vm != null)
			{
				_Vm.ItemChanged();
			}
		}

		#endregion
	}
}
