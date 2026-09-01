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
using Microsoft.EntityFrameworkCore.Internal;

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
	[RoutePrefix("api/DeviceTopic")]
	public class DeviceTopicController : ApiController
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		public DeviceTopic[] GetDeviceTopics(int deviceId)
		{
			if(UserHasAccessToTheDevice(deviceId))
			{
				return db.DeviceTopics.Where(dt => dt.DeviceId == deviceId)
					.ToArray();
			}

			// Device not found
			throw new ArgumentException(nameof(deviceId));
		}

		// PUT: api/DeviceTopic/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutDeviceTopic(int id, DeviceTopic deviceTopic)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != deviceTopic.Id)
			{
				return BadRequest();
			}

			if (UserHasAccessToTheDevice(deviceTopic.DeviceId))
			{
				// Find the same topic for the same device, just with different deviceTopicId
				var exists = db.DeviceTopics.FirstOrDefault(dt =>
					dt.DeviceId == deviceTopic.DeviceId &&
					dt.Topic.Equals(deviceTopic.Topic) &&
					dt.Id != deviceTopic.Id);

				// If not exists already such topic for device
				if(exists == null)
				{
					db.Entry(deviceTopic).State = EntityState.Modified;

					try
					{
						db.SaveChanges();
					}
					catch (DbUpdateConcurrencyException)
					{
						if (!DeviceTopicExists(id))
						{
							return NotFound();
						}
						else
						{
							throw;
						}
					}
				}

			}
			else
			{
				return NotFound();
			}

			return StatusCode(HttpStatusCode.NoContent);
		}

		// POST: api/DeviceTopic
		[ResponseType(typeof(DeviceTopic))]
		public IHttpActionResult PostDeviceTopic(DeviceTopic deviceTopic)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if(UserHasAccessToTheDevice(deviceTopic.DeviceId))
			{
				// Try to find the deviceTopic with the same deviceid and topic
				var exists = db.DeviceTopics.FirstOrDefault(d => 
					d.DeviceId == deviceTopic.DeviceId &&
					d.Topic.Equals(deviceTopic.Topic));
				if (exists != null)
				{
					// Return already existing
					return CreatedAtRoute("DefaultApi", new { id = exists.Id }, exists);
				}
				else
				{
					// Not existing! Add new and return it
					db.DeviceTopics.Add(deviceTopic);
					db.SaveChanges();

					return CreatedAtRoute("DefaultApi", new { id = deviceTopic.Id }, deviceTopic);
				}
			}
			else
			{
				return NotFound();
			}
		}

		// DELETE: api/DeviceTopic/5
		[ResponseType(typeof(DeviceTopic))]
		public IHttpActionResult DeleteDeviceTopic(int id)
		{
			var deviceTopic = db.DeviceTopics.Find(id);
			if(deviceTopic != null)
			{
				if (UserHasAccessToTheDevice(deviceTopic.DeviceId))
				{
					db.DeviceTopics.Remove(deviceTopic);
					db.SaveChanges();

					return Ok(deviceTopic);
				}
			}
			return NotFound();
		}


		#region Helpers
		/// <summary>
		/// If device exists and user has access to it
		/// </summary>
		/// <param name="deviceId"></param>
		/// <returns>true</returns>
		bool UserHasAccessToTheDevice(int deviceId)
		{
			// Search for device with specific deviceId
			var device = db.Devices
				.Include(o => o.Object)
				.Include(p => p.Object.Permissions)
				.FirstOrDefault(dev => dev.Id == deviceId);

			// Device not found
			if (device == null)
				return false;

			// Device found
			// Retrieving authorized user Id
			var userId = User.Identity.GetUserId();

			// User has an access do the device?
			return device.Object.UserId.Equals(userId) || 
				device.Object.Permissions.Any(p => p.FriendUserId.Equals(userId));
		}

		private bool DeviceTopicExists(int id)
		{
			return db.DeviceTopics.Count(e => e.Id == id) > 0;
		}


		#endregion
	}
}