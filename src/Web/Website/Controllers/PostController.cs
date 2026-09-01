using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

using Microsoft.AspNet.Identity;

using Experiment.Core;
using Experiment.Data.Enums;
using Experiment.Data.Metadata;
using Experiment.Data.Drawing;
using M = Experiment.Data.Models;

using Website.Data;
using Website.Models;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/Post")]
	public class PostController : ApiController
	{
		const string TYPE_NAME = nameof(PostController);
		private ApplicationDbContext db = new ApplicationDbContext();

		// GET: api/Post
		[ResponseType(typeof(M.Post[]))]
		public M.Post[] GetPosts(PostFeedType feed, DateTime? sinceDate)
		{
			DateTime date = (sinceDate.HasValue ? sinceDate.Value : DateTime.Now);

			var sql = "EXEC prcPostList @userId, @feed, @date";
			var rawResult = db.Database.SqlQuery<M.Post>(
				sql, 
				new SqlParameter("@userId", User.Identity.GetUserId()),
				new SqlParameter("@feed", feed),
				new SqlParameter("@date", date));
			var result = rawResult.ToArray();
			return result;
		}

		// GET: api/Post/5
		[ResponseType(typeof(Post))]
		public IHttpActionResult GetPost(int id)
		{
			Post post = db.Posts.Find(id);
			db.Entry(post).Collection(nameof(Post.Images)).Load();

			if (post == null)
			{
				return NotFound();
			}

			return Ok(post);
		}

		/*
		// PUT: api/Post/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutPost(int id, Post post)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != post.Id)
			{
				return BadRequest();
			}

			// Check that user updated only own posts
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(post.UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.Entry(post).State = EntityState.Modified;

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!PostExists(id))
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
		*/

		// POST: api/Post
		[ResponseType(typeof(Post))]
		public IHttpActionResult PostPost(M.PostNew postNew)
		{
			var vLoc = string.Format("{0}::{1}(M.PostNew postNew)",
				TYPE_NAME, nameof(PostPost));
			Debug.WriteLine(vLoc + "...");

			// Forming EF post
			var post = new Post()
			{
				UserId = User.Identity.GetUserId(),
				Body = postNew.Body,
				Audience = postNew.Audience,
				Images = new List<PostImage>(),
			};

			// Processing its images
			if(postNew.Images != null)
			{
				foreach(var img in postNew.Images)
				{
					// Genrating random Guid and formatting it without signs
					var guid = Guid.NewGuid().ToString("N");

					// Retrieving file extension
					var ext = Path.GetExtension(img.Name);

					// File name
					var fileName = guid + ext;
					Debug.WriteLine(vLoc + ", fileName: " + fileName);

					// Generating new file path
					var path = Path.Combine(WebsiteDefaults.GetOriginalFilesPath(), fileName);
					Debug.WriteLine(vLoc + ", path: " + path);

					// Writting the file to the disk (make sure about file permissions)
					ImageProcessor proc = new ImageProcessor(
						WebsiteDefaults.ImageProcessingSettings);
					proc.Process(fileName, img.Data);
					//File.WriteAllBytes(path, img.Data);

					post.Images.Add(new PostImage()
					{
						ContentType = img.ContentType,
						Name = img.Name,
						RawName = fileName,
					});
				}
			}

			//ModelState.Clear();
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			db.Posts.Add(post);
			var response = db.SaveChanges();

			return CreatedAtRoute("DefaultApi", new { id = post.Id }, post);
		}

		/*
		// DELETE: api/Post/5
		[ResponseType(typeof(Post))]
		public IHttpActionResult DeletePost(int id)
		{
			Post post = db.Posts.Find(id);
			if (post == null)
			{
				return NotFound();
			}

			// User can delete only own posts
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(post.UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.Posts.Remove(post);
			db.SaveChanges();

			return Ok(post);
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

		private bool PostExists(int id)
		{
			return db.Posts.Count(e => e.Id == id) > 0;
		}
	}
}