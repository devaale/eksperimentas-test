using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Experiment.Data.Enums;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels.Devices;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.ViewModels.Devices;
using Experiment.Maui.Models;

/*
 More info about XAML grouping, I had issues with it:
	https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/listview/customizing-list-appearance

	GroupDisplayBinding="{Binding TypeDesc}"
	GroupShortNameBinding="{Binding TypeName}"
				VerticalOptions="CenterAndExpand" 
                HorizontalOptions="CenterAndExpand"

 
 */
namespace Experiment.Maui.Views.Devices{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DatapointsListPage : ContentPage
	{
#warning @TODO: Very old code with wrong implementation
		#region Attributes
		VisualDevice _Device;
		DatapointViewModel _Datapoint;

		#endregion

		#region Properties

		#endregion

		#region Ctor
		public DatapointsListPage(VisualDevice device)
		{
			InitializeComponent();
			/*
			if (device == null && device.Id < 1)
				throw new ArgumentException(E.T("nothing-selected"));
			*/
			_Device = device;

			this.BindingContext = new VM.DatapointsListViewModel(device);
		}

		#endregion

		#region Overrides
		protected override async void OnAppearing()
		{
			base.OnAppearing();

			// ML Init
			Title = E.T("datapoints");
			CmdCancel.Text = E.T("cancel");

			if(BindingContext != null && BindingContext is DatapointsListViewModel)
			{
				await ((DatapointsListViewModel)BindingContext).LoadAsync(this);
			}
		}

		private async void LvwDatapoints_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			var datapoint = LvwList.SelectedItem;
			if (datapoint != null && datapoint is DatapointViewModel)
			{
				var dvm = datapoint as DatapointViewModel;
				if(dvm.DatapointType == DatapointType.Virtual)
				{
					try
					{
						await Navigation.PushAsync(new VirtualDatapointPage()
						{
							BindingContext = dvm,
						});
					}
					catch(Exception ex)
					{
						Debug.WriteLine(ex.Message);
					}
				}
				else
				{
					switch (_Device.Protocol)
					{
						case DeviceProtocol.API:
							await Navigation.PushAsync(new ApiDatapointPage()
							{
								BindingContext = datapoint,
							});
							break;

						case DeviceProtocol.Modbus:
							await Navigation.PushAsync(new ModbusDatapointPage()
							{
								BindingContext = datapoint,
							});
							break;

						case DeviceProtocol.BACnet:
						case DeviceProtocol.MQTT:
						case DeviceProtocol.CoAP:
						case DeviceProtocol.OpenThread:
							await Navigation.PushAsync(new OtherProtocolDatapointPage()
							{
								BindingContext = datapoint,
							});
							Console.WriteLine("Not supported");
							break;

						default:
							Console.WriteLine("Not supported");
							break;
					}
				}

				// Cleanup of selection that selection again worked 
				if (sender is ListView)
					((ListView)sender).SelectedItem = null;
			}
		}

		private async void CmdCancel_Clicked(object sender, EventArgs e)
		{
			await Navigation.PopAsync();
		}

		#endregion

		#region Delegates

		#endregion

		#region Helpers

		#endregion
	}
}
