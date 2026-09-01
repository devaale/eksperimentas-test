using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
#if ANDROID
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
#endif
using Microsoft.Maui.Controls.Xaml;

using Experiment.Maui.ViewModels.Main;
using Experiment.Maui.Data;

namespace Experiment.Maui.Views.Main{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : global::Microsoft.Maui.Controls.TabbedPage
	{
		public MainPage()
		{
			InitializeComponent();
#if ANDROID
			// Horizontal swipes on the dashboard are for switching graphs (CarouselView), not tabs.
			this.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
#endif
		}

		#region Overrides
		protected override async void OnAppearing()
		{
			// Collecting all pages, except this MainPage
			List<Page> forRemoval = new List<Page>();
			foreach (var page in Navigation.NavigationStack)
			{
				if (page != this)
				{
					forRemoval.Add(page);
				}
			}

			// Removing them all from stack
			// Remove LoginPage from NavigationStack, @see https://stackoverflow.com/a/55040924
			foreach (var page in forRemoval)
			{
				Navigation.RemovePage(page);
			}

			// Old variant
			// Remove LoginPage from NavigationStack, @see https://stackoverflow.com/a/55040924
			//if (Navigation.NavigationStack.Count > 1)
			//{
			//	Page page = Navigation.NavigationStack.First();
			//	if (page != null && page != this)
			//	{
			//		Navigation.RemovePage(page);
			//	}
			//}
			base.OnAppearing();

			/*
			 * 2023-07 Oberved that LoadAsync causing an error, while environment was changed
			 *			and user was logged in, while LoadAsync doesn't check do user is still logged in.
			 *			Idea is that this dialogue can be accessed not only via startup, but as well after environment change.
			 */
			if(Data.Settings.IsLoggedIn)
			{
				if (BindingContext != null && BindingContext is MainViewModel)
				{
					await ((MainViewModel)BindingContext).LoadAsync(this);
				}
			}
		}

		#endregion
	}
}
