using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;

using Microsoft.AspNet.Identity;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;
using M = Experiment.Data.Models;

using Website.Data;
using Website.Models;
using System.Diagnostics;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/TokenTransaction")]
	public class TokenTransactionController : ApiController
	{
		private ApplicationDbContext db = new ApplicationDbContext();
		/*
		// GET: api/TokenTransaction/5
		[ResponseType(typeof(TokenTransaction))]
		public IHttpActionResult GetTokenTransaction(Guid id)
		{
			TokenTransaction tokenTransaction = db.TokenTransactions.Find(id);
			if (tokenTransaction == null)
			{
				return NotFound();
			}

			var userId = User.Identity.GetUserId();
			var userRelatedToTransaction = userId.Equals(tokenTransaction.SenderUserId) || userId.Equals(tokenTransaction.ReceiverUserId);
			if(!userRelatedToTransaction)
			{
				return Unauthorized();
			}

			return Ok(tokenTransaction);
		}

		// PUT: api/TokenTransaction/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutTokenTransaction(Guid id, TokenTransaction tokenTransaction)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != tokenTransaction.Id)
			{
				return BadRequest();
			}

			var userId = User.Identity.GetUserId();
			if(!userId.Equals(tokenTransaction.SenderUserId))
			{
				return Unauthorized();
			}

			db.Entry(tokenTransaction).State = EntityState.Modified;

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!TokenTransactionExists(id))
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

		// POST: api/TokenTransaction
		[ResponseType(typeof(TokenTransaction))]
		public async Task<IHttpActionResult> PostTokenTransaction(TokenTransaction tokenTransaction)
		{
			// Model is valid?
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Confirm is user trying to give own, not another user tokens, what can be wrong
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(tokenTransaction.SenderUserId))
			{
				return Unauthorized();
			}

			// Is receiver exists?
			var receiver = db.Users.Find(tokenTransaction.ReceiverUserId);
			if(receiver == null)
			{
				return NotFound();
			}

			// Now checking do specific user has enough of tokens
			var sender = db.Users.Find(tokenTransaction.SenderUserId);
			if(sender.Tokens < tokenTransaction.Tokens)
			{
				return BadRequest("User doesn't have enough of tokens!");
			}

			// Removing specific ammount of tokens from sender
			//Debug.WriteLine(String.Format("{0}={1} {2}", "sender.Tokens", sender.Tokens, "BEFORE"));
			sender.Tokens -= tokenTransaction.Tokens;
			//Debug.WriteLine(String.Format("{0}={1} {2}", "sender.Tokens", sender.Tokens, "AFTER"));
			db.Entry(sender).State = EntityState.Modified;

			// Adding this ammount of tokens for receiver
			//Debug.WriteLine(String.Format("{0}={1} {2}", "receiver.Tokens", sender.Tokens, "BEFORE"));
			receiver.Tokens += tokenTransaction.Tokens;
			//Debug.WriteLine(String.Format("{0}={1} {2}", "receiver.Tokens", sender.Tokens, "AFTER")); 
			db.Entry(receiver).State = EntityState.Modified;

			// Set status as valid transaction
			tokenTransaction.Status = TokenTransactionStatus.Valid;

			// Adding transaction itself
			db.TokenTransactions.Add(tokenTransaction);

            var bc = new Blockchain();
            await bc.Send(User.Identity, receiver.Id, sender.Id, tokenTransaction.Tokens);

            // Saving changes
            db.SaveChanges();

			return CreatedAtRoute("DefaultApi", new { id = tokenTransaction.Id }, tokenTransaction);
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool TokenTransactionExists(Guid id)
		{
			return db.TokenTransactions.Count(e => e.Id == id) > 0;
		}
	}
}