using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

using Experiment.Data.Metadata;

namespace Website.Models
{
	[Table("tblPostImage")]
	public class PostImage : IPostImage
	{
		/// <summary>
		/// Primary key
		/// </summary>
		[Key]
		// Not helping, better works to assign it in constructor
		//[DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
		public Guid Id { get; set; }

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

		/// <summary>
		/// File content type/META
		/// </summary>
		[StringLength(128)]
		public string ContentType { get; set; }

		/// <summary>
		/// File name without path
		/// </summary>
		[StringLength(256)]
		public string Name { get; set; }

		/// <summary>
		/// Internet accessible image URL for front-end
		/// </summary>
		[Required]
		public string RawName { get; set; }

		public PostImage()
		{
			Id = Guid.NewGuid();
		}
	}
}
