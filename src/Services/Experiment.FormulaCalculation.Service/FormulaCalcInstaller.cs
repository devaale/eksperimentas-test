using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Experiment.FormulaCalculation.Service{
	[RunInstaller(true)]
	public partial class FormulaCalcInstaller : System.Configuration.Install.Installer
	{
		public FormulaCalcInstaller()
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
