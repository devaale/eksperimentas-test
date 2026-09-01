using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class Product : IProduct
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
	}
}
