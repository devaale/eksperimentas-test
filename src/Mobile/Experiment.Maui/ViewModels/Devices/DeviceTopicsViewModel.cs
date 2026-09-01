using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Newtonsoft.Json;

using Experiment.Core.Base;
using Experiment.Data.Enums;

// MVVM
using V = Experiment.Maui.Views.Devices;
using VM = Experiment.Maui.ViewModels.Devices;
using D = Experiment.Maui.Data;

using Experiment.Core;
using Experiment.Core.Metadata;
using Experiment.Core.Data;
using M = Experiment.Data.Models;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.Views;
using Experiment.Maui.Models;
using Experiment.Data.Models;
using Experiment.Maui.ViewModels.Settings;
using Experiment.Data.Metadata;
using Experiment.Maui.Views.Devices;
using System.Net.Http;

namespace Experiment.Maui.ViewModels.Devices{
	public class DeviceTopicsViewModel : ViewModelBase, ILoadableAsync
	{
		#region Const
		const string TYPE_NAME = nameof(DeviceTopicsViewModel);

		#endregion

		#region Attributes
		readonly ApiServices _ApiServices = new ApiServices();
		M.Device _Device;
		DeviceTopic _SelectedItem;
		#endregion

		#region Properties
		public ObservableCollection<DeviceTopic> Items { get; protected set; }
		public DeviceTopic SelectedItem
		{
			get => _SelectedItem;
			set
			{
				SetProperty(ref _SelectedItem, value);

				OnPropertyChanged(nameof(IsAnythingSelected));
			}
		}

		public bool IsAnythingSelected
		{
			get => _SelectedItem != null;
		}

		public string LabelAdd { get => E.T("new"); }
		public string LabelEdit { get => E.T("edit"); }
		public string LabelDelete { get => E.T("delete"); }

		#endregion

		#region Ctor 
		public DeviceTopicsViewModel(M.Device device)
		{
			Validation.RequireValid(device, nameof(device));
			_Device = device;

			// ML Init
			Title = string.Format(E.T("nDeviceTopics"), Experiment.Core.Utils.ShortenTo(_Device.Name, 10));
			Items = new ObservableCollection<DeviceTopic>();

#if NOT_TRIGGERED_FROM_OUTSIDE
			LoadAsync(this);
#endif
		}

		#endregion

		#region Helpers
		/// <summary>
		/// Loading data async for Page 
		/// </summary>
		public async Task LoadAsync(object sender)
		{
			var vLoc = $"{TYPE_NAME}::{nameof(LoadAsync)}()";
			IsBusy = true;

			try
			{
				// Pre-Load current user
				//CurrentUser = await Dictionaries.Instance.GetCurrentUser(false);
				// Pre-Load current object
				//CurrentObject = await Dictionaries.Instance.GetCurrentObject(false);

#warning @TODO: GOOD EXAMPLE! Make like this everywhere!
				// Must have everywhere: reset items and selected item at once
				// Different approach can cause old validation states, after list is purged
				SelectedItem = null;
				Items.Clear();

				var items = await _ApiServices.DeviceTopicListAsync(_Device.Id);
				foreach (var i in items)
				{
					Items.Add(i);
				}
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
		public ICommand NewCommand
		{
			get
			{
				return new Command(async () =>
				{
					await Application.Current.MainPage.Navigation.PushAsync(

						new DeviceTopicPage()
						{
							BindingContext = new DeviceTopicViewModel()
							{
								Item = new M.DeviceTopic()
								{
									DeviceId = _Device.Id,
								},
							},
						});
				});
			}
		}

		public ICommand EditCommand
		{
			get
			{
				return new Command(async () =>
				{
					await Application.Current.MainPage.Navigation.PushAsync(

						new DeviceTopicPage()
						{
							BindingContext = new DeviceTopicViewModel()
							{
								Item = SelectedItem,
							},
						});
				});
			}
		}

		public ICommand DeleteCommand
		{
			get
			{
				return new Command(async () =>
				{
					var vLoc = $"{TYPE_NAME}::{nameof(DeleteCommand)}";

					try
					{
						if (SelectedItem != null)
						{
							var answer = await Application.Current.MainPage.DisplayAlert(
								E.T("question"),
								$"{E.T("sure-delete")}\r\n\r\n{SelectedItem.Topic}",
								E.T("yes"),
								E.T("no"));

							if (answer)
							{
								await _ApiServices.DeviceTopicDeleteAsync(SelectedItem);
							}
						}
#if PROCESS_RESPONSES
						E.ProcessResponse(response);
#endif
						await LoadAsync(this);
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
		#endregion
	}
}

