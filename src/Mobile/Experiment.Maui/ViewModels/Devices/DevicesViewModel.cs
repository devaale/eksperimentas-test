using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Experiment.Core.Base;
using Experiment.Data.Enums;

// MVVM
using V = Experiment.Maui.Views.Devices;
using VM = Experiment.Maui.ViewModels.Devices;
using D = Experiment.Maui.Data;

using Experiment.Core.Data;
using Experiment.Core.Metadata;
using M =Experiment.Data.Models;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.Views;
using Experiment.Maui.Models;

namespace Experiment.Maui.ViewModels.Devices{
	public class DevicesViewModel : ViewModelBase, ILoadableAsync
	{
		#region Const
		const string TYPE_NAME = nameof(DevicesViewModel);

		#endregion

		#region Attributes
		readonly ApiServices _ApiServices = new ApiServices();
		VisualUser _CurrentUser;
		M.Object _CurrentObject;

		VisualDevice _SelectedItem;
		readonly List<VisualDevice> _AllDevices = new List<VisualDevice>();
		string _DeviceFilterText = string.Empty;

		#endregion

		#region Properties
		public ObservableCollection<Grouping<string, VisualDevice>> Devices { get; protected set; }

		/// <summary>Filters the device list by name, description, URL, or protocol group (case-insensitive substring).</summary>
		public string DeviceFilterText
		{
			get => _DeviceFilterText;
			set
			{
				if (SetProperty(ref _DeviceFilterText, value))
					ApplyDeviceFilter();
			}
		}

		public VisualDevice SelectedItem
		{
			get => _SelectedItem;
			set
			{
				SetProperty(ref _SelectedItem, value);

				if (_SelectedItem is VisualDevice)
				{
					ShowSelectedItem(_SelectedItem);
				}
			}
		}

		internal VisualUser CurrentUser
		{
			get => _CurrentUser;
			set
			{
				SetProperty(ref _CurrentUser, value);

				OnPropertyChanged(nameof(CanBeEdited));
			}
		}

		internal M.Object CurrentObject
		{
			get => _CurrentObject;
			set
			{
				SetProperty(ref _CurrentObject, value);

				OnPropertyChanged(nameof(CanBeEdited));
				OnPropertyChanged(nameof(CurrentObjectDesc));
			}
		}

		public string CurrentObjectDesc
		{
			get
			{
				var currentObject = E.T("none");
				if (CurrentObject != null)
				{
					currentObject = CurrentObject.Name;
				}
				return string.Format("{0} {1}", E.T("objectSelected"), currentObject);
			}
		}

		public bool CanBeEdited
		{
			get
			{
				if(CurrentObject != null && CurrentUser != null)
				{
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
								hasAccess = perm.PermDevice && perm.PermWrite;
							}
						}
					}

