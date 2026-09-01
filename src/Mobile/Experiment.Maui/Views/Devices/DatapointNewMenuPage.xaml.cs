using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Experiment.Maui.Views.Devices{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DatapointNewMenuPage : ContentPage
	{
		public DatapointNewMenuPage()
		{
			InitializeComponent();

			Title = E.T("new-datapoint");
		}
	}
}
