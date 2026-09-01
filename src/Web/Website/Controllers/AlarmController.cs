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
using Experiment.Data.Enums;
using Experiment.Data.Metadata;

using Website.Data;
using Website.Models;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/Alarm")]
	public class AlarmController : ApiController
	{

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

		private bool AlgorithmExists(int id)
		{
			return db.Algorithms.Count(e => e.Id == id) > 0;
		}

		#endregion

		#region Methods
		/// <summary>
		/// </summary>
		/// <returns></returns>
		// GET: api/Algorithm
		public IQueryable<Algorithm> GetAlarms(int objectId)
		{
			return db.Algorithms
				.Where(a =>
					a.ObjectId.Equals(objectId) &&
					a.Type == AlgorithmType.Alarm &&
					a.Deleted == null)
				.OrderByDescending(a => 
					a.EventTime);
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		// GET: api/Algorithm
		public IQueryable<Algorithm> GetAlarms(int objectId, decimal status)
		{
			return db.Algorithms
				.Where(a =>
					a.ObjectId.Equals(objectId) &&
					a.Type == AlgorithmType.Alarm &&
					a.Status == status &&
					a.Deleted == null)
				.OrderByDescending(a => 
					a.EventTime);
		}

		[Route("Read")]
		[HttpGet]
		public void ReadAlarms(int objectId)
		{
			using (var transaction = db.Database.BeginTransaction())
			{
				var alarms = db.Algorithms.Where(a =>
					a.ObjectId == objectId &&
					a.Type == AlgorithmType.Alarm &&
					a.Status == 1 &&
					a.Read == null
				);

				foreach (var alarm in alarms)
				{
					alarm.Read = DateTime.Now;
					db.Entry(alarm).State = EntityState.Modified;
					db.SaveChanges();
				}

				transaction.Commit();
			}
		}

		#endregion

		#region Deprecated
#if DEPRECATED_20231220
		/// <summary>
		/// </summary>
		/// <returns></returns>
		// GET: api/Algorithm
		public IList<IAlgorithm> GetAlarms(int objectId)
		{
            var result = db.Algorithms.Where(algorithm =>
                algorithm.ObjectId.Equals(objectId) &&
                    algorithm.Type == AlgorithmType.Alarm &&
                    algorithm.Deleted == null);

            // Converting to list
            var retVal = new List<IAlgorithm>();
			foreach (var o in result)
			{
				retVal.Add(o);
			}
			return retVal;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		// GET: api/Algorithm
		public IList<IAlgorithm> GetAlarms(int objectId, decimal status)
		{
			var result = db.Algorithms.Where(algorithm =>
				algorithm.ObjectId.Equals(objectId) &&
					algorithm.Type == AlgorithmType.Alarm &&
					algorithm.Status == status &&
					algorithm.Deleted == null);

			// Converting to list
			var retVal = new List<IAlgorithm>();
			foreach (var o in result)
			{
				retVal.Add(o);
			}
			return retVal;
		}
#endif


		#endregion

	}
}