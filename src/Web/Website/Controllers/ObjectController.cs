//#define REAL_DELETE // DON'T Enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

using Microsoft.AspNet.Identity;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;

using Website.Models;

namespace Website.Controllers
{

	[Authorize]
	[RoutePrefix("api/Object")]
	public class ObjectController : ApiController
	{
		#region Constant
		const string TYPE_NAME = nameof(ObjectController);
		const bool DEBUG = true;

		#endregion

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

		private bool ObjectExists(int id)
		{
			return db.Objects.Count(e => e.Id == id) > 0;
		}

		#endregion

		#region Methods

		/// <summary>
		/// It works but returning as well user's info
		/// </summary>
		/// <returns></returns>
		// GET: api/Object
		public IQueryable<EObject> GetObjects()
		//public string GetObjects()
		{
			// Filter only specific user's objects
			var userId = User.Identity.GetUserId();

			var result = db.Objects.Where(
				obj => (
					obj.UserId == userId ||
					obj.Permissions.Any(op => userId.Equals(op.FriendUserId))) &&
					obj.Deleted == null)
				// @see Eagerly Loading at https://learn.microsoft.com/en-us/ef/ef6/querying/related-data
				.Include(
					obj => obj.Permissions);
			return result;
			//return JsonConvert.SerializeObject(result);
			/*
			// Getting friends for processing
			var friends = db.Friends.Where(friend => friend.UserId.Equals(userId));

			// Converting to safe objects
			var retVal = new List<IObject>();
			foreach (var o in result)
			{
				retVal.Add(EObject.ToFrontendObject(userId, o, friends));
			}
			return retVal;
			*/
		}

		// GET: api/Object/5
		[ResponseType(typeof(IObject))]
		public IHttpActionResult GetObject(int objectId)
		{
			// Find object
			EObject obj = db.Objects.Find(objectId);
			// Load its permissions, @see Explicitly Loading at https://learn.microsoft.com/en-us/ef/ef6/querying/related-data
			db.Entry(obj).Collection(nameof(EObject.Permissions)).Load();

			if (obj != null)
			{
				// Filter only specific user's objects
				var userId = User.Identity.GetUserId();
				var friends = db.Friends.Where(friend => friend.UserId.Equals(userId));

				if (
					obj.Deleted == null && 
					(
						obj.UserId.Equals(userId) || // Specific user object
						obj.Permissions.Any(op => userId.Equals(op.FriendUserId))	// Permitted to specific user object
					))
				{
					//return Ok(EObject.ToFrontendObject(userId, obj, friends));
					return Ok(obj);
				}
			}

			return NotFound();
		}

		/*
		// GET: api/Object/5
		[Route("New")]
		[ResponseType(typeof(IObject))]
		public IObject GetNewObject()
		{
			// Filter only specific user's objects
			var userId = User.Identity.GetUserId();
			var friends = db.Friends.Where(friend => friend.UserId.Equals(userId));
			return EObject.ToFrontendObject(userId, new EObject()
			{
				UserId = userId,
			}, friends);
		}
		*/

		// PUT: api/Object/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutObject(int id, EObject obj)
		{
			var vLog = string.Format("{0}::{1}(int id={2},..)", TYPE_NAME, nameof(PutObject), id);
			Debug.WriteLineIf(DEBUG, vLog);
			/*
			// Find object
			EObject obj = db.Objects.Find(id);

			// Load its permissions, @see Explicitly Loading at https://learn.microsoft.com/en-us/ef/ef6/querying/related-data
			db.Entry(obj).Collection(nameof(EObject.Permissions)).Load();

			// Update with data from frontend
			obj.Update(db, frontObj);
			*/
			using (var transaction = db.Database.BeginTransaction())
			{

				try
				{
					if (!ModelState.IsValid)
					{
						transaction.Rollback();
						return BadRequest(ModelState);
					}

					if (id != obj.Id)
					{
						transaction.Rollback();
						return BadRequest();
					}

					// Check that user updated only own friends
					var userId = User.Identity.GetUserId();
					if (!userId.Equals(obj.UserId))
					{
						transaction.Rollback();
						return StatusCode(HttpStatusCode.Conflict);
					}

					var perms = obj.Permissions;
					obj.Permissions = null;
					// Make sure they all have right objectId
					foreach (var perm in perms)
					{
						perm.Id = 0;
						perm.ObjectId = obj.Id;
					}

					// Update Object
					db.Entry(obj).State = EntityState.Modified;
					db.SaveChanges();

					// Purge existing object permissions
					// This way algorithm is simplier (A.G.)
					var actualPerms = db.ObjectPermissions.Where(op => op.ObjectId == obj.Id);
					db.ObjectPermissions.RemoveRange(actualPerms);
					db.SaveChanges();

					// Save new ones
					db.ObjectPermissions.AddRange(perms);
					db.SaveChanges();

					// Commit transaction
					transaction.Commit();

				}
				catch (Exception ex)
				{
					Debug.WriteLineIf(DEBUG, string.Format("{0}, {1}", vLog, ex.Message));
					transaction.Rollback();
					return NotFound();
				}
				finally
				{

				}

				return StatusCode(HttpStatusCode.NoContent);
			}
		}

