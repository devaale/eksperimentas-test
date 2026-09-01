//#define PROCESS_RESPONSES
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Maui.Controls;

using Newtonsoft.Json;

using Experiment.Core.Base;
using Experiment.Data.Enums;

// MVVM
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;
using D = Experiment.Maui.Data;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.Views.Devices;
using Experiment.Maui.Models;

namespace Experiment.Maui.ViewModels.Devices{
	public class DeviceViewModel : ViewModelBase
	{
		#region Const
		const string TYPE_NAME = nameof(DeviceViewModel);
		const bool DEBUG = false;

		#endregion

		#region Attributes
		readonly ApiServices _ApiServices = new ApiServices();
		VisualUser _CurrentUser;
		M.Object _CurrentObject;
		VisualDevice _Item;

		#endregion

		#region Properties
		public VisualDevice Item
		{
			get => _Item;
			set
			{
				SetProperty(ref _Item, value);

				if (_Item != null)
				{
					if (Item.IsNew)
					{
						Title = E.T("new-device");
					}
					else
					{
						Title = Item.Name;
					}
				}
				else
				{
					Title = E.T("notFound");
				}
			}
		}

		[JsonIgnore]
		internal VisualUser CurrentUser
		{
			get => _CurrentUser;
			set
			{
				SetProperty(ref _CurrentUser, value);

				OnPropertyChanged(nameof(CanBeEdited));
			}
		}

		[JsonIgnore]
		internal M.Object CurrentObject
		{
			get => _CurrentObject;
			set
			{
				SetProperty(ref _CurrentObject, value);

				OnPropertyChanged(nameof(CanBeEdited));
			}
		}

		[JsonIgnore]
		public bool CanBeEdited
		{
			get
			{
				if(Item != null)
				{
					if (CurrentObject != null && CurrentUser != null)
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
				}

				return false;
			}
		}

		[JsonIgnore]
		public bool CanBeDeleted
		{
			get
			{
				if(Item != null)
				{
					return CanBeEdited && Item.Protocol != DeviceProtocol.API;
				}
				return false;
			}
		}

		/// <summary>
		/// For legacy picker bindings that don't support normal bindings.
		/// </summary>
		public KeyValuePair<DeviceProtocol, string> SelectedProtocol
		{
			get
			{
				return Hardcoded.ProtocolTypes.FirstOrDefault(i => i.Key == Item?.Protocol);
			}

			set
			{
				if(Item != null)
				{
					if (Hardcoded.ProtocolTypes.ContainsKey(value.Key))
					{
						Item.Protocol = value.Key;
						//Debug.WriteLine("Device::SelectedProtocol, assigned Type: " + Type);
					}
					else
					{
						//throw new KeyNotFoundException();
						Item.Protocol = Hardcoded.ProtocolTypes.FirstOrDefault().Key;
					}
				}
			}
		}

		public bool IsMqtt { get => Item?.Protocol == DeviceProtocol.MQTT; }
		public bool IsBacnet { get => Item?.Protocol == DeviceProtocol.BACnet; }

		// deprecated 2024-06-27 DeviceViewModel should be VM of VIEW not Model class (VisualDevice)
		//public bool Selected { get; set; } 

		public string LabelSettings { get => E.T("settings"); }
		public string LabelAddDatapoint
		{
			get
			{
				var retVal = E.T("add-datapoint");

				if(Item?.Protocol == DeviceProtocol.MQTT)
				{
					retVal = E.T("topics");	// topigs
				}

				return retVal;
			}
		}
		public string LabelDatapoints { get => E.T("datapoints"); }
		public string LabelCancel { get => E.T("cancel"); }
		public string LabelSave { get => E.T("save"); }
		public string LabelBack { get => E.T("back"); }
		public string LabelDelete { get => E.T("delete"); }

		public string LabelName { get => E.T("name"); }
		public string LabelDescription { get => E.T("description"); }
		public string LabelUrl { get => E.T("url"); }
		public string LabelUnitId { get => E.T("unit-id"); }
		public string LabelInterval { get => E.T("interval"); }
		public string LabelProtocol { get => E.T("protocol"); }
		public string LabelClientId { get => E.T("client-id"); }
		public string LabelTopic { get => E.T("topic"); }
		public string LabelType { get => E.T("type"); }

		public string LabelDatapoint { get => E.T("datapoint"); }
		public string LabelVirtualDatapoint { get => E.T("virtualDatapoint"); }
		
		public string LabelDepreciation { get => E.T("depreciation"); }
		public string LabelDeprGL { get => E.T("deprGL"); }
		public string LabelDeprA { get => E.T("deprA"); }
		public string LabelDeprLIR { get => E.T("deprLIR"); }
		public string LabelDeprRL { get => E.T("deprRL"); }
		public string LabelDeprC { get => E.T("deprC"); }
		public string LabelDeprSD { get => E.T("deprSD"); }

		public string LabelClientUsername { get => E.T("clientUsername"); }
		public string LabelClientPassword { get => E.T("clientPassword"); }

		#endregion

		#region Ctor
		public DeviceViewModel()
		{
			Title = E.T("notFound");

			LoadAsync(this);
		}

		#endregion

		#region Helpers
		async Task LoadAsync(object sender)
		{
			// Pre-Load current user
			CurrentUser = await Dictionaries.Instance.GetCurrentUser(false);
			// Pre-Load current object
			CurrentObject = await Dictionaries.Instance.GetCurrentObject(false);
		}

		#endregion

		#region Commands

