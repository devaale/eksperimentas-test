using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Experiment.Maui.Views{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ForgotPasswordPage : ContentPage
	{
		public ForgotPasswordPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			// ML Init
			Title = E.T("forgot-password");
			CmdSubmit.Text = E.T("submit");
		}
	}
}
