using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class PostReaction : IPostReaction
	{
		public int Id { get; set; }
		public int PostId { get; set; }
		public string UserId { get; set; }
		public PostReactionType Reaction { get; set; }
	}
}
