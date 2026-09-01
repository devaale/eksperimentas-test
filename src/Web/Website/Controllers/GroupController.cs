//#define REAL_DELETE // DON'T Enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

using Microsoft.AspNet.Identity;

using M = Experiment.Data.Models;

using Experiment.Data.Metadata;

using Website.Data;
using Website.Models;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/Group")]
	public class GroupController : ApiController
	{

		#region Attributes
		private ApplicationDbContext db = new ApplicationDbContext();

		#endregion

		#region Helpers

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool GroupExists(int id)
		{
			return db.Groups.Count(e => e.Id == id) > 0;
		}

		#endregion

		#region Methods

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		// GET: api/Group
		[ResponseType(typeof(IGroup[]))]
		public IGroup[] GetGroups(int objectId)
		{
			// Filter only specific objects's groups
			var result = db.Groups.Where(
				grp => (
					grp.ObjectId == objectId) &&
					grp.Deleted == null);

			var datapoints = (from dtp in db.Datapoints
				join dev in db.Devices
				on dtp.DeviceId equals dev.Id
				join obj in db.Objects
				on dev.ObjectId equals obj.Id
				where obj.Id == objectId &&
				dtp.ReadWrite == 1
				select dtp);

			var groupDatapoints = db.GroupDatapoints;

			// Converting to safe groups
			var retVal = new List<M.Group>();
			foreach (var grp in result)
			{
				retVal.Add(Group.ToFrontendGroup(objectId, grp, datapoints, groupDatapoints));
			}
			
			Debug.WriteLine(JsonConvert.SerializeObject(retVal));
			return retVal.ToArray();
		}

		// GET: api/Group/5
		[Route("New")]
		[ResponseType(typeof(IGroup))]
		public IGroup GetNewGroup(int objectId)
		{
			var datapoints = (from dtp in db.Datapoints
				join dev in db.Devices
				on dtp.DeviceId equals dev.Id
				join obj in db.Objects
				on dev.ObjectId equals obj.Id
				where obj.Id == objectId &&
				dtp.ReadWrite == 1
				select dtp);

			var groupDatapoints = db.GroupDatapoints;

			return Group.ToFrontendGroup(objectId, new Group()
			{
				ObjectId = objectId,
			}, datapoints, groupDatapoints);
		}

		// Edit
		// PUT: api/Group/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutGroup(int id, M.Group frontGrp)
		{
			// Find object
			Group grp = db.Groups.Find(id);

			// Load group datapoints, @see Explicitly Loading at https://learn.microsoft.com/en-us/ef/ef6/querying/related-data
			db.Entry(grp).Collection(nameof(Group.GroupDatapoints)).Load();

			// Update with data from frontend
			grp.Update(db, frontGrp);

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != grp.Id)
			{
				return BadRequest();
			}

			db.Entry(grp).State = EntityState.Modified;

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!GroupExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return StatusCode(HttpStatusCode.NoContent);
		}

		// Save new group
        // POST: api/Group
        [ResponseType(typeof(Group))]
        public IHttpActionResult PostGroup(M.Group group)
        {
			// Creating new Group
			var grp = new Group()
			{
				ObjectId = group.ObjectId,
				GroupDatapoints = new List<GroupDatapoint>(),
			};
			grp.Update(db, group);

			ModelState.Clear();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Groups.Add(grp);
            var response = db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = group.Id }, group);
        }

        // DELETE: api/Group/5
        [ResponseType(typeof(Group))]
		public IHttpActionResult DeleteGroup(int id)
		{
			Group grp = db.Groups.Find(id);
#if REAL_DELETE
			if (grp == null)
			{
				return NotFound();
			}

			// User can delete only own groups
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(grp.UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.Groups.Remove(grp);
			db.SaveChanges();

			return Ok(grp);
#else
            if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != grp.Id)
			{
				return BadRequest();
			}

			// Check that user updated only own groups
			var userId = User.Identity.GetUserId();

            // Mark as deleted
            grp.Deleted = DateTime.Now;

			db.Entry(grp).State = EntityState.Modified;

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!GroupExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return StatusCode(HttpStatusCode.NoContent);
#endif
		}

		#endregion
	}
}