using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Experiment.Maui{
	static class E
	{
		internal static bool Loaded = false;

		internal readonly static Dictionary<string, string> Words = new Dictionary<string, string>()
		{
/*			{"ok", "Ok"},
			{"cancel", "Cancel"},
			{"yes", "Yes"},
			{"no", "No"},
			{"warning", "Warning" },
			{"question", "Question"},
			{"are-sure-to-delete", "Do you want to delete this record?"},
			{"failed", "Failed"},
			{"na-info", "Not enough information" },
			{"nothing-selected", "Nothing selected!" },
			{"x-selected", "Selected {0} item(s)." },*/
		};

		/// <summary>
		/// Translation function for future 
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static string T(string alias)
		{
			if (string.IsNullOrEmpty(alias))
				return alias;
			if (Words.ContainsKey(alias))
			{
				var text = Words[alias];
				return string.IsNullOrEmpty(text) ? alias : text;
			}
			Debug.WriteLine(string.Format("Alias [{0}] not found!", alias));
			return alias;
		}

		public async static Task<bool> ProcessResponse(HttpResponseMessage httpMessage)
		{
			if (!httpMessage.IsSuccessStatusCode)
			{
				await Application.Current.MainPage.DisplayAlert(
					E.T("attention"),
					httpMessage.ReasonPhrase,
					E.T("ok"));
				return false;
			}

			return true;
		}
	}
}

