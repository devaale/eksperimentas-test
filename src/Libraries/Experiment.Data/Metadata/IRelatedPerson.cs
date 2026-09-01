using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IRelatedPerson
	{
		int Id { get; set; }
		string UserId { get; set; }
		string RelatedUserId { get; set; }
		string Name { get; set; }
	}
}
