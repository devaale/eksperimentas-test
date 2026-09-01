using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

using M = Experiment.Data.Models;
using Experiment.Data.Metadata;

namespace Website.Models
{
	[Table("tblObjectPermission")]
	public class ObjectPermission : IObjectPermission
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int ObjectId { get; set; }

		//[ForeignKey(nameof(ObjectId))]
		//public virtual EObject Object { get; set; }

		[Required]
		[StringLength(128)]
		public string FriendUserId { get; set; }

		//[ForeignKey(nameof(FriendUserId))]
		//public virtual ApplicationUser Friend { get; set; }

		public bool PermWrite { get; set; }
		public bool PermDevice { get; set; }
		public bool PermAlgorithm { get; set; }
		public bool PermGroup { get; set; }
		public bool PermAlarm { get; set; }

		public static IObjectPermission ToFrontendObject(IObjectPermission subject)
		{
			if (subject == null)
				throw new ArgumentNullException("subject");

			var result = new M.ObjectPermission()
			{
				Id = subject.Id,
				ObjectId = subject.ObjectId,
				FriendUserId = subject.FriendUserId,
			};

			return result;
		}
	}
}
