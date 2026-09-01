using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.UI.WebControls;

using Microsoft.AspNet.Identity;

using Experiment.Core;
using Experiment.Core.Data;
using Experiment.Core.Enums;
using M=Experiment.Data.Models;

using Website.Data;
using Website.Data.ContentEngine;
using Website.Models;
using Experiment.Data.Enums;
using Experiment.Core.Metadata;

namespace Website.Controllers
{
	/// <summary>
	/// Experiment API controller
	/// </summary>
	[Authorize]
	[RoutePrefix("api/Experiment")]
	public class ExperimentApiController : ApiController
	{
		#region Constants
		const string TYPE_OBJECT = "obj";
		const string TYPE_DEVICE = "dev";
		const string TYPE_DATAPOINT = "dtp";

		#endregion

		private ApplicationDbContext db = new ApplicationDbContext();

		public string GetLanguage()
		{
			using (var dbc = new ApplicationDbContext())
			{
				string currentUserId = HttpContext.Current.User.Identity.GetUserId();
				if (string.IsNullOrEmpty(currentUserId))
				{
					return Defaults.DEFAULT_LANGUAGE;
				}
				else
				{
					// Retrieving User's info
					ApplicationUser currentUser = dbc.Users.FirstOrDefault(x => x.Id == currentUserId);
					// Saving in session his prefered language
					return currentUser.Language;
				}
			}
		}

		[Route("Tree")]
		//[HttpGet]
		public TreeItem[] GetTree()
		{
			var sql = "EXEC prcMainTree @userId";
			var rawResult = db.Database.SqlQuery<TreeItem>(
				sql, new SqlParameter("@userId", User.Identity.GetUserId()));
			var result = rawResult.ToArray();
			return result;
		}

		[AllowAnonymous]
		[Route("TreeItem")]
		public ContentEngineResult GetTreeItem(string id)
		{
			if(string.IsNullOrEmpty(id))
				throw new ArgumentNullException("id");

			var paramType = Regex.Replace(id, "[^A-z.]", "");
			var paramId = int.Parse(Regex.Replace(id, "[^0-9.]", ""));
			var lang = GetLanguage();

			switch (paramType)
			{
				case TYPE_DEVICE:
					return GetDevice(lang, paramId);

				case TYPE_DATAPOINT:
					return GetDatapoint(lang, paramId);

				default:
					throw new ArgumentException("id");
			}
		}

		ContentEngineResult GetDevice(string lang, int id)
		{
			var device = db.Devices.Find(id);
			var result = new ContentEngineResult()
			{
				SubmitUrl = Defaults.URL_API_DEVICE + "/" + id,
				Options = new ContentEngineOptions()
				{
					CanUpdate = true,
					UpdateText = E.T("save", lang),
				},
				Messages = new List<ContentEngineMessage>()
				{
					new ContentEngineMessage()
					{
						MessageLevel = 1,
						Message = "Device load test!",
					},
				},
				Data = new List<IContentEngineItem>()
				{
					new ContentEngineGroup()
					{
						Label =  E.T("mainInfo", lang),
						Type = ContentEngine.TYPE_GROUP,
						Items = new List<IContentEngineItem>()
						{
							new ContentEngineField()
							{
								Name = "Name",
								Label = E.T("name", lang),
								Type = ContentEngine.TYPE_STRING,
								Required = true,
								Value = device.Name,
							},
							new ContentEngineField()
							{
								Name = "Description",
								Label = E.T("description", lang),
								Type = ContentEngine.TYPE_TEXT,
								Value = device.Description,
							},
						},
					}, // mainGroup
					new ContentEngineGroup()
					{
						Label =  E.T("metaInfo", lang),
						Type = ContentEngine.TYPE_GROUP,
						Items = new List<IContentEngineItem>()
						{
							new ContentEngineField()
							{
								Name = "Id",
								Label = E.T("id", lang),
								Type = ContentEngine.TYPE_ID,
								Value = device.Id,
								ReadOnly = true,
							},
						}
					},
				},
			};
			return result;
		}

