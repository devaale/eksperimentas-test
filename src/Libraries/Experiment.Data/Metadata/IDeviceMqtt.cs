using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Core.Metadata;

namespace Experiment.Data.Metadata{
	public interface IDeviceMqtt : IDbItem
	{
		string Url { get; set; }
		string Username { get; set; }
		string Password { get; set; }
		//string Topic { get; set; }	// 2024-05-02-ag removed, moved to tblDeviceTopic
		int Interval { get; set; }
		DateTime? LastScanTime { get; set; }
		DateTime ProjectedScanTime { get; set; }
		List<string> Topics { get; set; }
	}
}
