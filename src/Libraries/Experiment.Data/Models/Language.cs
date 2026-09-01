using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class Language : ILanguage
	{
		public string Code { get; set; }

		public string Name { get; set; }
	}
}