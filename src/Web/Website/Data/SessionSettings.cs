using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

using Microsoft.AspNet.Identity;

using Experiment.Core;
using Website.Controllers;
using Website.Models;

namespace Website.Data
{
	public static class SessionSettings 
	{
		#region Const
		/// <summary>
		/// This Class type name for Debug
		/// </summary>
		internal const string TYPE_NAME = nameof(SessionSettings);

		const string SETTINGS_PREFIX = "Exp_";
		const string SVAR_LANGUAGE = SETTINGS_PREFIX + "Language";

		#endregion

		#region Properties
		public static string Language
		{
			get
			{
				var vLoc = string.Format("{0}::{1}(GET)", TYPE_NAME, nameof(Language));
				if (IsSessionOk)
				{
					/*
					 * Idea here is like this, that User's prefered language is saved in database at AspNetUser table
					 * While I discovered that all other HTTP session variables, except ASP.net ones getting desroyed very often
					 * But they at least stay for same browsing session, but may disappear eg. after some minutes, maybe hour
					 * 
					 * While we have static Multilanguage class E (E.T), how to use it easier,
					 * I needed non-static solution to save specific user's prefered language
					 * As if to set it as static, it will affect every user, not only one
					 * 
					 * To call database every time, when need to take multilanguage word as well is not the case;
					 * 
					 * So I decided to save it in session, even if it getting destroyed pretty often, 
					 * after what possible to retake it back with this code
					 * 
					 * WARNING! We probably may discover better ideas in future, but I'm pretty green on this platform yet (2022-08-10)
					 * 
					 * This algorithm checks to session has language
					 */
					if (Session[SVAR_LANGUAGE] == null)
					{
						// If language wasn't found, we retrieving it from DB via EF, sort of it. 
						// ASP.net logged in user's session data, but there are no custom asp.net user properties
						var dbc = new ApplicationDbContext();
						//var um = new UserManager<ApplicationUser>(dbc); // redundant, left it just to show how to initialize

						// Retrieving logged in User's Id
						string currentUserId = HttpContext.Current.User.Identity.GetUserId();
						if(string.IsNullOrEmpty(currentUserId))
						{
							Session[SVAR_LANGUAGE] = Defaults.DEFAULT_LANGUAGE;
						} else
						{
							// Retrieving User's info
							ApplicationUser currentUser = dbc.Users.FirstOrDefault(x => x.Id == currentUserId);
							// Saving in session his prefered language
							Session[SVAR_LANGUAGE] = currentUser.Language;
						}
					}
					// Returning language from HTTP session
					return Session[SVAR_LANGUAGE].ToString();
				}
				else
				{
					throw new Exception(vLoc + ", Warning! HTTP Session doesn't exist!");
				}
			}

			/*
			 * This one is used from AccountController.Settings, when user changing language
			 * That it insstantly affected UI, elsewhere session has an old setting
			 * While this property using multilanguage Website.Data.E class to retrieve ML words
			 */
			set => Session[SVAR_LANGUAGE] = value;
		}

		internal static bool IsSessionOk { get => Session != null; }

		#region Static

		public static HttpSessionState Session { get => HttpContext.Current.Session; }

		#endregion // Static
		#endregion // Properties

		#region Methods
		#region Static

		#endregion // Static
		#endregion // Methods
	}
}