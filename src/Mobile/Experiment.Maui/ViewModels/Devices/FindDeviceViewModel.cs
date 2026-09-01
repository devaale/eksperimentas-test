using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Experiment.Core.Base;
using Experiment.Data.Enums;

// MVVM
using M = Experiment.Maui.Models;
using Experiment.Maui.Views.Devices;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Services;

namespace Experiment.Maui.ViewModels.Devices{
    public class FindDeviceViewModel : ViewModelBase
    {
        const string TYPE_NAME = nameof(FindDeviceViewModel);

		public string LabelDevice { get => E.T("find-device"); }
		public string LabelDeviceDesc { get => E.T("find-device-description"); }

		public FindDeviceViewModel()
        {
        }

		/// <summary>
		/// New Device Command
		/// 
		/// This command supposed to work in two modes:
		/// 
		///		1. when param is not null and it is DeviceProtocol,
		///			it was started from FindDevicePage.
		///		
		///		2. when param is null, it was started from 
		/// </summary>
		public ICommand NewDeviceCommand
		{
			get
			{
				return new Command(async (param) =>
				{
					if (param is DeviceProtocol)
					{
						// Pop up current view, if user already selected something
						await Application.Current.MainPage.Navigation.PopAsync();

						switch ((DeviceProtocol)param)
						{
							case DeviceProtocol.Modbus:

								await Application.Current.MainPage.Navigation.PushAsync(
									new ModbusDevicePage()
									{
										BindingContext = new DeviceViewModel()
										{
											Item = new M.VisualDevice()
											{
												Protocol = DeviceProtocol.Modbus,
											},
										}
									});
								break;

							case DeviceProtocol.BACnet:

								await Application.Current.MainPage.Navigation.PushAsync(
									new BACnetDevicePage()
									{
										BindingContext = new ChooseNewDeviceViewModel()
										{
											Protocol = DeviceProtocol.BACnet,
										}
									});
								break;

							case DeviceProtocol.MQTT:

								await Application.Current.MainPage.Navigation.PushAsync(
									new MqttDevicePage()
									{
										BindingContext = new ChooseNewDeviceViewModel()
										{
											Protocol = DeviceProtocol.MQTT,
										}
									});
								break;

							case DeviceProtocol.CoAP:

								await Application.Current.MainPage.Navigation.PushAsync(
									new CoAPDevicePage()
									{
										BindingContext = new ChooseNewDeviceViewModel()
										{
											Protocol = DeviceProtocol.CoAP,
										}
									});
								break;

							case DeviceProtocol.OpenThread:

								await Application.Current.MainPage.Navigation.PushAsync(
									new OpenThreadDevicePage()
									{
										BindingContext = new ChooseNewDeviceViewModel()
										{
											Protocol = DeviceProtocol.OpenThread,
										}
									});
								break;

							default:
								break;
						}
					}
                    
				});
			}
		}
	}
}

