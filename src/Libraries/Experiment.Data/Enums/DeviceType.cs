using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Enums{
	public enum DeviceType : int
	{
		Uncategorized = 1,
#warning TODO: Put here real types or something else to implement.
		ATGM = 10,
		FPGM = 20,
		Drone = 30,
		TacticalDrone = 40,
		MLRS = 50,
		ShortRangeArtillery = 60,
		Howtizers = 70,
		AlienDevice = 1000,
	}
}
