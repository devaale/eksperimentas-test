//#define ENFORCE_MODEL
#define ENABLE_REDUNDANT_CODE

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
using System.Net.Http.Headers;
using System.Web.Http.Results;

using Microsoft.AspNet.Identity;

using Experiment.Core;
using Experiment.Data.Enums;
using Experiment.Data.Drawing;
using Experiment.Data.Metadata;
using M = Experiment.Data.Models;

using Website.Data;
using Website.Models;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/Message")]
	public class MessageController : ApiController
	{
		const string TYPE_NAME = nameof(MessageController);

		private ApplicationDbContext db = new ApplicationDbContext();

		/// <summary>
		/// Returns conversation by receiverUserId
		/// 
		/// GET: api/Message?receiverUserId=xx&sinceDate=null
		/// </summary>
		/// <param name="receiverUserId"></param>
		/// <param name="sinceDate"></param>
		/// <returns></returns>
		public M.ChatMessage[] GetConversation(
			string receiverUserId, 
			ListLoadMode loadMode, 
			DateTime? firstDate,
			DateTime? lastDate)
		{

			object firstDateObj = DBNull.Value;
			if (firstDate.HasValue)
				firstDateObj = firstDate;

			object lastDateObj = DBNull.Value;
			if (lastDate.HasValue)
				lastDateObj = lastDate;

			var sql = "EXEC prcChatMessages @senderUserId, @receiverUserId, @listLoadMode, @firstDate, @lastDate";
			var rawResult = db.Database.SqlQuery<M.ChatMessage>(
				sql,
				new SqlParameter("@senderUserId", User.Identity.GetUserId()),
				new SqlParameter("@receiverUserId", receiverUserId),
				new SqlParameter("@listLoadMode", loadMode),
				new SqlParameter("@firstDate", firstDateObj),
				new SqlParameter("@lastDate", lastDateObj));
			var result = rawResult.ToArray();
			return result;
		}

		/// <summary>
		/// Returns conversation by specific postId, which author is user
		/// 
		/// GET: api/Message?receiverUserId=xx&sinceDate=null
		/// </summary>
		/// <param name="postId"></param>
		/// <param name="sinceDate"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public M.ChatMessage[] GetConversation(
			int postId,
			ListLoadMode loadMode,
			DateTime? firstDate,
			DateTime? lastDate)
		{
			var post = db.Posts.Find(postId);
			if (post == null)
			{
				throw new Exception("post not found");
			}

			return GetConversation(post.UserId, loadMode, firstDate, lastDate);
		}

		/// <summary>
		/// Get all user's conversations
		/// </summary>
		/// <returns></returns>
		public M.ChatConversation[] GetConversations()
		{
			var sql = "EXEC prcChatConversations @userId";
			var rawResult = db.Database.SqlQuery<M.ChatConversation>(
				sql,
				new SqlParameter("@userId", User.Identity.GetUserId()));
			var result = rawResult.ToArray();
			return result;
		}

		/// <summary>
		/// Returns all user's messages
		/// 
		/// GET: api/Message
		/// </summary>
		/// <returns></returns>
		//public IQueryable<Message> GetMessages()
		//{
		//	// Filter only specific user's messages
		//	var userId = User.Identity.GetUserId();
		//	return db.Messages.Where(message => message.SenderUserId == userId);
		//}

		// GET: api/Message/5
		//[ResponseType(typeof(Message))]
		//public IHttpActionResult GetMessage(int id)
		//{
		//	Message message = db.Messages.Find(id);
		//	if (message == null)
		//	{
		//		return NotFound();
		//	}
		//
		//	return Ok(message);
		//}

		// PUT: api/Message/5
		//[ResponseType(typeof(void))]
		//public IHttpActionResult PutMessage(int id, Message message)
		//{
		//	if (!ModelState.IsValid)
		//	{
		//		return BadRequest(ModelState);
		//	}
		//
		//	if (id != message.Id)
		//	{
		//		return BadRequest();
		//	}

		//	// Check that user updated only own messages
		//	var userId = User.Identity.GetUserId();
		//	if (!userId.Equals(message.SenderUserId))
		//	{
		//		return StatusCode(HttpStatusCode.Conflict);
		//	}
		//
		//	db.Entry(message).State = EntityState.Modified;
		//
		//	try
		//	{
		//		db.SaveChanges();
		//	}
		//	catch (DbUpdateConcurrencyException)
		//	{
		//		if (!MessageExists(id))
		//		{
		//			return NotFound();
		//		}
		//		else
		//		{
		//			throw;
		//		}
		//	}
		//
		//	return StatusCode(HttpStatusCode.NoContent);
		//}

		// POST: api/Message
		[ResponseType(typeof(Message))]
		public IHttpActionResult PostMessage(Message message)
		{
			var vLoc = string.Format("{0}::{1}(Message message)", TYPE_NAME, nameof(PostMessage));
			// Adding User Id
			var userId = User.Identity.GetUserId();
			message.SenderUserId = userId;  // user can write only own messages (security)

#if ENFORCE_MODEL
			ModelState.Clear();
#endif
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			db.Messages.Add(message);
			var response = db.SaveChanges();

			return CreatedAtRoute("DefaultApi", new { id = message.Id }, message);
		}

		// DELETE: api/Message/5
		[ResponseType(typeof(Message))]
		public IHttpActionResult DeleteMessage(int id)
		{
			Message message = db.Messages.Find(id);
			if (message == null)
			{
				return NotFound();
			}

			// User can delete only own messages
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(message.SenderUserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.Messages.Remove(message);
			db.SaveChanges();

			return Ok(message);
		}

		[Route("Read")]
		[HttpGet]
		public bool ReadMessage(int messageId)
		{
			var msg = db.Messages.Find(messageId);
			if (msg == null)
				return false;

			// Only receiver marks messages as read, as sender when written it already read it.
			var userId = User.Identity.GetUserId();
			if (!string.Equals(msg.ReceiverUserId, userId))
				return false;

			msg.Read = DateTime.Now;
			db.Entry(msg).State = EntityState.Modified;
			db.SaveChanges();

			return true;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool MessageExists(int id)
		{
			return db.Messages.Count(e => e.Id == id) > 0;
		}
	}
}