		// POST: api/Object
		[ResponseType(typeof(EObject))]
		public IHttpActionResult PostObject(EObject obj)
		{
			/*
			// Creating new Object
			var obj = new EObject()
			{
				UserId = User.Identity.GetUserId(),
				Permissions = new List<ObjectPermission>(),
			};
			obj.Update(db, frontObj);

			ModelState.Clear();
			*/
			var userId = User.Identity.GetUserId();
			obj.UserId = userId;

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			db.Objects.Add(obj);
			var response = db.SaveChanges();

			return CreatedAtRoute("DefaultApi", new { id = obj.Id }, obj);
		}

		// DELETE: api/Object/5
		[ResponseType(typeof(EObject))]
		public IHttpActionResult DeleteObject(int id)
		{
			EObject obj = db.Objects.Find(id);
#if REAL_DELETE
			if (obj == null)
			{
				return NotFound();
			}

			// User can delete only own friends
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(obj.UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.Objects.Remove(obj);
			db.SaveChanges();

			return Ok(obj);
#else
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != obj.Id)
			{
				return BadRequest();
			}

			// Check that user updated only own object
			var userId = User.Identity.GetUserId();
			if (!userId.Equals(obj.UserId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			// Mark as deleted
			obj.Deleted = DateTime.Now;

			db.Entry(obj).State = EntityState.Modified;

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ObjectExists(id))
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


		[Route("Ai")]
        public IHttpActionResult GetEnableAi(int id)
		{
			var retVal = false;

            using (var tran = db.Database.BeginTransaction())
			{
                EObject obj = db.Objects.Find(id);

				if (obj != null)
                {
                    // Check that user updated only own object
                    var userId = User.Identity.GetUserId();
                    if (!userId.Equals(obj.UserId))
                    {
                        return StatusCode(HttpStatusCode.Conflict);
                    }

                    var relDevice = db.Devices
                        .Where(d => d.ObjectId == id &&
                                d.Protocol == DeviceProtocol.API &&
                                d.UnitId == 1)
                        .FirstOrDefault();

					// Ai Device Template
					// Experiment.Data.Models.DeviceSetting.AiSupportDeviceSetting; @deprecated

					if (relDevice == null)
                    {
						// Create new AI support device for specific Object
						relDevice = new Device()
						{
                            Name = "API AI Support",
                            ObjectId = obj.Id,
							UnitId = 1,
							Protocol = DeviceProtocol.API,
							Interval = 3600,
                        };

						db.Devices.Add(relDevice);
						db.SaveChanges();
                    }

					if(relDevice != null)
					{
						// Select all datapoints of specific device, which Alias field is not null
						var relDatapoints = db.Datapoints
							.Where(dp =>
								dp.DeviceId == relDevice.Id &&
								!string.IsNullOrEmpty(dp.Alias))
							.ToList();

						var dSettings = db.DatapointSettings.Where(ds => ds.Protocol == 100);
						foreach (var templateDp in dSettings)
						{
							if(!relDatapoints.Any(rdp => rdp.Alias.Equals(templateDp.Name)))
							{
								var nDp = new Datapoint()
								{
									DeviceId = relDevice.Id,
									Name = templateDp.Name,
									Alias = templateDp.Name,
									DatapointType = DatapointType.Virtual,
									Multiplier = 1,
								};

								db.Datapoints.Add(nDp);
								db.SaveChanges();
							}
						}
					}

					retVal = relDevice != null;

					tran.Commit();
                }
            }

            return Ok(retVal);
        }
		#endregion
	}
}