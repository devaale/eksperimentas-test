using System.Collections.Generic;

using Experiment.Data.Models;

namespace Experiment.FormulaCalculation.Service
{
	internal interface IDatapointProvider
	{
		List<Datapoint> GetDatapoints();
	}
}
