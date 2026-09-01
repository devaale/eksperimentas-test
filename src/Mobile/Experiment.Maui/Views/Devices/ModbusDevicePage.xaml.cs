using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Experiment.Maui.Views.Devices{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ModbusDevicePage : ContentPage
	{
		public ModbusDevicePage()
		{
			InitializeComponent();

			// ML Init
			Title = E.T("new-device");
		}
	}
}
