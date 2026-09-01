using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class Object : IObject
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string UserId { get; set; }

		public DateTime? Deleted { get; set; }

		//public ICollection<FriendSelection> Friends { get; set; }
		public ICollection<ObjectPermission> Permissions { get; set; }

		public bool IsNewObject { get => Id == 0; }
		public bool IsOwnedObject { get; set; }
	}
}