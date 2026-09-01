using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class Word
	{
		public string Alias { get; set; }

		public string Code { get; set; }

		public string Text { get; set; }

		public bool Autoadded { get; set; }
	}
}