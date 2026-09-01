using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Net.WebRequestMethods;

using Microsoft.Maui.Storage;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Experiment.Core;

using Experiment.Maui.Services;
using Experiment.Maui.ViewModels;

namespace Experiment.Maui.Data
{
    public static class Settings
    {
        #region Constant
        /// <summary>
        /// Prefix for all the settings
        /// </summary>
        internal const string D_PREF = "exp";

        /// <summary>
        /// Names
        /// </summary>
        internal const string NAME_LANG = D_PREF + "SysLang";
        internal const string NAME_SERVER = "srv";
        internal const string DEF_SERVER = "http://localhost/exp";

        // Login settings names
        internal const string NAME_USERNAME = D_PREF + "UName";
        internal const string NAME_AUTH_TOKEN = D_PREF + "Auth";
        internal const string NAME_AUTH_TOKEN_TYPE = NAME_AUTH_TOKEN + "Type";
        internal const string NAME_EXPIRES = D_PREF + "Expires";
        internal const string NAME_OBJECT = D_PREF + "ObjectId";
        //internal const string NAME_OBJECT_NAME = D_PREF + "ObjectName";
        internal const string NAME_GROUP = D_PREF + "Group";
        internal const string NAME_GROUP_NAME = D_PREF + "GroupName";
        internal const string NAME_ALGORITHM = D_PREF + "Algorithm";
        internal const string NAME_ALGORITHM_NAME = D_PREF + "AlgorithmName";

        // Login settings default values
        internal readonly static string DEF_USERNAME = string.Empty;
        internal readonly static string DEF_AUTH_TOKEN = string.Empty;
        internal readonly static string DEF_AUTH_TOKEN_TYPE = string.Empty;
        internal readonly static DateTime DEF_EXPIRES = DateTime.Now.AddDays(-1);

        // ASP.net auth token names
        internal const string JSON_ACCESS_TOKEN = "access_token";
        internal const string JSON_ACCESS_TYPE = "token_type";
        internal const string JSON_EXPIRES_IN = "expires_in";
        internal const string JSON_USERNAME = "userName";
        internal const string JSON_ISSUED = ".issued";
        internal const string JSON_EXPIRES = ".expires";
        #endregion

        #region Attributes

        #endregion

        #region Properties

        #region System properties

        /// <summary>
        ///
        /// Servers list
        ///
        /// When you want to switch to your sandbox, comment all until your environment is first
        /// And re-run application, after what it won't found previously set server and yours will be first
        /// Which app will set as default.
        ///
        /// After this, you can uncomment them, as in APP settings already saved your environment.
        /// This issue is only for devs, as users should see only production, who will test app in DEBUG mode compiled, will see servers options enabled.
        ///
        /// </summary>
        internal static readonly List<KeyValuePair<string, string>> Servers = new List<KeyValuePair<string, string>>()
        {
            //new KeyValuePair<string, string>("http://localhost:59812/",  "TEST"),
			//new KeyValuePair<string, string>("http://192.168.0.182/exp/",  "TEST"),	// VPN/LAN

			// HTTP
			//new KeyValuePair<string, string>("http://217.117.18.100:42900/exp/",  "TEST"),	// HTTP
			//new KeyValuePair<string, string>("http://exp.energus.ee:42900/exp/",  "TEST"),	// HTTP

			// HTTPS
			//new KeyValuePair<string, string>("https://217.117.18.100:42901/exp/",  "TEST"),	// ISP changed 2024-03-19
			new KeyValuePair<string, string>("https://exp.energus.ee:42901/exp/",  "TEST"),	// Hostname variant (2024-09-19)

			// Developers sandboxes
			new KeyValuePair<string, string>("http://terra/exp/",       "DEV_A"),	// Arvydas
			//new KeyValuePair<string, string>("http://192.168.0.7/exp/",	"DEV_D"),	// Dima?
			//new KeyValuePair<string, string>("http://roman/exp/",		"DEV_R"),	// Roman (nenaudotas?)
		};

        internal static KeyValuePair<string, string> CurrentServerKvp
        {
            get
            {
                // Getting saved server
                var savedServer = Preferences.Get(NAME_SERVER, Servers.FirstOrDefault().Key);

                // Declaring current server KeyValuePair
                KeyValuePair<string, string>? currentServer = null;

                // Searching do in legal servers list available such server, with such URL
                foreach (var server in Servers)
                {
                    if (server.Key.Equals(savedServer))
                    {
                        // Yes, available
                        currentServer = server;
                        break;
                    }
                }


                if (!currentServer.HasValue)
                {
                    // If legal server wasn't found, need to set currently default server
                    currentServer = Servers.FirstOrDefault();

                    // As well as to update server's settings
                    Server = currentServer.Value.Key;
                }

                return currentServer.Value;
            }
        }

