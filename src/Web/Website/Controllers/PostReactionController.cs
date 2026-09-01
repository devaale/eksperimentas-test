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

using Microsoft.AspNet.Identity;

using M = Experiment.Data.Models;
using Experiment.Data.Metadata;

using Website.Data;
using Website.Models;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/PostReaction")]
	public class PostReactionController : ApiController
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		// POST: api/PostReaction
		[ResponseType(typeof(PostReaction))]
		public IHttpActionResult PostPostReaction(PostReaction postReaction)
		{
			// Check that user updated only own postReactions
			var userId = User.Identity.GetUserId();

			var found = db.PostReactions.Where(pr =>
				pr.PostId == postReaction.PostId &&
				pr.UserId.Equals(userId));

			PostReaction newPostReaction = new PostReaction();
			if(found.Count() > 0)
			{
				foreach(var item in found)
				{
					db.PostReactions.Remove(item);
				}
			}
			else
			{
				newPostReaction = new PostReaction()
				{
					PostId = postReaction.PostId,
					Reaction = postReaction.Reaction,
					UserId = userId,
				};

				db.PostReactions.Add(newPostReaction);
			}
			var response = db.SaveChanges();

			return CreatedAtRoute("DefaultApi", new { id = newPostReaction.Id }, newPostReaction);
		}

		/*
		// GET: api/PostReaction/5
		[ResponseType(typeof(PostReaction))]
		public IHttpActionResult GetPostReaction(int id)
		{
			PostReaction postReaction = db.PostReactions.Find(id);
			if (postReaction == null)
			{
				return NotFound();
			}

			var userId = User.Identity.GetUserId();
			if(!postReaction.UserId.Equals(userId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}
			return Ok(postReaction);
		}

		// PUT: api/PostReaction/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutPostReaction(int id, PostReaction postReaction)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != postReaction.Id)
			{
				return BadRequest();
			}

			// Check that user updated only own postReactions
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(postReaction.UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.Entry(postReaction).State = EntityState.Modified;

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!PostReactionExists(id))
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

		// POST: api/PostReaction
		[ResponseType(typeof(PostReaction))]
		public IHttpActionResult PostPostReaction(PostReaction postReaction)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Check that user updated only own postReactions
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(postReaction.UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.PostReactions.Add(postReaction);
			var response = db.SaveChanges();

			return CreatedAtRoute("DefaultApi", new { id = postReaction.Id }, postReaction);
		}

		// DELETE: api/PostReaction/5
		[ResponseType(typeof(PostReaction))]
		public IHttpActionResult DeletePostReaction(int id)
		{
			// Find postReaction
			PostReaction postReaction = db.PostReactions.Find(id);
			if (postReaction == null)
			{
				return NotFound();
			}

			// Check that user updated only own postReactions
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(postReaction.UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.PostReactions.Remove(postReaction);
			db.SaveChanges();

			return Ok(postReaction);
		}
		*/

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool PostReactionExists(int id)
		{
			return db.PostReactions.Count(e => e.Id == id) > 0;
		}
	}
}