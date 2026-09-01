using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using Experiment.Data.Metadata;

namespace Website.Models
{
	[Table("tblFriend")]
	public class Friend : IRelatedPerson
	{
		/// <summary>
		/// Id of the record
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// Subject User's Id
		/// </summary>
		[Required]
		[StringLength(128)]
		public string UserId { get; set; }

		/// <summary>
		/// Subject's User
		/// </summary>
		//[ForeignKey(nameof(UserId))]
		//public virtual ApplicationUser User { get; set; }

		/// <summary>
		/// Friend's UserId
		/// </summary>
		[Required]
		[StringLength(128)]
		public string RelatedUserId { get; set; }

		/// <summary>
		/// Friend User
		/// </summary>
		//[ForeignKey(nameof(RelatedUserId))]
		//public virtual ApplicationUser RelatedUser { get; set; }

		/// <summary>
		/// Name of friend 
		/// 
		/// Computed via SQL: [dbo].[fncUsernameById]([RelatedUserId])
		/// </summary>
		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public string Name { get; set; }
	}
}