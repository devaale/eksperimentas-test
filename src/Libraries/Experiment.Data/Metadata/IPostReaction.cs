using Experiment.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IPostReaction
	{
		int Id { get; set; }
		int PostId { get; set; }
		string UserId { get; set; }
		PostReactionType Reaction { get; set; }
	}
}