		/// <summary>
		/// Device saving or updating command
		/// </summary>
		public ICommand PostDeviceCommand
		{
			get
			{
				return new Command(async () =>
				{
					var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(PostDeviceCommand));

					try
					{
						IsBusy = true;
						HttpResponseMessage response;

						if(Item != null)
						{
							if (Item.Id < 1)
							{
								response = await _ApiServices.DevicePostAsync(Item);
							}
							else
							{
								response = await _ApiServices.DevicePutAsync(Item);
							}
#if PROCESS_RESPONSES
							E.ProcessResponse(response);
#endif
							// Close Device info ContentPage and go to the DevicesPage
							await Application.Current.MainPage.Navigation.PopAsync();
							// Until 2023-11-07 Dmitrijus legacy code
							//await Application.Current.MainPage.Navigation.PushAsync(new Experiment.Maui.Views.Devices.DevicesPage());	
						}
					}
					catch (Exception ex)
					{
						await Application.Current.MainPage.DisplayAlert(
							vLoc,
							E.T("err-op") + Environment.NewLine + Environment.NewLine + ex.Message,
							E.T("ok"));

					}
					finally
					{
						IsBusy = false;
					}
				});
			}
		}

		/// <summary>
		/// Device deletion command
		/// </summary>
		public ICommand DeleteDeviceCommand
		{
			get
			{
				return new Command(async () =>
				{
					var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(DeleteDeviceCommand));

					try
					{
						IsBusy = true;
						HttpResponseMessage response;

						if(Item != null)
						{
							if (Item.Id > 0)
							{

								var confirmationResult = await Application.Current.MainPage.DisplayAlert(
									E.T("question"),
									E.T("sure-delete"),
									E.T("yes"),
									E.T("no"));
								if (confirmationResult)
								{
									response = await _ApiServices.DeviceDeleteAsync(Item);
#if PROCESS_RESPONSES
								E.ProcessResponse(response);
#endif
									// Close Device info ContentPage
									await Application.Current.MainPage.Navigation.PopAsync();
								}
							}
						}
					}
					catch (Exception ex)
					{
						await Application.Current.MainPage.DisplayAlert(
							vLoc,
							E.T("err-op") + Environment.NewLine + Environment.NewLine + ex.Message,
							E.T("ok"));

					}
					finally
					{
						IsBusy = false;
					}

				});
			}
		}

		/// <summary>
		/// Cancel of Back command
		/// </summary>
		public ICommand CancelCommand
		{
			get
			{
				return new Command(async () =>
				{
					await Application.Current.MainPage.Navigation.PopAsync();
				});
			}
		}

		public ICommand AddDatapointMenuCommand
		{
			get
			{
				return new Command(async () =>
				{
					if(IsMqtt)
					{
						await Application.Current.MainPage.Navigation.PushAsync(
							new DeviceTopicsPage()
							{
								BindingContext = new DeviceTopicsViewModel(Item),
							});
					}
					else
					{
						await Application.Current.MainPage.Navigation.PushAsync(
							new DatapointNewMenuPage()
							{
								BindingContext = this,
							});
					}
				});
			}
		}

		public ICommand AddDatapointCommand
		{
			get
			{
				return new Command(async (param) =>
				{
					if (Item != null && param is DatapointType)
					{
						var datapoint = new DatapointViewModel()
						{
							DeviceId = Item.Id,
							// Initializing of Device property will cause creation of duplicate devices, which referencing this new Datapoint
							//Device = this,	
							DatapointType = (DatapointType)param,
							DeviceProtocol = Item.Protocol,
						};

						switch (datapoint.DatapointType)
						{
							case DatapointType.Normal:

								switch (Item.Protocol)
								{
									case DeviceProtocol.Modbus:

										await Application.Current.MainPage.Navigation.PushAsync(
											new ModbusDatapointPage()
											{
												BindingContext = datapoint,
											});

										break;

									case DeviceProtocol.BACnet:
									case DeviceProtocol.MQTT:
									case DeviceProtocol.CoAP:
									case DeviceProtocol.OpenThread:

										await Application.Current.MainPage.Navigation.PushAsync(
											new OtherProtocolDatapointPage()
											{
												BindingContext = datapoint,
											});

										break;

									default:
										break;
								}
								break;

							case DatapointType.Virtual:

								await Application.Current.MainPage.Navigation.PushAsync(
									new VirtualDatapointPage()
									{
										BindingContext = datapoint,
									});

								break;

							default:
								break;
						}
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
					if (Item != null)
					{
						switch (Item.Protocol)
						{
							case DeviceProtocol.Modbus:

								await Application.Current.MainPage.Navigation.PushAsync(
									new ModbusDeviceSettingsPage()
									{
										BindingContext = this,
									});

								break;

                            case DeviceProtocol.API:
                            case DeviceProtocol.MQTT:
                            case DeviceProtocol.BACnet:
							case DeviceProtocol.CoAP:
							case DeviceProtocol.OpenThread:

								await Application.Current.MainPage.Navigation.PushAsync(
									new OtherProtocolDeviceSettingsPage()
									{
										BindingContext = this,
									});

								break;

							default:
								break;

						}
					}
				});
			}
		}

		public ICommand DatapointsListCommand
		{
			get
			{
				return new Command(async () =>
				{
					if(Item != null)
					{
						if (Item.Id > 0)
						{
							await Application.Current.MainPage.Navigation.PushAsync(
								new DatapointsListPage(Item));
						}
						else
						{
							await Application.Current.MainPage.DisplayAlert(
								E.T("attention"),
								E.T("nothing-selected"),
								E.T("cancel"));
						}
					}
				});
			}
		}

		#endregion
	}
}

