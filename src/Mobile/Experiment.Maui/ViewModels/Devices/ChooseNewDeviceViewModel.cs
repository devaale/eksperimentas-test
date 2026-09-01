using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

using Microsoft.Maui.Controls;

using Experiment.Core.Base;
using Experiment.Data.Enums;

using Experiment.Maui.Models;
using Experiment.Maui.Views.Devices;

namespace Experiment.Maui.ViewModels.Devices{
	internal class ChooseNewDeviceViewModel : ViewModelBase
	{
		#region Attributes

		DeviceProtocol _Protocol;

		#endregion


		#region Properties
		internal DeviceProtocol Protocol
		{
			get => _Protocol;
			set => SetProperty(ref _Protocol, value);
		}

		public string LabelNext { get => E.T("next"); }
		public string LabelCancel { get => E.T("back"); }

		#endregion


		#region Commands

		/// <summary>
		/// Add device command, when user confirmed this dialogue and wish to go to the next dialogue
		/// </summary>
		public ICommand AddCommand
		{
			get
			{
				return new Command(async () =>
				{
					// Pop and remove current dialogue first
					await Application.Current.MainPage.Navigation.PopAsync();

					Page page = null;

					switch(Protocol)
					{
						case DeviceProtocol.BACnet:
						case DeviceProtocol.MQTT:
						case DeviceProtocol.CoAP:
						case DeviceProtocol.OpenThread:

							// 2023-12-19 OtherProtocolDeviceNewPage Deprecated!
							//page = new OtherProtocolDeviceNewPage()
							page = new OtherProtocolDeviceSettingsPage()
							{
								BindingContext = new DeviceViewModel()
								{
									Item = new VisualDevice()
									{
										Protocol = Protocol,
									},
								}
							};
							break;
					}

					// Open page, if we have such
					if(page != null)
					{
						await Application.Current.MainPage.Navigation.PushAsync(page);
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

		#endregion
	}
}

