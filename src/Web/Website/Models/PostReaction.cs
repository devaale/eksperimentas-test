using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;
using Website.Models;

namespace Website.Models
{
	[Table("tblPostReaction")]
	public class PostReaction : IPostReaction
	{
		/// <summary>
		/// Primary key
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// Post Id
		/// </summary>
		[Required]
		public int PostId { get; set; }

		/// <summary>
		/// Post referende regarding PostId
		/// </summary>
		[ForeignKey(nameof(PostId))]
		public Post Post { get; set; }

		[Required]
		[StringLength(128)]
		public string UserId { get; set; }

		// How need to declare it but then it starting to annoy with plenty of incorrect structures of Asp.Net tables, so better to comment it.
		[ForeignKey(nameof(UserId))]
		public virtual ApplicationUser User { get; set; }

		/// <summary>
		/// 0 None
		/// 1 Like
		/// </summary>
		public PostReactionType Reaction { get; set; }
	}
}
