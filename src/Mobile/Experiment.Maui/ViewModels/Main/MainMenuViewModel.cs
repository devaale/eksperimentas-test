using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Timer = System.Timers.Timer;

using Experiment.Core.Base;
using Experiment.Core.Ui;
using Experiment.Data.Enums;
using M=Experiment.Data.Models;

using D = Experiment.Maui.Data;
using V = Experiment.Maui.Views;

using Experiment.Maui.Models;
using Experiment.Maui.Services;
using System.Collections.Generic;
using Experiment.Maui.Data;

namespace Experiment.Maui.ViewModels.Main{
	public class MainMenuViewModel : ViewModelBase
	{
		#region Const
		const string TYPE_NAME = nameof(MainMenuViewModel);
		const int TIMER_DELAY = 60 * 1000;
#if DEBUG
		public const bool DEBUG = true;
#else
        public const bool DEBUG = false;
#endif

		#endregion

		#region Attributes
		readonly ApiServices _ApiServices = new ApiServices();
		Timer _Timer;
		M.Situation _CurrentSituation;
		int _CurrentAlarmsSituation;

		VisualUser _CurrentUser;
		M.Object _CurrentObject;

		string _LabelMenu;
		string _LabelDevices;
		string _LabelEcosystem;
		//string _LabelChat;
		string _LabelGraphs;
		string _LabelControl;
		string _LabelDeterioration;
		string _LabelEngineering;
		string _LabelWallet;
		string _LabelTest;
		string _LabelSettings;
		string _LabelLogoff;
		#endregion

		#region Properties
		public M.Situation CurrentSituation
		{
			get => _CurrentSituation;
			set
			{
				SetProperty(ref _CurrentSituation, value);
				OnPropertyChanged(nameof(ColorChat));
				OnPropertyChanged(nameof(LabelChat));
			}
		}

		public int CurrentAlarmsSituation
		{
			get => _CurrentAlarmsSituation;
			set
			{
				SetProperty(ref _CurrentAlarmsSituation, value);
				OnPropertyChanged(nameof(ColorAlarms));
				OnPropertyChanged(nameof(LabelAlarms));
			}
		}

		public VisualUser CurrentUser
		{
			get => _CurrentUser;
			set => SetProperty(ref _CurrentUser, value);
		}

		public M.Object CurrentObject
		{
			get => _CurrentObject;
			set => SetProperty(ref _CurrentObject, value);
		}

		public static bool ReloadUser = false;

		// To hide unneeded menu buttons (Martynas asked)
		public bool ShowNotImplemented { get => false; }

		// Multilanguage
		public string LabelMenu { get => _LabelMenu; set => SetProperty(ref _LabelMenu, value); }
		public string LabelDevices { get => _LabelDevices; set => SetProperty(ref _LabelDevices, value); }
		public string LabelEcosystem { get => _LabelEcosystem; set => SetProperty(ref _LabelEcosystem, value); }
		public string LabelChat
		{
			get
			{
				if (CurrentSituation != null)
				{
					if (CurrentSituation.NumOfUnreadMessages > 0)
					{
						return string.Format("{0} ({1})", E.T("chat"), CurrentSituation.NumOfUnreadMessages);
					}
				}
				return E.T("chat");
			}
		}
		public string LabelAlarms
		{
			get
			{
				if (CurrentAlarmsSituation > 0)
				{
					return string.Format("{0} ({1})", E.T("alarms"), CurrentAlarmsSituation);
				}
				return E.T("alarms");
			}
		}
		public string LabelGraphs { get => _LabelGraphs; set => SetProperty(ref _LabelGraphs, value); }
		public string LabelControl { get => _LabelControl; set => SetProperty(ref _LabelControl, value); }
		public string LabelDeterioration { get => _LabelDeterioration; set => SetProperty(ref _LabelDeterioration, value); }
		public string LabelEngineering { get => _LabelEngineering; set => SetProperty(ref _LabelEngineering, value); }
		public string LabelWallet { get => _LabelWallet; set => SetProperty(ref _LabelWallet, value); }
		public string LabelTest { get => _LabelTest; set => SetProperty(ref _LabelTest, value); }
		public string LabelSettings { get => _LabelSettings; set => SetProperty(ref _LabelSettings, value); }
		public string LabelLogoff { get => _LabelLogoff; set => SetProperty(ref _LabelLogoff, value); }
		// Colors
		public Color ColorChat
		{
			get
			{
				if (CurrentSituation != null)
				{
					if (CurrentSituation.NumOfUnreadMessages > 0)
					{
						return Colors.BlueViolet;
					}
				}
				return Colors.Black;
			}
		}
		public Color ColorAlarms
		{
			get
			{
				if (CurrentAlarmsSituation > 0)
				{
					return Colors.BlueViolet;
				}
				return Colors.Black;
			}
		}