					return hasAccess;
				}
				return false;
			}
		}

		#endregion

		#region Ctor 
		public DevicesViewModel()
		{
			// ML Init
			Title = E.T("app-name");

			Devices = new ObservableCollection<Grouping<string, VisualDevice>>();

			LoadAsync(this);
		}
		#endregion

		#region Delegates
		internal static string GetDeviceGroupingKey(VisualDevice d)
		{
			return d.ProtocolName;
		}
		#endregion

		#region Helpers

		static bool DeviceMatchesFilter(VisualDevice d, string query, CompareInfo compareInfo)
		{
			static bool Sub(string? s, string q, CompareInfo inv) =>
				s != null && inv.IndexOf(s, q, CompareOptions.IgnoreCase) >= 0;

			return Sub(d.Name, query, compareInfo)
				|| Sub(d.Description, query, compareInfo)
				|| Sub(d.Url, query, compareInfo)
				|| Sub(d.ProtocolName, query, compareInfo);
		}

		void ApplyDeviceFilter()
		{
			if (Devices == null)
				return;

			List<VisualDevice> filtered;
			if (string.IsNullOrWhiteSpace(_DeviceFilterText))
			{
				filtered = _AllDevices.ToList();
			}
			else
			{
				var q = _DeviceFilterText.Trim();
				var inv = CultureInfo.CurrentCulture.CompareInfo;
				filtered = _AllDevices.Where(d => DeviceMatchesFilter(d, q, inv)).ToList();
			}

			Utils.GroupDevices(Devices, filtered, GetDeviceGroupingKey);

			if (SelectedItem != null && !filtered.Contains(SelectedItem))
				SelectedItem = null;
		}

		async Task ShowSelectedItem(VisualDevice item)
		{
			if (item != null)
			{
				switch (item.Protocol)
				{
					case DeviceProtocol.Modbus:

						await Application.Current.MainPage.Navigation.PushAsync(
						   new V.ModbusDeviceInfoPage()
						   {
							   BindingContext = new DeviceViewModel()
							   {
								   Item = item,
							   },
						   });

						break;


                    case DeviceProtocol.API:
                    case DeviceProtocol.BACnet:
                    case DeviceProtocol.MQTT:
                    case DeviceProtocol.CoAP:
                    case DeviceProtocol.OpenThread:

                        await Application.Current.MainPage.Navigation.PushAsync(
							new V.OtherProtocolDeviceInfoPage()
							{
								BindingContext = new DeviceViewModel()
								{
									Item = item,
								},
							});
							Console.WriteLine("Not supported");

						break;

					default:
						Console.WriteLine("Not supported");
						break;
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Loading data async for Page 
		/// </summary>
		public async Task LoadAsync(object sender)
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(LoadAsync));
			IsBusy = true;

			try
			{
				// Pre-Load current user
				CurrentUser = await Dictionaries.Instance.GetCurrentUser(false);
				// Pre-Load current object
				CurrentObject = await Dictionaries.Instance.GetCurrentObject(false);

				_AllDevices.Clear();
				var devices = await _ApiServices.DeviceListAsync(
					D.Settings.ObjectId.ToString());
				if (devices != null)
					_AllDevices.AddRange(devices);

				_DeviceFilterText = string.Empty;
				OnPropertyChanged(nameof(DeviceFilterText));
				ApplyDeviceFilter();
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert(
					vLoc,
					E.T("err-list-load") + Environment.NewLine + Environment.NewLine + ex.Message,
					E.T("ok"));
			}
			finally
			{
				IsBusy = false;
			}
		}
		#endregion

		#region Commands
		public ICommand NewDeviceCommand
		{
			get
			{
				return new Command(async () =>
				{
					await Application.Current.MainPage.Navigation.PushAsync(
						new V.FindDevicePage());
				});
			}
		}

        public ICommand EnableAiSupportCommand
        {
            get
            {
                return new Command(async () =>
                {
					var vLoc = $"{TYPE_NAME}::{nameof(EnableAiSupportCommand)}";
					try
					{
						IsBusy = true;

						var user = await Dictionaries.Instance.GetCurrentUser(false);
						if(user != null)
						{
							if(user.Licenses.Any(l => l.Type == UserLicenseType.License3))
							{
								// User has license
								var obj = await Dictionaries.Instance.GetCurrentObject(false);
								if (obj != null)
								{
									var retVal = await _ApiServices.ObjectEnableAiAsync(obj);
									if (retVal.IsSuccessStatusCode)
									{
										await LoadAsync(this);

										await Application.Current.MainPage.DisplayAlert(
											E.T("info"),
											E.T("done"),
											E.T("ok"));
									}
									else
									{
										await Application.Current.MainPage.DisplayAlert(
											E.T("operationFailed"),
											$"{retVal.ReasonPhrase}{Environment.NewLine}{Environment.NewLine}Code: {(int)retVal.StatusCode}{Environment.NewLine}Name: {retVal.StatusCode}",
											E.T("cancel"));
									}
								}
							}
							else
							{
								IsBusy = false;

								// User has no license
								await Application.Current.MainPage.DisplayAlert(
									E.T("weAreSorry"),
									string.Format(E.T("noLicenseX"), E.T("lic3name")),
									E.T("cancel"));
							}
						}

					}
					catch (Exception ex)
					{
						await Application.Current.MainPage.DisplayAlert(
							vLoc,
							E.T("err-op") + Environment.NewLine + Environment.NewLine + ex.Message,
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

