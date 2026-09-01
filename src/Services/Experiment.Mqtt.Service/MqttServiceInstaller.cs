using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Experiment.Mqtt.Service{
	[RunInstaller(true)]
	public partial class MqttServiceInstaller : System.Configuration.Install.Installer
	{
		public MqttServiceInstaller()
		{
			InitializeComponent();

			var spi = new ServiceProcessInstaller();
			var si = new ServiceInstaller();

			spi.Account = ServiceAccount.LocalSystem;
			spi.Username = null;
			spi.Password = null;

			si.DisplayName = Program.ServiceName;
			si.ServiceName = Program.ServiceName;
			si.StartType = ServiceStartMode.Automatic;

			Installers.Add(spi);
			Installers.Add(si);
		}
	}
}
