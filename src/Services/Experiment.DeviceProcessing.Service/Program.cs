#define CONSOLE_STOP_ENABLED // enable press-any-key stop for console runs
using System;
using System.ServiceProcess;

namespace Experiment.DeviceProcessing.Service{
	internal static class Program
	{
		internal static string ServiceName => typeof(Program).Assembly.GetName().Name;

		private static DeviceProcessingService _service;

		private static void Main(string[] args)
		{
			if (Environment.UserInteractive)
			{
				RunConsole(args);
				return;
			}

			using (var service = new DeviceProcessingService())
			{
				ServiceBase.Run(service);
			}
		}

		private static void RunConsole(string[] args)
		{
			_service = new DeviceProcessingService();
			_service.StartInteractive(args);

#if CONSOLE_STOP_ENABLED
			Console.WriteLine("Press any key to stop...");
			Console.ReadKey(true);
			_service.StopInteractive();
#endif
		}
	}
}
