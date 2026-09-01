//#define USE_DEFAULT_CODE  // Enable auto generated default code, which left only for learning purposes
//#define TESTING

using System;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Devices;

#if TESTING
using Experiment.Core.Data;
using Experiment.Core.Metadata;
#endif

using Experiment.Maui.Services;
using Experiment.Maui.Views;
using Experiment.Maui.Data;

namespace Experiment.Maui{
	public partial class App : Application
	{
#if USE_DEFAULT_CODE
		//TODO: Replace with *.azurewebsites.net url after deploying backend to Azure
		//To debug on Android emulators run the web backend against .NET Core not IIS
		//If using other emulators besides stock Google images you may need to adjust the IP address
		public static string AzureBackendUrl =
			DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000" : "http://localhost:5000";
		public static bool UseMockDataStore = true;
#endif

		public App()
		{
			InitializeComponent();

#if USE_DEFAULT_CODE
			if (UseMockDataStore)
				DependencyService.Register<_MockDataStore>();
			else
				DependencyService.Register<_AzureDataStore>();
			MainPage = new MainPage();
#endif // USE_DEFAULT_CODE

#if TESTING
			DateRange.Test();
#endif // TESTING

			MainPage = new NavigationPage(new StartupPage());
		}

		protected override async void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}

