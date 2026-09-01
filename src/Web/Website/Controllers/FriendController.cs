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
	[RoutePrefix("api/Friend")]
	public class FriendController : ApiController
	{
		const string TYPE_NAME = nameof(FriendController);
		private ApplicationDbContext db = new ApplicationDbContext();

		/// <summary>
		/// Returns all user's friends
		/// 
		/// GET: api/Friend
		/// </summary>
		/// <returns></returns>
		public IQueryable<Friend> GetFriends()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(GetFriends));

			// Filter only specific user's friends
			var userId = User.Identity.GetUserId();
			var retVal = db.Friends.Where(r =>
				r.UserId.Equals(userId) &&
				// Let's eliminate those, which records unavailable in AspNetUsers DB table
				db.Users.Any(u => u.Id.Equals(r.RelatedUserId)));

			return retVal;
		}

		// GET: api/Friend/5
		[ResponseType(typeof(Friend))]
		public IHttpActionResult GetFriend(int id)
		{
			Friend f = db.Friends.Find(id);
			if (f == null)
			{
				return NotFound();
			}

			return Ok(f);
		}

		// PUT: api/Friend/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutFriend(int id, Friend friend)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != friend.Id)
			{
				return BadRequest();
			}

			// Check that user updated only own friends
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(friend.UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.Entry(friend).State = EntityState.Modified;

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!FriendExists(id))
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

		// POST: api/Friend
		[ResponseType(typeof(Friend))]
		public IHttpActionResult PostFriend(Friend friend)
		{
			// Adding User Id
			var userId = User.Identity.GetUserId();
			friend.UserId = userId;

#if ENFORCE_MODEL
			ModelState.Clear();
#endif
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			db.Friends.Add(friend);
			var response = db.SaveChanges();

			return CreatedAtRoute("DefaultApi", new { id = friend.Id }, friend);
		}

		// DELETE: api/Friend/5
		[ResponseType(typeof(Friend))]
		public IHttpActionResult DeleteFriend(int id)
		{
			Friend f = db.Friends.Find(id);
			if (f == null)
			{
				return NotFound();
			}

			// User can delete only own friends
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(f.UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.Friends.Remove(f);
			db.SaveChanges();

			return Ok(f);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool FriendExists(int id)
		{
			return db.Friends.Count(e => e.Id == id) > 0;
		}
	}
}