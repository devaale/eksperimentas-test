using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IAudience
	{
		int Id { get; set; }
		string Name { get; set; }
	}
}
