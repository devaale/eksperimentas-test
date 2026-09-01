using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface INamedDbItem<T>
	{
		T Id { get; set; }
		string Name { get; set; }
	}
}
