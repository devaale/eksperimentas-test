using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IDbItem
	{
		int Id { get; set; }
		string Name { get; set; }
		//string Description { get; set; }
	}
}
