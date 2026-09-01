using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IProduct
	{
		string Name { get; set; }
		string Description { get; set; }
		decimal Price { get; set; }
	}
}
