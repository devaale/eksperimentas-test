//#define ENFORCE_MODEL

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

using Website.Data;
using Website.Models;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/Blocked")]
	public class BlockedController : ApiController
	{
		const string TYPE_NAME = nameof(BlockedController);
		private ApplicationDbContext db = new ApplicationDbContext();

		/// <summary>
		/// Returns all user's blocked
		/// 
		/// GET: api/Blocked
		/// </summary>
		/// <returns></returns>
		public IQueryable<Blocked> GetBlocked()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(GetBlocked));
			
			// Filter only specific user's blocked
			var userId = User.Identity.GetUserId();
			var retVal = db.Blocked.Where(r =>
				r.UserId == userId &&
				// Let's eliminate those, which records unavailable in AspNetUsers DB table
				db.Users.Any(u => u.Id.Equals(r.RelatedUserId)));

			return retVal;
		}

		// GET: api/Blocked/5
		[ResponseType(typeof(Blocked))]
		public IHttpActionResult GetBlocked(int id)
		{
			Blocked b = db.Blocked.Find(id);
			if (b == null)
			{
				return NotFound();
			}

			return Ok(b);
		}

		// PUT: api/Blocked/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutBlocked(int id, Blocked blocked)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != blocked.Id)
			{
				return BadRequest();
			}

			// Check that user updated only own blocked
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(blocked.UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.Entry(blocked).State = EntityState.Modified;

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!BlockedExists(id))
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

		// POST: api/Blocked
		[ResponseType(typeof(Blocked))]
		public IHttpActionResult PostBlocked(Blocked blocked)
		{
			// Adding User Id
			var userId = User.Identity.GetUserId();
			blocked.UserId = userId;

#if ENFORCE_MODEL
			ModelState.Clear();
#endif
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			db.Blocked.Add(blocked);
			var response = db.SaveChanges();

			return CreatedAtRoute("DefaultApi", new { id = blocked.Id }, blocked);
		}

		// DELETE: api/Blocked/5
		[ResponseType(typeof(Blocked))]
		public IHttpActionResult DeleteBlocked(int id)
		{
			Blocked b = db.Blocked.Find(id);
			if (b == null)
			{
				return NotFound();
			}

			// User can delete only own blocked
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(b.UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.Blocked.Remove(b);
			db.SaveChanges();

			return Ok(b);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool BlockedExists(int id)
		{
			return db.Blocked.Count(e => e.Id == id) > 0;
		}
	}
}