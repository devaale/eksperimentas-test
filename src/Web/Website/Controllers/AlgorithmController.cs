//#define REAL_DELETE // DON'T Enable
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
using Experiment.Data.Metadata;

using Website.Data;
using Website.Models;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/Algorithm")]
	public class AlgorithmController : ApiController
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
        /// It works but returning as well user's info
        /// </summary>
        /// <returns></returns>
        // GET: api/Algorithm
        public IList<IAlgorithm> GetAlgorithms(int objectId)
		{
            var result = db.Algorithms.Where(algorithm =>
                algorithm.ObjectId.Equals(objectId) &&
                    algorithm.Deleted == null);

            // Converting to list
            var retVal = new List<IAlgorithm>();
			foreach (var o in result)
			{
				retVal.Add(o);
			}
			return retVal;
		}

        // GET: api/Algorithm/5
        [Route("New")]
        [ResponseType(typeof(IAlgorithm))]
        public IAlgorithm GetNewAlgorithm()
        {
            var alg = new M.Algorithm();

            var result = new M.Algorithm()
            {
            };
			
            return result;
        }

        // PUT: api/Algorithm/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAlgorithm(int id, Algorithm algorithm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != algorithm.Id)
            {
                return BadRequest();
            }

            // Find object
            EObject obj = db.Objects.Find(algorithm.ObjectId);
            if (obj == null)
            {
                return NotFound();
            }

            // Check that user updated only own algorithms
            var userId = User.Identity.GetUserId();
            if (!userId.Equals(obj.UserId))
            {
                return StatusCode(HttpStatusCode.Conflict);
            }

            db.Entry(algorithm).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlgorithmExists(id))
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

        // POST: api/Algorithm
        [ResponseType(typeof(Algorithm))]
        public IHttpActionResult PostAlgorithm(Algorithm algorithm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Algorithms.Add(algorithm);
            var response = db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = algorithm.Id }, algorithm);
        }

        // DELETE: api/Algorithm/5
        [ResponseType(typeof(Algorithm))]
        public IHttpActionResult DeleteAlgorithm(int id)
        {
            Algorithm alg = db.Algorithms.Find(id);
#if REAL_DELETE
			if (alg == null)
			{
				return NotFound();
			}

            // Find object
            EObject obj = db.Objects.Find(alg.ObjectId);
            if (obj == null)
            {
                return NotFound();
            }

            // User can delete algorithms only in own object
            var userId = User.Identity.GetUserId();
			if (!userId.Equals(alg.ObjectId))
			{
				return StatusCode(HttpStatusCode.Conflict);
			}

			db.Algorithms.Remove(alg);
			db.SaveChanges();

			return Ok(alg);
#else
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != alg.Id)
            {
                return BadRequest();
            }

            // Find object
            EObject obj = db.Objects.Find(alg.ObjectId);
            if (obj == null)
            {
                return NotFound();
            }

            // Check that user updated algorithms only in own object
            var userId = User.Identity.GetUserId();
            if (!userId.Equals(obj.UserId))
            {
                return StatusCode(HttpStatusCode.Conflict);
            }

            // Mark as deleted
            alg.Deleted = DateTime.Now;

            db.Entry(alg).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlgorithmExists(id))
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

        #endregion
    }
}