		ContentEngineResult GetDatapoint(string lang, int id)
		{
			var datapoint = db.Datapoints.Find(id);
			var result = new ContentEngineResult()
			{
				SubmitUrl = Defaults.URL_API_DATAPOINT + "/" + id,
				Options = new ContentEngineOptions()
				{
					CanUpdate = true,
					UpdateText = E.T("save", lang),
				},
				Messages = new List<ContentEngineMessage>()
				{
					new ContentEngineMessage()
					{
						MessageLevel = 1,
						Message = "Datapoint load test!",
					},
				},
				Data = new List<IContentEngineItem>()
				{
					new ContentEngineGroup()
					{
						Label =  E.T("mainInfo", lang),
						Type = ContentEngine.TYPE_GROUP,
						Items = new List<IContentEngineItem>()
						{
							new ContentEngineField()
							{
								Name = "Name",
								Label = E.T("name", lang),
								Type = ContentEngine.TYPE_STRING,
								Required = true,
								Value = datapoint.Name,
							},
							new ContentEngineField()
							{
								Name = "Description",
								Label = E.T("description", lang),
								Type = ContentEngine.TYPE_TEXT,
								Value = datapoint.Description,
							},
						},
					}, // mainGroup
					new ContentEngineGroup()
					{
						Label =  E.T("metaInfo", lang),
						Type = ContentEngine.TYPE_GROUP,
						Items = new List<IContentEngineItem>()
						{
							new ContentEngineField()
							{
								Name = "Id",
								Label = E.T("id", lang),
								Type = ContentEngine.TYPE_ID,
								Value = datapoint.Id,
								ReadOnly = true,
							},
							new ContentEngineField()
							{
								Name = "DeviceId",
								Label = E.T("deviceId", lang),
								Type = ContentEngine.TYPE_ID,
								Value = datapoint.DeviceId,
								ReadOnly = true,
							},
						}
					},
				},
			};
			return result;
		}

		[Route("Users")]
		[HttpGet]
		public List<M.NamedDbItem<string>> GetUsers()
		{
			string currentUserId = User.Identity.GetUserId();
			var currentUser = db.Users.Find(currentUserId);
			if (!currentUser.IsAdmin)
				throw new Exception(E.T("accessDenied"));

			List<M.NamedDbItem<string>> retVal = new List<M.NamedDbItem<string>>();
			foreach (var user in db.Users.OrderBy(u => u.Name))
			{
				retVal.Add(new M.NamedDbItem<string>()
				{
					Id = user.Id,
					Name = user.Name,
				});
			}

			return retVal;
		}

		[Route("User/Info")]
		[HttpGet]
		public M.UserInfo GetUserInfo(string userId)
		{
			// Check for admin rights
			string currentUserId = User.Identity.GetUserId();
			var currentUser = db.Users.Find(currentUserId);
			if (!currentUser.IsAdmin)
				throw new Exception(E.T("accessDenied"));

			var user = db.Users.Find(userId);
			if(user != null)
			{
				var retVal = new M.UserInfo()
				{
					Id = user.Id,
					Name = user.Name,
				};

				var now = DateTime.Now;
				var licenses = db.Licenses
					.Where(
						l => l.UserId.Equals(retVal.Id)) // && (l.ValidFrom <= now && l.ValidUntil >= now));
					.OrderBy(
						l => l.ValidFrom); 

				if (retVal.Licenses is ObservableCollection<M.License>)
				{
					var ocLicenses = retVal.Licenses as ObservableCollection<M.License>;
					foreach (var license in licenses)
					{
						ocLicenses.Add(Utils.Clone<M.License>(license));
					}
				}

				return retVal;
			}

			throw new Exception(E.T("notFound"));
		}