        internal static string Language
        {
            get => Preferences.Get(NAME_LANG, Defaults.DEFAULT_LANGUAGE);
            set => Preferences.Set(NAME_LANG, value);
        }

        internal static string Server
        {
            get => CurrentServerKvp.Key;    // This one has server URL validity check
            set => Preferences.Set(NAME_SERVER, value);
        }

        internal static int ObjectId
        {
            get => Preferences.Get(NAME_OBJECT, 0);
            set => Preferences.Set(NAME_OBJECT, value);
        }

        //internal static string ObjectName
        //{
        //	get => Preferences.Get(NAME_OBJECT_NAME, string.Empty);
        //	set => Preferences.Set(NAME_OBJECT_NAME, value);
        //}

        internal static int Group
        {
            get => Preferences.Get(NAME_GROUP, 0);
            set => Preferences.Set(NAME_GROUP, value);
        }

        internal static string GroupName
        {
            get => Preferences.Get(NAME_GROUP_NAME, string.Empty);
            set => Preferences.Set(NAME_GROUP_NAME, value);
        }

        internal static int Algorithm
        {
            get => Preferences.Get(NAME_ALGORITHM, 0);
            set => Preferences.Set(NAME_ALGORITHM, value);
        }

        internal static string AlgorithmName
        {
            get => Preferences.Get(NAME_ALGORITHM_NAME, string.Empty);
            set => Preferences.Set(NAME_ALGORITHM_NAME, value);
        }
        #endregion

        #region Login stuff

        /// <summary>
        /// Logged in Email
        /// </summary>
        internal static string LoginUsername
        {
            get => Preferences.Get(NAME_USERNAME, DEF_USERNAME);
            set => Preferences.Set(NAME_USERNAME, value);
        }

        /// <summary>
        /// Full, constructed Auth token type
        /// </summary>
        internal static string LoginTokenType
        {
            get => Preferences.Get(NAME_AUTH_TOKEN_TYPE, DEF_AUTH_TOKEN_TYPE);
            set => Preferences.Set(NAME_AUTH_TOKEN_TYPE, value);
        }

        /// <summary>
        /// Full, constructed Auth token
        /// </summary>
        internal static string LoginToken
        {
            get => Preferences.Get(NAME_AUTH_TOKEN, DEF_AUTH_TOKEN);
            set => Preferences.Set(NAME_AUTH_TOKEN, value);
        }

        /// <summary>
        /// When login will expire
        /// </summary>
        internal static DateTime LoginExpires
        {
            get => Preferences.Get(NAME_EXPIRES, DEF_EXPIRES);
            set => Preferences.Set(NAME_EXPIRES, value);
        }

        /// <summary>
        /// Is user logged in
        /// </summary>
        public static bool IsLoggedIn
        {
            get => DateTime.Now < LoginExpires &&
                !String.IsNullOrEmpty(LoginToken) &&
                !String.IsNullOrEmpty(LoginUsername);
        }

        #endregion

        #endregion // Properties

        #region Helpers

        #endregion

        #region Methods
        /// <summary>
        /// Deserializes login token to settings
        /// </summary>
        /// <param name="json"></param>
        internal static void DeserializeLoginToken(string json)
        {
            JObject jd = JsonConvert.DeserializeObject<dynamic>(json);

            // Token type, eg. Bearer
            var tokenType = jd.Value<string>(JSON_ACCESS_TYPE);
            LoginTokenType = tokenType;

            // Token itself
            var accessToken = jd.Value<string>(JSON_ACCESS_TOKEN);
            LoginToken = accessToken;

            //var expiresIn = jd.Value<int>(JSON_EXPIRES_IN);
            LoginUsername = jd.Value<string>(JSON_USERNAME);

            //var issued = jd.Value<DateTime>(JSON_ISSUED);
            LoginExpires = jd.Value<DateTime>(JSON_EXPIRES);
        }

        internal static async Task Logout()
        {
            if (Settings.IsLoggedIn && !string.IsNullOrEmpty(Settings.Server))
            {
                var srv = new ApiServices();
                await srv.UserLogoutAsync();
            }

            Dictionaries.Instance.Logout();

            Settings.ObjectId = -1;
            Settings.LoginUsername = DEF_USERNAME;
            Settings.LoginTokenType = DEF_AUTH_TOKEN_TYPE;
            Settings.LoginToken = DEF_AUTH_TOKEN;
            Settings.LoginExpires = DEF_EXPIRES;
        }

        #endregion

    }
}