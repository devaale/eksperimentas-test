using Experiment.Core.Metadata;
using Experiment.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Service
{
	internal class Program
	{
		#region Const

		#endregion

		#region Attributes
		/// <summary>
		/// Relevant only for Console app
		/// </summary>
		static bool WaitForKeyInput = false;

		#endregion

		#region Properties
		internal static ExperimentService _Service = null;

		#endregion

		#region Methods

		static void Main(string[] args)
		{
			if (Environment.UserInteractive)
			{
				// running as console app
				Start(args);

				if (WaitForKeyInput)
				{
					Console.WriteLine("Press any key to stop...");
					Console.ReadKey(true);
					Stop();
				}
			}
			else
			{
				// running as service
				using (var service = new ExperimentService())
				{
					ExperimentService.Run(service);
				}
			}

		}

		/// <summary>
		/// Method starting the service in console mode
		/// </summary>
		/// <param name="args"></param>
		static void Start(string[] args)
		{
			if (_Service == null)
			{
				_Service = new ExperimentService(new IServiceBase[]
				{
					//new ControlService(),
					new PurgeUserService(),
				});
			}
			_Service.Start(args);
		}

		/// <summary>
		/// Method stopping the service in console mode
		/// </summary>
		internal static void Stop()
		{
			if (_Service != null)
			{
				_Service.Stop();
			}
		}

		#endregion
	}
}
