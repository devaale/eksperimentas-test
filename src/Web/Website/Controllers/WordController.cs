using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Description;

using Website.Data;
using Website.Models;

namespace Website.Controllers
{
    [RoutePrefix("api/Word")]
    public class WordController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Word/en
        [ResponseType(typeof(IQueryable<Word>))]
        public IQueryable<Word> GetWords(string lang)
        {
            var language = lang.ToLower();
            if (language.Equals("current"))
            {
                var ec = new ExperimentApiController();
                language = ec.GetLanguage();
            }

            if (string.IsNullOrEmpty(language))
                throw new ArgumentNullException(nameof(lang));

            // If specific language exist
            if (db.Languages.Count(e => e.Code == language) > 0)
			{
                // returning its words
                return db.Words.Where(
                    word => word.Code.Equals(language));
            }

            throw new ArgumentException(nameof(lang));
        }

        // GET: api/Word
        [ResponseType(typeof(IQueryable<Word>))]
        public IQueryable<Word> GetAllWords()
        {
            return db.Words;
        }

        /// <summary>
        /// Reset cached multilanguage words
        /// </summary>
        /// <returns></returns>
        [Route("Reset")]
        public string GetReset()
		{
            E.Words.Clear();

            return "OK";
		}

        /*
                // GET: api/Word/5
                [ResponseType(typeof(Word))]
                public IHttpActionResult GetWord(string id)
                {
                    Word word = db.Words.Find(id);
                    if (word == null)
                    {
                        return NotFound();
                    }

                    return Ok(word);
                }

                // PUT: api/Word/5
                [ResponseType(typeof(void))]
                public IHttpActionResult PutWord(string id, Word word)
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    if (id != word.Alias)
                    {
                        return BadRequest();
                    }

                    db.Entry(word).State = EntityState.Modified;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!WordExists(id))
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

                // POST: api/Word
                [ResponseType(typeof(Word))]
                public IHttpActionResult PostWord(Word word)
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    db.Words.Add(word);

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateException)
                    {
                        if (WordExists(word.Alias))
                        {
                            return Conflict();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return CreatedAtRoute("DefaultApi", new { id = word.Alias }, word);
                }

                // DELETE: api/Word/5
                [ResponseType(typeof(Word))]
                public IHttpActionResult DeleteWord(string id)
                {
                    Word word = db.Words.Find(id);
                    if (word == null)
                    {
                        return NotFound();
                    }

                    db.Words.Remove(word);
                    db.SaveChanges();

                    return Ok(word);
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
        private bool WordExists(string id)
        {
            return db.Words.Count(e => e.Alias == id) > 0;
        }

    }
}