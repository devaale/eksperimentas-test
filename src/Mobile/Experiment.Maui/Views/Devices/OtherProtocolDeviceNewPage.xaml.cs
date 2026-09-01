using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Experiment.Maui.Views.Devices{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OtherProtocolDeviceNewPage : ContentPage
    {
        public OtherProtocolDeviceNewPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // ML Init
            Title = E.T("new-device");

            CmbProtocol.Title = E.T("protocol");

            LblName.Text = E.T("name");
            TxtName.Placeholder = E.T("name");

            LblDescription.Text = E.T("description");
            TxtDescription.Placeholder = E.T("description");

            //LblClientId.Text = E.T("client-id");
            //TxtClientId.Placeholder = E.T("client-id");

            LblTopic.Text = E.T("topic");
            TxtTopic.Placeholder = E.T("topic");

			LblDeprGL.Text = E.T("deprGL");
			TxtDeprGL.Placeholder = E.T("deprGL");

			LblDeprA.Text = E.T("deprA");
			TxtDeprA.Placeholder = E.T("deprA");

			LblDeprLIR.Text = E.T("deprLIR");
			TxtDeprLIR.Placeholder = E.T("deprLIR");

			LblDeprRL.Text = E.T("deprRL");
			TxtDeprRL.Placeholder = E.T("deprRL");

			LblDeprC.Text = E.T("deprC");
			TxtDeprC.Placeholder = E.T("deprC");

			LblDeprSD.Text = E.T("deprSD");
			TxtDeprSD.Placeholder = E.T("deprSD");

			CmdCancel.Text = E.T("cancel");
        }
    }
}

