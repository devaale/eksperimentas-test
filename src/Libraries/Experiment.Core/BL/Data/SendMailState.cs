using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Core.BL.Data{
	/// <summary>
	/// Class representation of tblSendMailStatus DB table
	/// </summary>
	public enum SendMailState : int
	{
		None = 0,
		Sent = 10,
		Error = 100,
	}
}
