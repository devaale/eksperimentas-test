using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

using M = Experiment.Data.Models;
using Experiment.Data.Metadata;

namespace Website.Models
{
	/// <summary>
	/// This is Object, but I had conflicts with System.Object what forced me to rename it to EObject.
	/// </summary>
	[Table("tblObject")]
	public class EObject : IObject
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(256)]
		public string Name { get; set; }

		[Required]
		[StringLength(128)]
		public string UserId { get; set; }

		public DateTime? Deleted { get; set; }

		[ForeignKey(nameof(ObjectPermission.ObjectId))]
		public ICollection<ObjectPermission> Permissions { get; set; }

		/*
		public void Update (ApplicationDbContext db, M.Object obj)
		{
			// Assigning name
			Name = obj.Name;

			// Collecting permissions for purge
			var purge = new List<ObjectPermission>();
			foreach(var perm in Permissions)
			{
				// If specific FriendUserId already not exists in friends collection, need to remove it
				if (obj.Friends.FirstOrDefault(f => f.RelatedUserId.Equals(perm.FriendUserId) && f.Selected == true) == null)
					purge.Add(perm);
			}

			// Now removing them
			// Based on https://stackoverflow.com/a/24940051
			foreach (var perm in purge)
			{
				db.ObjectPermissions.Remove(perm);
			}

			// After what adding new ones, which unavailable yet in DB
			foreach(var f in obj.Friends)
			{
				if(f.Selected)
				{
					// If it not found or still not added
					if (Permissions.FirstOrDefault(p => p.FriendUserId.Equals(f.RelatedUserId)) == null)
					{
						// adding it
						db.ObjectPermissions.Add(new ObjectPermission()
						{
							ObjectId = Id,
							FriendUserId = f.RelatedUserId,
						});
					}
				}
			}
		}

		public static IObject ToFrontendObject(
			string userId, EObject obj, IQueryable<Friend> friends)
		{
			if (string.IsNullOrEmpty(userId))
				throw new ArgumentNullException("userId");

			if (obj == null)
				throw new ArgumentNullException("obj");

			var result = new M.Object()
			{
				Id = obj.Id,
				Name = obj.Name,
				UserId = obj.UserId,
				Friends = new List<M.FriendSelection>(),
				IsOwnedObject = userId.Equals(obj.UserId),
			};

			var hasPermissions = obj.Permissions != null;
			foreach (var friend in friends)
			{
				bool selected = false;
				if(hasPermissions)
				{
					selected = obj.Permissions.LastOrDefault(op => op.FriendUserId == friend.RelatedUserId) != null;
				}

				result.Friends.Add(new M.FriendSelection()
				{
					Id = friend.Id,
					UserId = friend.UserId,
					RelatedUserId = friend.RelatedUserId,
					Name = friend.Name,
					Selected = selected,
				});
			}

			return result;
		}
		*/
	}
}