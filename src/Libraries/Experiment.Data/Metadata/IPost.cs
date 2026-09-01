using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IPost
	{
		int Id { get; set; }
		string UserId { get; set; }
		DateTime Date { get; set; }
		string Body { get; set; }
		int Audience { get; set; }
		//ICollection<IPostImage> Images { get; set; }
	}
}
