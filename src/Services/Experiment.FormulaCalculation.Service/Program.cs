//#define CONSOLE_WE_NEED_STOP // not in this case, but possible scenario
using System;
using System.ServiceProcess;

namespace Experiment.FormulaCalculation.Service{
	/// <summary>
	/// Application which can start as Windows Service as well as Console application
	/// Solution was borrowed from this post: https://stackoverflow.com/a/41783380
	/// 
	/// An useful info as well here: 
	/// https://docs.microsoft.com/en-us/dotnet/framework/windows-services/walkthrough-creating-a-windows-service-application-in-the-component-designer
	/// </summary>
	class Program
	{
		/// <summary>
		/// Getting current class assembly short name
		/// </summary>
		internal static string ServiceName { get { return typeof(Program).Assembly.GetName().Name; } }
		internal static FormulaCalculationService _Service = null;

		static void Main(string[] args)
		{
			if (Environment.UserInteractive)
			{
				// running as console app
				Start(args);

#if CONSOLE_WE_NEED_STOP
                Console.WriteLine("Press any key to stop...");
                Console.ReadKey(true);
                Stop();
#endif
			}
			else
			{
				// running as service
				using (var service = new FormulaCalculationService())
				{
					ServiceBase.Run(service);
				}
			}
		}

		/// <summary>
		/// Method starting the service in console mode
		/// </summary>
		/// <param name="args"></param>
		internal static void Start(string[] args)
		{
			_Service = new FormulaCalculationService();
			_Service.Start();
		}

		/// <summary>
		/// Method stopping the service in console mode, what is not needed at the moment
		/// </summary>
		internal static void Stop()
		{
			// sort of stop
		}
	}
}
