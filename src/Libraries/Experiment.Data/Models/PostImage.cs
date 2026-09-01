using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class PostImage : IPostImage
	{
		public Guid Id { get; set; }
		public int PostId { get; set; }
		public string ContentType { get; set; }
		public string Name { get; set; }
		public string RawName { get; set; }
	}
}
