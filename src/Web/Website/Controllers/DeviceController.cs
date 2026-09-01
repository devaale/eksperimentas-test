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
using System.Net.Http.Headers;
using static System.Net.WebRequestMethods;

using Microsoft.AspNet.Identity;

using Experiment.Core;
using Experiment.Core.Enums;
using Experiment.Data.Enums;
using Experiment.Data.Metadata;

using M = Experiment.Data.Models;

using Website.Data;
using Website.Models;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/Device")]
	public class DeviceController : ApiController
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		// GET: api/Device
		[Route("All")]
		public IQueryable<Device> GetDevices(string objectIds)
		{
			// If ids not NULL split & if result not nul select/convert it to ints array
			var ids = objectIds?.Split(Defaults.FIELD_SEPARATOR)?.Select(Int32.Parse)?.ToList();

			if(ids != null)
			{
				// Filter only specific user's devices
				var userId = User.Identity.GetUserId();

				return db.Devices.Where(device =>
					ids.Contains(device.ObjectId) &&
					(device.Object.UserId.Equals(userId) ||
					device.Object.Permissions.Any(op => op.FriendUserId.Equals(userId))));
			}
			else
			{
				throw new ArgumentNullException(nameof(objectIds));
			}
		}

		// GET: api/Device/5
		[ResponseType(typeof(Device))]
		public IHttpActionResult GetDevice(int id)
		{
			Device device = db.Devices.Find(id);
			if (device == null)
			{
				return NotFound();
			}

			return Ok(device);
		}

		// PUT: api/Device/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutDevice(int id, Device device)
		{
            if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != device.Id)
			{
				return BadRequest();
			}

            // Find object
            var obj = db.Objects.Find(device.ObjectId);
            if (obj == null)
            {
                return NotFound();
            }

			// User Id
			var userId = User.Identity.GetUserId();
			// Is this object owner is this user
			var hasPerms = userId.Equals(obj.UserId);
			// If not
			if (!hasPerms)
			{
				// Maybe this object is shared for him with write permissions?
				hasPerms = db.ObjectPermissions.Any(op =>
					op.ObjectId.Equals(obj.Id) &&
					op.FriendUserId.Equals(userId) &&
					op.PermWrite == true);
			}

			if (!hasPerms)
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.Entry(device).State = EntityState.Modified;

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!DeviceExists(id))
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

		// POST: api/Device
		[ResponseType(typeof(Device))]
		public IHttpActionResult PostDevice(Device device)
		{
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find object
            var obj = db.Objects.Find(device.ObjectId);
            if (obj == null)
            {
                return NotFound();
            }

            // User Id
            var userId = User.Identity.GetUserId();
			// Is this object owner is this user
			var hasPerms = userId.Equals(obj.UserId);
			// If not
			if(!hasPerms)
			{
				// Maybe this object is shared for him with write permissions?
				hasPerms = db.ObjectPermissions.Any(op => 
					op.ObjectId.Equals(obj.Id) && 
					op.FriendUserId.Equals(userId) &&
					op.PermWrite == true);
			}

			if (!hasPerms)
            {
                return StatusCode(HttpStatusCode.Conflict);
            }

			db.Devices.Add(device);
			var response = db.SaveChanges();

			// Create depreciation datapoint
			Datapoint datapoint = new Datapoint()
			{
				DeviceId = device.Id,
				DatapointType = DatapointType.Virtual,
				Name = device.Name + " Depreciation",
				DatapointFormulaId = 1030,
				IntervalDatepart = DatePartOrInterval.Day,
				Multiplier = 1,
			};

			db.Datapoints.Add(datapoint);
			db.SaveChanges();

			return CreatedAtRoute("DefaultApi", new { id = device.Id }, device);
		}

		// DELETE: api/Device/5
		[ResponseType(typeof(Device))]
		public IHttpActionResult DeleteDevice(int id)
		{
            // Find device
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return NotFound();
            }

            // Find object
            EObject obj = db.Objects.Find(device.ObjectId);
            if (obj == null)
            {
                return NotFound();
            }

            // Check that user updated only own devices
            var userId = User.Identity.GetUserId();
            if (!userId.Equals(obj.UserId))
            {
                return StatusCode(HttpStatusCode.Conflict);
            }

			// Remove datapoints
			var datapoints = db.Datapoints.Where(d => d.DeviceId.Equals(device.Id));

			foreach (var d in datapoints)
			{
				db.Datapoints.Remove(d);
			}
			db.SaveChanges();

			db.Devices.Remove(device);
			db.SaveChanges();

			return Ok(device);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool DeviceExists(int id)
		{
			return db.Devices.Count(e => e.Id == id) > 0;
		}
	}
}