		#endregion

		#region Ctor

		public MainMenuViewModel()
		{
			Title = E.T("menu");

			_Timer = new Timer(TIMER_DELAY);
			_Timer.Elapsed += new ElapsedEventHandler(TimerHandler);
			_Timer.AutoReset = true;
		}

		~MainMenuViewModel()
		{
			if (_Timer != null)
				_Timer.Enabled = false;
		}

		#endregion

		#region Events
		protected async void TimerHandler(object sender, ElapsedEventArgs e)
		{
			var vLoc = string.Format("{0}::{1}(object sender, ElapsedEventArgs e)", TYPE_NAME, nameof(TimerHandler));
			Debug.WriteLine(vLoc);

			if (D.Settings.IsLoggedIn)
			{
				await UpdateSituationAsync();
			}
		}

		#endregion

		#region Helpers
		protected async Task GoToLogoutAsync()
		{
			// @see https://stackoverflow.com/a/55033403
			INavigation nav = Application.Current.MainPage.Navigation;

			// First we finding exactly this page,
			// As this can be eg. SettingsPage which still not closed asynchronously,
			// while it got PopAsync command, but delayed, but should be removed after all
			// We don't bother with lingering settings page, but just doing what we need using LinQ
			Page mainMenuPage = nav.NavigationStack.FirstOrDefault(p => p is V.Main.MainMenuPage);
			await nav.PushAsync(new V.LoginPage());
			if (mainMenuPage != null)
			{
				nav.RemovePage(mainMenuPage);
			}

			// Previous way (might be needed)
			//var mainMenuPage = nav.NavigationStack.LastOrDefault();
			//await nav.PushAsync(new LoginPage());
			//nav.RemovePage(mainMenuPage);
		}

		#endregion

		#region Methods
		public async Task UpdateSituationAsync()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(UpdateSituationAsync));
			Debug.WriteLine(vLoc);

