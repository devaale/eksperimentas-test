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

using Experiment.Core.Base;
using Experiment.Data.Enums;

// MVVM
using V = Experiment.Maui.Views.Devices;
using VM = Experiment.Maui.ViewModels.Devices;
using D = Experiment.Maui.Data;

using Experiment.Core;
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
	public class DeviceTopicViewModel : ViewModelBase
	{
		#region Constants
		const string TYPE_NAME = nameof(DeviceTopicViewModel);

		#endregion

		#region Attributes
		readonly ApiServices _ApiServices = new ApiServices();
		M.DeviceTopic _Item;

		#endregion

		#region Properties
		public M.DeviceTopic Item
		{
			get => _Item; 
			set
			{
				SetProperty(ref _Item, value);

				// Initialize dialogue title recarding set item
				if(_Item == null)
				{
					// Empty?
					Title = "N/A";
				}
				else
				{
					if(_Item.Id == 0)
					{
						// New
						Title = E.T("newTopic");
					}
					else
					{
						// Edit
						Title = _Item.Topic;
					}
				}
			}
		}

		public string LabelSave { get => E.T("save"); }
		public string LabelCancel { get => E.T("cancel"); }
		public string LabelTopic { get => E.T("topic"); }

		#endregion

		#region Commands

		public ICommand SaveCommand
		{
			get
			{
				return new Command(async () =>
				{
					var vLoc = $"{TYPE_NAME}::{nameof(SaveCommand)}";

					try
					{
						if(string.IsNullOrEmpty(_Item.Topic.Trim()))
						{
							await Application.Current.MainPage.DisplayAlert(
								vLoc,
								E.T("topicNotEmpty"),
								E.T("ok"));
							return;
						}

						IsBusy = true;

						HttpResponseMessage response;
						if (_Item.Id > 0)
						{
							response = await _ApiServices.DeviceTopicPutAsync(_Item);
						}
						else
						{
							response = await _ApiServices.DeviceTopicPostAsync(_Item);
						}
#if PROCESS_RESPONSES
						E.ProcessResponse(response);
#endif
						await Application.Current.MainPage.Navigation.PopAsync();

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

		#endregion
	}
}

