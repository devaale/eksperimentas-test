using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

using Experiment.Data.Metadata;

namespace Website.Models
{
	[Table("tblPost")]
	public class Post : IPost
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string UserId { get; set; }

		/// <summary>
		/// Subject's User
		/// </summary>
		[ForeignKey(nameof(UserId))]
		public virtual ApplicationUser User { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime Date { get; set; }

		[Required]
		public string Body { get; set; }

		[Required]
		public int Audience { get; set; }

		public ICollection<PostImage> Images { get; set; }
	}
}
