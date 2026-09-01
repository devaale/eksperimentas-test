using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IItemOwner
	{
		void ItemChanged(string propertyName);
	}
}
