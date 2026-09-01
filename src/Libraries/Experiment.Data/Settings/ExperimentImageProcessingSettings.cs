using Experiment.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Settings{
	public class ExperimentImageProcessingSettings : ImageProcessingSettings
	{
		public ExperimentImageProcessingSettings()
		{
			//OriginalFolder = GetOriginalFilesPath();

			CreateNormal = true;
			//NormalFolder = GetNormalFilesPath();
			MaxNormalWidth = 1280;
			MaxNormalHeigh = 1280;

			CreateThumb = true;
			//ThumbFolder = GetThumbFilesPath();
			MaxThumbWidth = 800;
			MaxThumbHeigh = 600;
		}
	}
}
