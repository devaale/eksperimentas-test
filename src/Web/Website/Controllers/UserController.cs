using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;

using Microsoft.AspNet.Identity;

using Experiment.Core;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;

using M = Experiment.Data.Models;

using Website.Data;
using Website.Models;
using System.Diagnostics;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/User")]
	public class UserController : ApiController
	{
		const string TYPE_NAME = nameof(UserController);
		private ApplicationDbContext db = new ApplicationDbContext();

		/// <summary>
		/// Searches for any system users, which public name contains specified part of case insensitive name.
		/// Used for adding new friends.
		/// 
		/// @TODO: name might be unsafe regarding SQL injections (not sure)
		/// </summary>
		/// <param name="name">Case insensitive part of user's name</param>
		/// <returns></returns>
		[Route("Search")]
		public IList<M.User> GetUsers(UserRelationType type, string name)
		{
			var sql = "EXEC prcUserSearch @userId, @type, @phrase";
			var rawResult = db.Database.SqlQuery<M.User>(
				sql, new SqlParameter("@userId", User.Identity.GetUserId()),
				new SqlParameter("@type", type.ToString()),
				new SqlParameter("@phrase", name));
			var result = rawResult.ToArray();
			return result;
		}

		/// <summary>
		/// Get logged in user's info
		/// 
		/// @POOR EXAMPLE - DO NOT COPY PASTE THIS!
		/// </summary>
		/// <returns></returns>
		[Route("Info")]
		[ResponseType(typeof(M.UserInfo))]
		public M.UserInfo GetUserinfoByUserId(string userId)
		{
			var vLoc = string.Format("{0}::{1}(string userId={2}", TYPE_NAME, nameof(GetUserinfoByUserId), userId);
			Debug.WriteLine(vLoc);

			string currentUserId;
			string requestedUserId;
			M.UserInfo user = null;

			using (var transaction = db.Database.BeginTransaction())
			{

				try
				{
					// Get current user id
					currentUserId = User.Identity.GetUserId();
					// If requested user id empty, set it to current user (security)
					requestedUserId = (string.IsNullOrEmpty(userId) ? currentUserId : userId);

					// Get user's info in front-end structure
					var sql = "EXEC prcUserInfo @currentUserId, @requestedUserId";
					var result = db.Database.SqlQuery<M.UserInfo>(
						sql, new SqlParameter("@currentUserId", currentUserId),
						new SqlParameter("@requestedUserId", requestedUserId));
					user = result.FirstOrDefault();

					// We found an user?
					if (user != null)
					{
						// Is user requesting info about himself
						// Licenses can view only user itself
						if (user.IsMe)
						{
							// Licenses
							var now = DateTime.Now;
							var licenses = db.Licenses.Where(l =>
								l.UserId.Equals(user.Id) &&
								l.ValidUntil >= now &&
								l.Active == true);//.ToList();

							if (user.Licenses is ObservableCollection<M.License>)
							{
								var ocLicenses = user.Licenses as ObservableCollection<M.License>;

								// Adding fake license. As according to managers user which has no licenses, have basic license of type 1, which is free
								ocLicenses.Add(new M.License()
								{
									Type = UserLicenseType.License1,
									Active = true,
								});

								// Adding real licenses stored in DB
								foreach (var license in licenses)
								{
									ocLicenses.Add(Utils.Clone<M.License>(license));
								}
							}
						} // if (user.IsMe)
					}
					else
					{
						throw new Exception("User wasn't found!");
					}

					transaction.Commit();
				}
				catch (Exception ex)
				{
					Debug.WriteLine(string.Format("{0}, {1}", vLoc, ex.Message));
					transaction.Rollback();
				}
				finally
				{

				}
			}
			// Returning
			return user;
		}

		/// <summary>
		/// Get logged in user's info
		/// </summary>
		/// <returns></returns>
		[Route("Info")]
		[ResponseType(typeof(M.UserInfo))]
		public M.UserInfo GetUserinfoByPostId(int postId)
		{
			string currentUserId = User.Identity.GetUserId();

			var post = db.Posts.Find(postId);
			if(post != null)
			{
				return GetUserinfoByUserId(post.UserId);
			}

			throw new Exception("Post not found");
		}

		[ResponseType(typeof(void))]
		public IHttpActionResult PutUser(string id, ApplicationUser user)
		{
			if (user == null)
				return BadRequest();

			var currentUserId = User.Identity.GetUserId();
			var currentUser = db.Users.Find(currentUserId);

			if (currentUser != null)
			{
				if(!currentUser.Language.Equals(user.Language))
				{
					currentUser.Language = user.Language;
					db.Entry(currentUser).State = EntityState.Modified;
					db.SaveChanges();
					return StatusCode(HttpStatusCode.NoContent);
				}
			}

			return BadRequest();
		}

	}
}
