using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Experiment.Data.Enums;

using Experiment.Maui.Models;
using Experiment.Maui.ViewModels.Devices;

namespace Experiment.Maui.Views.Devices{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OpenThreadDevicePage : ContentPage
    {
        public OpenThreadDevicePage()
        {
            InitializeComponent();
		
            // ML Init
			Title = E.T("new-device");
			LblDescription.Text = E.T("server-address") + ": 88.216.164.51:20004";
		}

		// Events
		/// <summary>
        /// @deprecated
		/// OpenThread new devices button clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void CmdNewDevice_Clicked(object sender, EventArgs e)
        {
			await Navigation.PushAsync(new OtherProtocolDeviceNewPage()
            {
                BindingContext = new DeviceViewModel()
                {
                    Item = new VisualDevice()
                    {
						Protocol = DeviceProtocol.OpenThread,
					},
                }
            });
		}
    }
}
