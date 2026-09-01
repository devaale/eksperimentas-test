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
	/// Group
	/// </summary>
	[Table("tblGroup")]
	public class Group : IGroup
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(256)]
		public string Name { get; set; }

		public int ObjectId { get; set; }

		public DateTime? Deleted { get; set; }

		public ICollection<GroupDatapoint> GroupDatapoints { get; set; }

		public void Update(ApplicationDbContext db, M.Group grp)
        {
			Name = grp.Name;

			// Collecting datapoints for purge
			var purge = new List<GroupDatapoint>();
			foreach (var grpDtp in GroupDatapoints)
			{
				// If specific datapoint already not exists in collection, need to remove it
				if (grp.Datapoints.FirstOrDefault(f => f.Id.Equals(grpDtp.DatapointId) && f.Selected == true) == null)
					purge.Add(grpDtp);
			}
			
			// Now removing them
			// Based on https://stackoverflow.com/a/24940051
			foreach (var grpDtp in purge)
			{
				db.GroupDatapoints.Remove(grpDtp);
			}
			
			// After what adding new ones, which unavailable yet in DB
			foreach (var grpDtp in grp.Datapoints)
			{
				if (grpDtp.Selected)
				{
					// If it not found or still not added
					if (GroupDatapoints.FirstOrDefault(p => p.GroupId.Equals(grpDtp.Id)) == null)
					{
						// Adding it
						db.GroupDatapoints.Add(new GroupDatapoint()
						{
							GroupId = Id,
							DatapointId = grpDtp.Id,
						});
					}
				}
			}
			
		}

        public static M.Group ToFrontendGroup(
            int objectId, Group grp, IQueryable<Datapoint> datapoints, IQueryable<GroupDatapoint> groupDatapoints)
        {
            if (grp == null)
                throw new ArgumentNullException("grp");

            var result = new M.Group()
            {
                Id = grp.Id,
                Name = grp.Name,
                ObjectId = objectId,
                Datapoints = new List<M.DatapointSelection>(),
                Editable = true,
                //Editable = userId.Equals(grp.UserId),
            };

			foreach (var datapoint in datapoints)
            {
                bool selected = false;

				foreach(var grpDtp in groupDatapoints)
				{
					if (grp.Id == grpDtp.GroupId && datapoint.Id == grpDtp.DatapointId)
						selected = true;
				}

                //selected = grp.GroupDatapoints.LastOrDefault(gd => gd.DatapointId == datapoint.Id) != null;
                //selected = true;

				result.Datapoints.Add(new M.DatapointSelection()
                {
                    Id = datapoint.Id,
                    Name = datapoint.Name,
                    Selected = selected,
                });
            }
            
            return result;
        }
    }
}