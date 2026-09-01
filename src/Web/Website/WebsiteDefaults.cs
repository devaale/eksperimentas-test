using System;
using System.IO;
using System.Web;

using Experiment.Data.Metadata;
using Experiment.Data.Models;
using Experiment.Data.Settings;

namespace Website
{
	internal class WebsiteDefaults
	{
		internal const string FLD_FILES = @"..\Files\";
		internal const string FLD_FILES_ORIGINAL = FLD_FILES + @"Original\";
		internal const string FLD_FILES_NORMAL = FLD_FILES + @"Normal\";
		internal const string FLD_FILES_THUMB = FLD_FILES + @"Thumb\";

		internal const string FLD_LOGS = @"..\Logs\";

		static IImageProcessingSettings _ImageProcessingSettings;
		public static IImageProcessingSettings ImageProcessingSettings
		{
			get
			{
				if (_ImageProcessingSettings == null)
				{
					_ImageProcessingSettings = new ExperimentImageProcessingSettings()
					{
						OriginalFolder = GetOriginalFilesPath(),
						NormalFolder = GetNormalFilesPath(),
						ThumbFolder = GetThumbFilesPath(),
					};
				}
				return _ImageProcessingSettings;
			}
		}


		/// <summary>
		/// Server root path
		/// </summary>
		/// <returns></returns>
		internal static string GetWebsiteRootPath()
		{
			return HttpContext.Current.Server.MapPath("~");
		}

		internal static string GetFilesPath()
		{
			return Path.GetFullPath(Path.Combine(GetWebsiteRootPath(), FLD_FILES));
		}

		internal static string GetOriginalFilesPath()
		{
			return Path.GetFullPath(Path.Combine(GetWebsiteRootPath(), FLD_FILES_ORIGINAL));
		}
		internal static string GetNormalFilesPath()
		{
			return Path.GetFullPath(Path.Combine(GetWebsiteRootPath(), FLD_FILES_NORMAL));
		}

		internal static string GetThumbFilesPath()
		{
			return Path.GetFullPath(Path.Combine(GetWebsiteRootPath(), FLD_FILES_THUMB));
		}
		internal static string GetLogsPath()
		{
			return Path.GetFullPath(Path.Combine(GetWebsiteRootPath(), FLD_LOGS));
		}
	}
}