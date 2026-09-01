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
using Website.Data;
using Website.Models;

namespace Website.Controllers
{
    [Authorize]
    public class LanguageController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Language
        [AllowAnonymous]
        public IQueryable<Language> GetLanguages()
        {
            return db.Languages;
        }
/*
        // GET: api/Language/5
        [ResponseType(typeof(Language))]
        public IHttpActionResult GetLanguage(string id)
        {
            Language language = db.Languages.Find(id);
            if (language == null)
            {
                return NotFound();
            }

            return Ok(language);
        }

        // PUT: api/Language/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutLanguage(string id, Language language)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != language.Code)
            {
                return BadRequest();
            }

            db.Entry(language).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LanguageExists(id))
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

        // POST: api/Language
        [ResponseType(typeof(Language))]
        public IHttpActionResult PostLanguage(Language language)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Languages.Add(language);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (LanguageExists(language.Code))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = language.Code }, language);
        }

        // DELETE: api/Language/5
        [ResponseType(typeof(Language))]
        public IHttpActionResult DeleteLanguage(string id)
        {
            Language language = db.Languages.Find(id);
            if (language == null)
            {
                return NotFound();
            }

            db.Languages.Remove(language);
            db.SaveChanges();

            return Ok(language);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
*/
        private bool LanguageExists(string id)
        {
            return db.Languages.Count(e => e.Code == id) > 0;
        }

    }
}