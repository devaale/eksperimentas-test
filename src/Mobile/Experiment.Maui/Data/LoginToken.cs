//#define STJS	// Serialization using System.Text.Json.Serialization NuGet Library had problems with DateTimes (didn't dig very deep)

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

#if STJS
using System.Text.Json.Serialization;
#else
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

namespace Experiment.Maui.Data{
	public class LoginToken
	{
		#region Constants
		public const string TYPE = "LoginToken";

		internal const string NAME_ACCESS_TOKEN = "access_token";
		internal const string NAME_ACCESS_TYPE = "token_type";
		internal const string NAME_EXPIRES_IN = "expires_in";
		internal const string NAME_USERNAME = "userName";
		internal const string NAME_ISSUED = ".issued";
		internal const string NAME_EXPIRES = ".expires";

		#endregion

		#region Properties

		#region Serializable

#if STJS
		[JsonPropertyName(LoginToken.NAME_ACCESS_TOKEN)]
#endif
		public string AccessToken { get; set; }

#if STJS
		[JsonPropertyName(LoginToken.NAME_ACCESS_TYPE)]
#endif
		public string TokenType { get; set; }

#if STJS
		[JsonPropertyName(LoginToken.NAME_EXPIRES_IN)]
#endif
		public int ExpiresIn { get; set; }

#if STJS
		[JsonPropertyName(LoginToken.NAME_USERNAME)]
#endif
		public string Username { get; set; }

#if STJS
		[JsonPropertyName(LoginToken.NAME_ISSUED)]
#endif
		public DateTime Issued { get; set; }

#if STJS
		[JsonPropertyName(LoginToken.NAME_EXPIRES)]
#endif
		public DateTime Expires { get; set; }

		#endregion

		#region Other

		public bool IsOk => !String.IsNullOrEmpty(AccessToken) && !string.IsNullOrEmpty(TokenType)
					&& !String.IsNullOrEmpty(Username) && DateTime.Now < Expires;

		#endregion

		#endregion

		#region Methods
		internal string ToAuthString()
		{
			var sb = new StringBuilder();

			if(IsOk)
			{
				sb.Append(TokenType);
				sb.Append(" ");
				sb.Append(AccessToken);
			}

			return sb.ToString();
		}

		#endregion

		#region Static

		public static LoginToken Deserialize(string json)
		{
			var wLoc = TYPE + "::Deserialize()";

			LoginToken retVal = new LoginToken();

			JObject jd = JsonConvert.DeserializeObject<dynamic>(json);
			retVal.AccessToken = jd.Value<string>(LoginToken.NAME_ACCESS_TOKEN);
			retVal.TokenType = jd.Value<string>(LoginToken.NAME_ACCESS_TYPE);
			retVal.ExpiresIn = jd.Value<int>(LoginToken.NAME_EXPIRES_IN);
			retVal.Username = jd.Value<string>(LoginToken.NAME_USERNAME);
			retVal.Issued = jd.Value<DateTime>(LoginToken.NAME_ISSUED);
			retVal.Expires = jd.Value<DateTime>(LoginToken.NAME_EXPIRES);

			Debug.WriteLine(string.Format("{0}: {1}",
				wLoc, retVal.IsOk ? "Okay!" : "Failed..."));

			return retVal;
		}

		#endregion
	}
}
