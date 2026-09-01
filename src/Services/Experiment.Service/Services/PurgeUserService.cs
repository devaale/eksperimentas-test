using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Experiment.Core.Enums;
using Experiment.Core.Data;
using Experiment.Core;
using static System.Net.Mime.MediaTypeNames;

namespace Experiment.Service.Services{
	internal class PurgeUserService : ServiceBase
	{
		#region Properties
		public override string Name { get => nameof(PurgeUserService); }

		#endregion

		#region Helpers
		static DateTime Next (DateTime now)
		{
			if (now == null)
				throw new ArgumentNullException(nameof(now));

			var next = now.AddHours(1);
			var retVal = new DateTime(next.Year, next.Month, next.Day, next.Hour, 32, 16);
			return retVal;
		}

		#endregion

		#region Methods
		public override void Heartbeat()
		{
			var vLoc = string.Format("{0}::{1}()", Name, nameof(Heartbeat));
			Logger.WriteLine(3, string.Format("{0}, Starting...", vLoc));

			var next = Next(DateTime.Now.AddHours(-2));

			while (State < ServiceState.StopRequested)
			{
				Logger.WriteLine(7, string.Format("{0}, Heartbeat cycle...", vLoc));

				var now = DateTime.Now;
				if (next < now)
				{
					Logger.WriteLine(4, string.Format("{0}, {1}.{2}()...", 
						vLoc, nameof(Db), nameof(Db.ExpPurgeUsersData)));

					// Reset error state
					Db.Error();

					// Execute DB query
					Db.ExpPurgeUsersData();

					// Check and log errog if happened
					if(Db.IsError)
					{
						Logger.WriteLine(0, Db.ErrorMsg);
					}

					next = Next(now);
					Logger.WriteLine(5, string.Format("{0}, Next purge after {1}", vLoc, next.ToString(Defaults.DEFAULT_DATETIME_FORMAT)));
				}

				// Sleep for 10 secs
				Thread.Sleep(10 * 1000);
			}

			Logger.WriteLine(3, string.Format("{0}, Ending...", vLoc));
			State = ServiceState.Stopped;
		}

		#endregion
	}
}