		[Route("User/Update")]
		public IHttpActionResult PostUserUpdate(M.UserInfo user)
		{
			//var retVal = true;

			if (user == null)
				return NotFound();

			// Check for admin rights
			string currentUserId = User.Identity.GetUserId();
			var currentUser = db.Users.Find(currentUserId);
			if (!currentUser.IsAdmin)
				throw new Exception(E.T("accessDenied"));

			using(var transaction = db.Database.BeginTransaction())
			{
				try
				{
					// Getting existing user
					var existingUser = db.Users.Find(user.Id);
					if (existingUser != null)
					{
						// Getting user's existing licenses
						var existingLicenses = db.Licenses.Where(
							l => l.UserId.Equals(existingUser.Id));

						// Checking all user's existing licenses
						foreach (var existingLicense in existingLicenses)
						{
							// If user deleted some licenses, they won't be find in returned from UI list 
							if (!user.Licenses.Any(l => l.Id.Equals(existingLicense.Id)))
							{
								// And we need to remove from db not available in the list of front-end licenses
								db.Licenses.Remove(existingLicense);
								// Save changes
								db.SaveChanges();
							}
						}

						// UPDATE old or INSERT new ones
						foreach (var license in user.Licenses)
						{
							var dbLicense = db.Licenses.Find(license.Id);
							if (dbLicense == null)
							{
								// new
								var newLicense = new License()
								{
									UserId = user.Id,
									ValidFrom = license.ValidFrom,
									ValidUntil = license.ValidUntil,
									Type = license.Type,
									Active = license.Active,
								};

								db.Licenses.Add(newLicense);
								db.SaveChanges();
							}
							else
							{
								// existing
								if (dbLicense.UserId.Equals(license.UserId))
								{
									dbLicense.Active = license.Active;
									db.Entry(dbLicense).State = EntityState.Modified;
									db.SaveChanges();
								}
							}
						}

					}

					// All fine, if we are here. Let's commit.
					transaction.Commit();
				}
				catch(Exception ex)
				{
					Debug.WriteLine(ex.Message);
					transaction.Rollback();
#warning @TODO: This not working well, not sending real Exception error message to the front-end. May need additional research, how to do this.
					return InternalServerError(ex);
				}
			}

			return Ok(true);
		}

		[Route("DateRanges")]
		public M.NamedDbItem<int>[] GetEnumDateRange ()
		{
			var retVal = new M.NamedDbItem<int>[] {
				new M.NamedDbItem<int>() { Id = (int)DateRange.Today, Name = "today" },
				new M.NamedDbItem<int>() { Id = (int)DateRange.ThisWeek, Name = "this-week" },
				new M.NamedDbItem<int>() { Id = (int)DateRange.ThisMonth, Name = "this-month" },
				new M.NamedDbItem<int>() { Id = (int)DateRange.ThisQuarter, Name = "this-quarter" },
				new M.NamedDbItem<int>() { Id = (int)DateRange.ThisYear, Name = "this-year" },
				new M.NamedDbItem<int>() { Id = (int)DateRange.Last24Hours, Name = "last24hours" },
				new M.NamedDbItem<int>() { Id = (int)DateRange.Last7Days, Name = "last7days" },
				new M.NamedDbItem<int>() { Id = (int)DateRange.Last12Months, Name = "last12months" },
			};
			return retVal;
		}

		[Route("DateRanges")]
		public object GetDateRangeParse(int dateRange)
		{
			IDateRangeData range = null;
			DatePartOrInterval? measureUnit = null;
			ChartAggregationType? aggregationType = null;

			var enumDateRange = Enum.Parse(typeof(DateRange), dateRange.ToString());

			switch (enumDateRange)
			{
				default:
				case DateRange.Today:
					range = DateRangeData.Today;
					measureUnit = DatePartOrInterval.Hour;
					break;

				case DateRange.ThisWeek:
					range = DateRangeData.ThisWeek;
					measureUnit = DatePartOrInterval.Day;
					break;

				case DateRange.ThisMonth:
					range = DateRangeData.ThisMonth;
					measureUnit = DatePartOrInterval.Week;
					break;

				case DateRange.ThisQuarter:
					range = DateRangeData.ThisQuarter;
					measureUnit = DatePartOrInterval.Week;
					break;

				case DateRange.ThisYear:
					range = DateRangeData.ThisYear;
					measureUnit = DatePartOrInterval.Month;
					break;

				case DateRange.Last24Hours:
					range = DateRangeData.Last24Hours;
					aggregationType = ChartAggregationType.RealValue;
					break;

				case DateRange.Last7Days:
					range = DateRangeData.Last7Days;
					measureUnit = DatePartOrInterval.Day;
					break;

				case DateRange.Last12Months:
					range = DateRangeData.Last12Months;
					measureUnit = DatePartOrInterval.Month;
					break;
			}

			return new
			{
				range = range,
				measureUnit = measureUnit,
				aggregationType = aggregationType,
			};
		}

		[Route("Aggregation")]
		public void GetAggregation()
		{

		}

	}
}