			if (!IsBusy)
			{
				IsBusy = true;
				_Timer.Enabled = false;

				// Situation update
				CurrentSituation = await _ApiServices.SituationAsync();

				// Active alarms list update
				CurrentAlarmsSituation = await _ApiServices.AlarmsSituationForMenuAsync(D.Settings.ObjectId.ToString(), 1m);

				_Timer.Enabled = true;
				IsBusy = false;
			}
		}

		public async Task LoadAsync(object sender)
		{
			try
			{
				var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(LoadAsync));
				Debug.WriteLine(vLoc);

				// This case can happen if in settings dualogue user changed server and logout happened
				// Then we need to kick user to login page. 
				if (!D.Settings.IsLoggedIn)
				{
					await GoToLogoutAsync();
					return;
				}

				IsBusy = true;

				// Pre-Load current user
				CurrentUser = await Dictionaries.Instance.GetCurrentUser(false);
				// Pre-Load current object
				CurrentObject = await Dictionaries.Instance.GetCurrentObject(false);

				// Re-assignment on each appearing of multilingual words needed because user in settings can change the language
				// After what everything should be updated
				// Elsewhere could be possible to set them in getters as they won't change, like in case of other non-main menu modules.
				Title = E.T("menu");

				LabelDevices = E.T("devices");
				LabelEcosystem = E.T("ecosystem");
				LabelGraphs = E.T("graphs");
				LabelControl = E.T("control");
				LabelDeterioration = E.T("deterioration");
				LabelEngineering = E.T("engineering");
				LabelWallet = E.T("wallet");
				LabelTest = E.T("test");
				LabelSettings = E.T("settings");
				LabelLogoff = E.T("logoff");
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
			}
			finally
			{
				IsBusy = false;
			}

			await UpdateSituationAsync();
		}

		#endregion

		#region Commands
		public ICommand DevicesCommand
		{
			get
			{
				return new Command(async () =>
				{
					if(CurrentObject == null)
					{
						await Application.Current.MainPage.DisplayAlert(
							E.T("err-op"),
							E.T("noObjectData"),
							E.T("cancel"));
					}
					else
					{
						// User always has access to own object
						var hasAccess = CurrentObject.IsOwnedObject;
						if(!hasAccess)
						{
							// But if it is not own object, check permissions
							var hasPermissions = CurrentObject.Permissions != null;
							if(hasPermissions)
							{
								var perm = CurrentObject.Permissions.Where(p => p.FriendUserId == CurrentUser.Id).FirstOrDefault();
								if(perm != null)
								{
									hasAccess = perm.PermDevice;
								}
							}
						}

						if(hasAccess)
						{
							// Open Page
							await Application.Current.MainPage.Navigation.PushAsync(
								new V.Devices.DevicesPage());
						}
						else
						{
							await Application.Current.MainPage.DisplayAlert(
								E.T("accessDenied"),
								E.T("accessDeniedFeature"),
								E.T("cancel"));
						}
					}
				});
			}
		}

		public ICommand EcosystemCommand
		{
			get
			{
				return new Command(async () =>
				{
					await Application.Current.MainPage.Navigation.PushAsync(
						new V.Ecosystem.PostsPage());
				});
			}
		}

		public ICommand ChatCommand
		{
			get
			{
				return new Command(async () =>
				{
					await Application.Current.MainPage.Navigation.PushAsync(
						new V.Ecosystem.ChatConversationsPage());
				});
			}
		}

		public ICommand AlarmsCommand
		{
			get
			{
				return new Command(async () =>
				{
					if (CurrentObject == null)
					{
						await Application.Current.MainPage.DisplayAlert(
							E.T("err-op"),
							E.T("noObjectData"),
							E.T("cancel"));
					}
					else
					{
						// User always has access to own object
						var hasAccess = CurrentObject.IsOwnedObject;
						if (!hasAccess)
						{
							// But if it is not own object, check permissions
							var hasPermissions = CurrentObject.Permissions != null;
							if (hasPermissions)
							{
								var perm = CurrentObject.Permissions.Where(p => p.FriendUserId == CurrentUser.Id).FirstOrDefault();
								if (perm != null)
								{
									hasAccess = perm.PermAlarm;
								}
							}
						}

						if (hasAccess)
						{
							// Open Page
							await Application.Current.MainPage.Navigation.PushAsync(
								new V.Alarms.AlarmsPage());
						}
						else
						{
							await Application.Current.MainPage.DisplayAlert(
								E.T("accessDenied"),
								E.T("accessDeniedFeature"),
								E.T("cancel"));
						}
					}
				});
			}
		}

		public ICommand GraphsCommand
		{
			get
			{
				return new Command(async () =>
				{
					// Open Graphs page
					await Application.Current.MainPage.Navigation.PushAsync(
						new V.Graph.GraphDevicesSelectPage());
				});
			}
		}

		public ICommand ControlCommand
		{
			get
			{
				return new Command(async () =>
				{
					// Open Control page
					await Application.Current.MainPage.Navigation.PushAsync(
						new V.Control.ControlPage());
				});
			}
		}

		public ICommand DeteriorationCommand
		{
			get
			{
				return new Command(async () =>
				{
				});
			}
		}

		public ICommand EngineeringCommand
		{
			get
			{
				return new Command(async () =>
				{
				});
			}
		}

		public ICommand WalletCommand
		{
			get
			{
				return new Command(async () =>
				{
				});
			}
		}

		public ICommand TestCommand
		{
			get
			{
				return new Command(async () =>
				{
					try
					{
						// CartesianChartPage
						//await Application.Current.MainPage.Navigation.PushAsync(new V.Test.CartesianChartPage());

						// CartesianChartPage
						await Application.Current.MainPage.Navigation.PushAsync(new V.Test.ChartSeriesFromDataPage());
					}
					catch (Exception ex)
					{
						await Application.Current.MainPage.DisplayAlert(
							E.T("err-op"),
							ex.Message,
							E.T("cancel"));

					}
				});
			}
		}

		public ICommand SettingsCommand
		{
			get
			{
				return new Command(async () =>
				{
					// Open Graphs page
					await Application.Current.MainPage.Navigation.PushAsync(
						new V.Settings.SettingsPage());
				});
			}
		}

		public ICommand LogoffCommand
		{
			get
			{
				return new Command(async () =>
				{
					try
					{

						await D.Settings.Logout();
						await GoToLogoutAsync();
					}
					catch (Exception ex)
					{
						await Application.Current.MainPage.DisplayAlert(
							E.T("err-op"),
							ex.Message,
							E.T("cancel"));

					}
				});
			}
		}
		#endregion // Commands

	}
}

