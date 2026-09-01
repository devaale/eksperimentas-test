using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Experiment.Data.Metadata;
using Microsoft.AspNet.Identity;

using Website.Data;
using Website.Models;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/ObjectPermission")]
	public class ObjectPermissionController : ApiController
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

		private bool ObjectPermissionExists(int id)
		{
			return db.ObjectPermissions.Count(e => e.Id == id) > 0;
		}

		#endregion

		#region Methods

		/// <summary>
		/// It works but returning as well user's info
		/// </summary>
		/// <returns></returns>
		// GET: api/ObjectPermission
		public IList<IObjectPermission> GetObjectPermissions(int objectId)
		{
			// Taking not deleted, specific user objectPermissions
			var result = db.ObjectPermissions.Where(subj => subj.ObjectId == objectId);

			// Converting to safe objectPermissions
			var retVal = new List<IObjectPermission>();
			foreach (var o in result)
			{
				retVal.Add(ObjectPermission.ToFrontendObject(o));
			}
			return retVal;
		}

		// GET: api/ObjectPermission/5
		[ResponseType(typeof(ObjectPermission))]
		public IHttpActionResult GetObjectPermission(int objectPermissionId)
		{
			ObjectPermission obj = db.ObjectPermissions.Find(objectPermissionId);
			if (obj == null)
			{
				return NotFound();
			}

			return Ok(ObjectPermission.ToFrontendObject(obj));
		}

		// PUT: api/ObjectPermission/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutObjectPermission(int id, ObjectPermission perm)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != perm.Id)
			{
				return BadRequest();
			}

			// Check that user updated permissions only for own objects
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(db.Objects.Find(perm.ObjectId).UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.Entry(perm).State = EntityState.Modified;

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ObjectPermissionExists(id))
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

		// POST: api/ObjectPermission
		[ResponseType(typeof(ObjectPermission))]
		public IHttpActionResult PostObjectPermission(ObjectPermission obj)
		{
			// Adding User Id
			//var userId = User.Identity.GetUserId();

			ModelState.Clear();
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			db.ObjectPermissions.Add(obj);
			var response = db.SaveChanges();

			return CreatedAtRoute("DefaultApi", new { id = obj.Id }, obj);
		}

		// DELETE: api/ObjectPermission/5
		[ResponseType(typeof(ObjectPermission))]
		public IHttpActionResult DeleteObjectPermission(int id)
		{
			ObjectPermission obj = db.ObjectPermissions.Find(id);
			if (obj == null)
			{
				return NotFound();
			}

			// User can delete only own friends
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(db.Objects.Find(obj.ObjectId).UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.ObjectPermissions.Remove(obj);
			db.SaveChanges();

			return Ok(obj);
		}

		#endregion
	}
}