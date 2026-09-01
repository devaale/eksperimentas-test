using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace Experiment.DeviceProcessing.Service{
	[RunInstaller(true)]
	public partial class ServiceInstaller : System.Configuration.Install.Installer
	{
		public ServiceInstaller()
		{
			InitializeComponent();

			var spi = new ServiceProcessInstaller();
			var si = new System.ServiceProcess.ServiceInstaller();

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
