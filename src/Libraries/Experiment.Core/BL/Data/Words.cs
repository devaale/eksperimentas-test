using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Core.BL.Data{
	/// <summary>
	/// Multilingual module
	/// </summary>
	public static class Words
	{
		#region Constants
		public const string LANG_LT = "lt";
		public const string LANG_EN = "en";
		public const string LANG_RU = "ru";

		public const string LANG_DEFAULT = LANG_LT;

		#endregion

		#region Attributes
		static Dictionary<string, Dictionary<string, string>> _Word;

		#endregion

		#region Properties
		public static List<string> AvailableLanguages;

		/// <summary>
		/// First array index is language, 2nd is word alias
		/// 
		/// Usage:
		/// var aa = Words.Word['lt']['value'];
		/// 
		/// </summary>
		private static Dictionary<string, Dictionary<string, string>> Word
		{
			get
			{
				if (_Word == null)
				{
					LoadData();
				}

				return _Word;
			}
		}

		#endregion

		#region Ctor

		static Words()
		{
			AvailableLanguages = new List<string>(new string[] { LANG_LT, LANG_EN, LANG_RU, });
		}

		#endregion

		#region Helpers
		private static void LoadData()
		{
			_Word = new Dictionary<string, Dictionary<string, string>>();

			ExpSql db = ExpSql.GenerateFromDefaults(null);
			DataTable table = db.MultilanguageWordsAll();
			foreach (DataRow row in table.Rows)
			{
				string language = row[Defaults.DB_LANGUAGE_ID].ToString();

				if (!_Word.ContainsKey(language))
				{
					_Word.Add(language, new Dictionary<string, string>());
				}
				_Word[language][row[Defaults.DB_ALIAS].ToString()] = row[Defaults.DB_TEXT].ToString();
			}
		}

		#endregion

		#region Methods
		public static string ParseLanguageId (string languageId)
		{
			if (string.IsNullOrEmpty(languageId))
				return LANG_DEFAULT;

			if (AvailableLanguages.Contains(languageId.ToLower()))
			{
				return languageId;
			}
			return LANG_DEFAULT;
		}

		public static string GetWord(string languageId, string alias)
		{
			if(Word.ContainsKey(languageId))
			{
				if(Word[languageId].ContainsKey(alias))
				{
					return Word[languageId][alias];
				}
			} else
			{
				ExpSql db = ExpSql.GenerateFromDefaults(null);
				db.MultilanguageUpdateAlias(alias);
			}

			return languageId + ": " + alias;
		}

		#endregion
	}
}
