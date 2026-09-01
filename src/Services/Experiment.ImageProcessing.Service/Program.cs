using System;
using System.Linq;

using Experiment.Data.Drawing;
using Experiment.Data.Enums;
using Experiment.Data.Metadata;
using Experiment.Data.Settings;

namespace Experiment.ImageProcessing.Service{
	internal class Program
	{
		const string FIND_PATTERN1 = "Experiment";
		const string FIND_PATTERN2 = "Experiment";
		const string PATH_POJECTED = "Web\\Files";

		static readonly IList<string> SUPPORTED_IMAGES_EXT = new List<string>() {
			"jpg",
			"jpeg",
			"gif",
			"png",
			"webp"
		};

		/// <summary>
		/// Startup
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			Console.WriteLine("Experiment Front-End Images Processing Utility.");
			Console.WriteLine(); 

			Process();

			Console.WriteLine();
			Console.WriteLine("Press Any Key To Continue...");
			Console.ReadKey();
		}

		/// <summary>
		/// Main processing routine
		/// </summary>
		static void Process()
		{
			Console.WriteLine("* Processing starting...");

			var settings = GetSettings();
			if(settings != null)
			{
				ImageProcessor ip = new ImageProcessor(settings);

				var di = new DirectoryInfo(settings.OriginalFolder);
				Console.WriteLine("* Starting to process original files in folder: " + di.FullName);
				var files = di.GetFiles();
				foreach(var file in files)
				{
					var isImage = SUPPORTED_IMAGES_EXT.Any(e => file.Extension.EndsWith(e));
					if(isImage)
					{
						Console.WriteLine(String.Format("* Processing {0}..", file.Name));
						ip.Process(file.Name, file.FullName);
					}
					else
					{
						Console.WriteLine(String.Format("* SKIPPING {0}!", file.Name));
					}

				}

			}
		}

		/// <summary>
		/// Settings retrieval
		/// </summary>
		/// <returns></returns>
		static IImageProcessingSettings? GetSettings()
		{
			Console.WriteLine("* Trying to construct Settings...");

			try
			{
				// Find Website/Files path
				var filesPath = FindRootPath();
				var originalFolder = new DirectoryInfo(Path.Combine(filesPath, "Original\\"));
				var normalFolder = new DirectoryInfo(Path.Combine(filesPath, "Normal\\"));
				var thumbFolder = new DirectoryInfo(Path.Combine(filesPath, "Thumb\\"));

				// Initialize different Image files folders
				if (!originalFolder.Exists)
					originalFolder.Create();

				if (!normalFolder.Exists)
					normalFolder.Create();

				if (!thumbFolder.Exists)
					thumbFolder.Create();

				// Initializing settings with Folder settings together
				var retVal = new ExperimentImageProcessingSettings()
				{
					OriginalFolder = originalFolder.FullName,
					NormalFolder = normalFolder.FullName,
					ThumbFolder = thumbFolder.FullName,
				};
				return retVal;
			}
			catch(Exception ex)
			{
				Console.WriteLine();
				Console.WriteLine("***");
				Console.WriteLine(ex.Message);
				Console.WriteLine("***");
			}
			return null;
		}

		/// <summary>
		/// In order to initialize settings need to initialize folder structure, path
		/// </summary>
		/// <returns></returns>
		static string FindRootPath()
		{
			Console.WriteLine(
				"* Trying to find root path in this app launch path...");

			// Getting assembly file location
			var loc = System.Reflection.Assembly.GetExecutingAssembly().Location;
			Console.WriteLine(
				"* Assembly location: " + loc);
			var aFi = new FileInfo(loc);

			var expDi = FindPath(aFi.Directory, FIND_PATTERN1);
			if(expDi != null)
			{
				Console.WriteLine("* Preliminary base path: " + expDi);
				var verified = false;
				
				if(expDi.Parent != null)
				{
					verified = expDi.Parent.Name.Equals(FIND_PATTERN2);
				}

				if(verified)
				{
					var projectedPath = Path.Combine(expDi.FullName, PATH_POJECTED);
					var diFiles = new DirectoryInfo(projectedPath);
					if(diFiles.Exists)
					{
						Console.WriteLine("* Discovered existing Files folder: " + diFiles.FullName);
						return diFiles.FullName;
					}
				}
			}

			throw new Exception(string.Format(
				"* Base path wasn't found!\r\n* Make sure you run this APP from Website or Experiment Project folders structure.\r\n* That was possible to find {0}\\{1} folder pattern.",
				FIND_PATTERN2, FIND_PATTERN1));
			
		}

		static DirectoryInfo? FindPath(DirectoryInfo? di, string pattern)
		{
			if (di == null)
				return null;

			if (di.Name.Equals(pattern))
				return di;

			return FindPath(di.Parent, pattern);
		}
	}
}