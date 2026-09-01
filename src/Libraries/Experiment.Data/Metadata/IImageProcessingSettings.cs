using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IImageProcessingSettings
	{
		string OriginalFolder { get; }

		bool CreateNormal { get; }
		string NormalFolder { get; }
		int MaxNormalHeigh { get; }
		int MaxNormalWidth { get; }

		bool CreateThumb { get; }
		string ThumbFolder { get; }
		int MaxThumbHeigh { get; }
		int MaxThumbWidth { get; }
	}
}
