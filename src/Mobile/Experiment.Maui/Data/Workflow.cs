//#define DISABLE_PING
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;
using Experiment.Maui.Services;

namespace Experiment.Maui.Data{
	internal class Workflow
	{
		const string TYPE_NAME = nameof(Workflow);
		public static async Task Startup()
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(Startup));
			var apiServices = new ApiServices();
			Debug.WriteLine(vLoc + "...");

			// If local variables show that we are logged in
#if !DISABLE_PING
			if(Settings.IsLoggedIn)
			{
				try
				{
					// Pinging the server to confirm that we are logged in
					var result = await apiServices.PingAsync();
					if (!result.IsSuccessStatusCode)
					{
						// If not, we forgetting that we were logged in
						await Settings.Logout();
					}
		}
				catch
				{
					// If was any error, no matter, setting that we're not logged in too
					await Settings.Logout();
				}
			}
#endif

			if (!Settings.IsLoggedIn)
			{
				//Debug.WriteLine("Workflow::Startup... LOGIN...");
				// If not, let's give him way to log in
				await Application.Current.MainPage.Navigation.PushAsync(
					new V.LoginPage());
				return;
			}
			else
			{
				Debug.WriteLine(vLoc + ", Found LoggedIn!");
			}

			// Should be logged in here
			try
			{
				Debug.WriteLine(vLoc + ", Trying to get Objects...");

				// Loading objects for verification
				//
				// The idea is that Settings.LoggedIn shows only that device settings have some logged in info, which might be good
				// But as well that info can be wrong, eg. in case of environment or server changed via sttings
				// In such case we're using GetObjects to test is authentification valid,
				// where if saved credentials are wrong, of eg. different environment, will be thrown HttpRequestException (see below).
				// 
				//var objects = await Dictionaries.Instance.GetObjects(false);
				// Finding selected object
				//var currentObject = objects.FirstOrDefault(o => o.Id.Equals(Settings.Object));
				var currentObject = await Dictionaries.Instance.GetCurrentObject(false);
				if (currentObject == null)
				{
					//Debug.WriteLine("Workflow::Startup... SELECT OBJECT...");
					Settings.ObjectId = 0;

					// Go to Object selection
					// It should handle manually main menu opening, removal of itself and so on

					await Application.Current.MainPage.Navigation.PushAsync(
						new V.Settings.ObjectsPage()
						{
							BindingContext = new VM.Settings.ObjectsViewModel()
							{
								IsSelectionMode = true,
							},
						});
					return;
				}
			}
			catch(HttpRequestException hrex)
			{
				// In case if we got here, this means that GetObjects failed probably because of wrong or another enrivonment credentials
				// So then we doing Logoff, destroying all saved in device credentials and recursivelly calling this method after this itself
				// Which then will go under fist case Settings.LoggedIn after what Login window will be opened for login.
				Debug.WriteLine(string.Format("{0}, hrex.Message={1}", vLoc, hrex.Message));
				await Settings.Logout();
				await Startup();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(string.Format("{0}, ex.Message={1}", vLoc, ex.Message));
			}
			finally
			{

			}

			// If Logged in and object is ok, loading MainMenu, just now
			if (Settings.IsLoggedIn && Settings.ObjectId != 0)
			{
				//Debug.WriteLine("Workflow::Startup... MAIN MENU...");
				await Application.Current.MainPage.Navigation.PushAsync(
					new V.Main.MainPage());
			}
		}
	}
}

