using System.Collections.Generic;

using Experiment.Data.Models;

namespace Experiment.FormulaCalculation.Service
{
	internal interface IDatapointProcessor
	{
		void Process(IEnumerable<Datapoint> datapoints);
	}
}
