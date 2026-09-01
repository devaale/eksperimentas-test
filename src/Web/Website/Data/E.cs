using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Experiment.Core;

using Website.Models;
using Website.Controllers;

namespace Website.Data
{
	/// <summary>
	/// Class responsible for multilanguage
	/// </summary>
	public static class E
	{
		#region Const

		#endregion

		#region Attributes

		private static List<Language> _Languages = new List<Language>();
		public static IDictionary<string, IDictionary<string, string>> _Words =
			new Dictionary<string, IDictionary<string, string>>();

		#endregion

		#region Properties

		public static List<Language> Languages
		{
			get
			{
				if (_Languages.Count < 2)
				{
					var lc = new LanguageController();
					var languages = lc.GetLanguages();
					_Languages.AddRange(languages);
				}
				return _Languages;
			}
		}

		public static IDictionary<string, IDictionary<string, string>> Words
		{
			get
			{
				if(_Words.Count < 1)
				{
					var wc = new WordController();
					_Words.Clear();
					IEnumerable<Word> words = wc.GetAllWords();
					if (words != null)
					{
						foreach (var w in words)
						{
							// If language not added, adding language
							if (!_Words.ContainsKey(w.Code))
							{
								_Words.Add(w.Code, new Dictionary<string, string>());
							}

							// adding alias, word to specific language collection
							// Fix against bugging in this place for some reason (we not analyzed it, but was multiple atempts to add same alias in same language, might be worth to study real cause, this is only workaround!
							if (_Words[w.Code].ContainsKey(w.Alias))
							{
								_Words[w.Code][w.Alias] = w.Text;
							}
							else
							{
								_Words[w.Code].Add(w.Alias, w.Text);
							}
							
						}
					}
				}

				return _Words;
			}
		}

		#endregion

		#region Ctor

		static E()
		{
		}

		#endregion

		#region Helpers

		#endregion

		#region Methods

		/// <summary>
		/// Returns multilingual translated word by specified alias
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static string T(string alias)
		{
			return T(alias, SessionSettings.Language);
		}

		/// <summary>
		/// Returns multilingual translated word by specified language and alias
		/// </summary>
		/// <param name="language"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static string T(string alias, string language)
		{
			if (string.IsNullOrEmpty(language))
				throw new ArgumentNullException("language");

			if (string.IsNullOrEmpty(alias))
				throw new ArgumentNullException("alias");

			if (Words[language].ContainsKey(alias))
			{
				return Words[language][alias];
			}
			else
			{
				return language + ":" + alias;
			}
		}

		#endregion
	}
}