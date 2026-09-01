using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface ILanguage
	{
		string Code { get; set; }
		string Name { get; set; }
	}
}
