using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Models;

namespace Experiment.Data.Metadata{
	internal interface IUserInfo : IUser
	{
		/// <summary>
		/// User's licenses. Must check dates.
		/// </summary>
		IEnumerable<License> Licenses { get; }
	}
}
