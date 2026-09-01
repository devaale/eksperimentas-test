#define ERROR_FILE_LOGGING

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

using Microsoft.AspNet.Identity;

using Experiment.Core.IO;

using Website.Data;
using Website.Enums;
using Website.Models;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/PostImage")]
	public class PostImageController : ApiController
	{
		const string TYPE_NAME = nameof(PostImageController);
		private ApplicationDbContext db = new ApplicationDbContext();

		// GET: api/PostImage
		[Route("All")]
		public IQueryable<PostImage> GetPostImages(Guid postId)
		{
			// Filter only specific user's postImages
			var userId = User.Identity.GetUserId();

			return db.PostImages.Where(postImage => 
					postImage.Id == postId && 
					userId.Equals(postImage.Post.UserId));
		}

		// GET: api/PostImage/5
		[ResponseType(typeof(PostImage))]
		public IHttpActionResult GetPostImage(Guid id)
		{
			PostImage postImage = db.PostImages.Find(id);

			if (postImage == null)
			{
				return NotFound();
			}

			return Ok(postImage);
		}

		// PUT: api/PostImage/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutPostImage(Guid id, PostImage postImage)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Not null
			bool valid = id != null && postImage != null;
			// If not null both
			if(valid)
			{
				// Ids match?
				valid = id.Equals(postImage.Id);
			}

			// Invalid?
			if(!valid)
			{
				return BadRequest();
			}

			// Check that user updated only own postImages
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(postImage.Post.UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.Entry(postImage).State = EntityState.Modified;

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!PostImageExists(id))
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

		// POST: api/PostImage
		[ResponseType(typeof(PostImage))]
		public IHttpActionResult PostImagePostImage(PostImage postImage)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			db.PostImages.Add(postImage);
			var response = db.SaveChanges();

			return CreatedAtRoute("DefaultApi", new { id = postImage.Id }, postImage);
		}

		// DELETE: api/PostImage/5
		[ResponseType(typeof(PostImage))]
		public IHttpActionResult DeletePostImage(string id)
		{
			PostImage postImage = db.PostImages.Find(id);
			if (postImage == null)
			{
				return NotFound();
			}

			// User can delete only own postImages
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(postImage.Post.UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.PostImages.Remove(postImage);
			db.SaveChanges();

			return Ok(postImage);
		}

		[HttpGet]
		[AllowAnonymous]
		[Route("Raw")]
		public HttpResponseMessage GetFile(Guid id, ImageType type)
		{

			var vLoc = string.Format("{0}::{1}(string id={2}, ImageType type={3})",
				TYPE_NAME, nameof(GetFile), id, type.ToString());
			Debug.WriteLine(vLoc + "...");

#if ERROR_FILE_LOGGING
			FileLogger fl = new FileLogger(5, WebsiteDefaults.GetFilesPath(), nameof(PostImageController));
			//fl.WriteLine(5, vLoc + "...");
#endif
			//Create HTTP Response.
			HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

			try
			{

				// Find image info by id in database
				var postImage = db.PostImages.Find(id);

				// Folder base by file type
				string folderPath = WebsiteDefaults.GetThumbFilesPath();
				if (postImage != null)
				{
					switch (type)
					{
						case ImageType.Original:
							folderPath = WebsiteDefaults.GetOriginalFilesPath();
							break;

						case ImageType.Normal:
							folderPath = WebsiteDefaults.GetNormalFilesPath();
							break;

						default:
							//case ImageType.Thumbnail:
							//	folderPath = WebsiteDefaults.GetThumbFilesPath();
							break;
					}
					Debug.WriteLine(vLoc + ", folderPath: " + folderPath);

					// Final path version
					string filePath = Path.Combine(folderPath, postImage.RawName);
					Debug.WriteLine(vLoc + ", filePath: " + filePath);

					// Check whether File exists.
					if (!File.Exists(filePath))
					{
						//Throw 404 (Not Found) exception if File not found.
						response.StatusCode = HttpStatusCode.NotFound;
						response.ReasonPhrase = string.Format("File not found!");
#if ERROR_FILE_LOGGING
						fl.WriteLine(5, string.Format("{0}, File not found: {1}", vLoc, filePath));
#endif
						throw new HttpResponseException(response);
					}

					//Read the File into a Byte Array.
					byte[] bytes = File.ReadAllBytes(filePath);

					//Set the Response Content.
					response.Content = new ByteArrayContent(bytes);

					//Set the Response Content Length.
					response.Content.Headers.ContentLength = bytes.LongLength;

					//Set the Content Disposition Header Value and FileName.
					response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
					response.Content.Headers.ContentDisposition.FileName = postImage.Name;

					//Set the File Content Type.
					response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(postImage.Name));
				}
				else
				{
					response.StatusCode = HttpStatusCode.NotFound;
					response.ReasonPhrase = string.Format("Record about this file not found.");

#if ERROR_FILE_LOGGING
					fl.WriteLine(5, string.Format("{0}, PostImage DB record with Id {1} wasn't found", vLoc, id));
#endif
					throw new HttpResponseException(response);
				}
			}
			catch(Exception ex)
			{
				response.StatusCode = HttpStatusCode.InternalServerError;
				response.ReasonPhrase = ex.Message;

#if ERROR_FILE_LOGGING
				fl.WriteLine(5, string.Format("{0}, An exception thrown: [{1}], StackTrace: {2} ", vLoc, ex.Message, ex.StackTrace));
#endif
			}

			return response;
		}

#if DEBUG
		//[AllowAnonymous]
		//[HttpGet]
		//[Route("Test696")]
		//public string GetTest696()
		//{
		//	return String.Format(
		//		"Files: {0}\r\nOriginal: {1}\r\nNormal: {2}\r\nThumb: {3}",
		//		WebsiteDefaults.GetFilesPath(),
		//		WebsiteDefaults.GetOriginalFilesPath(),
		//		WebsiteDefaults.GetNormalFilesPath(),
		//		WebsiteDefaults.GetThumbFilesPath());
		//}
#endif
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool PostImageExists(Guid id)
		{
			return db.PostImages.Count(e => e.Id == id) > 0;
		}
	}
}