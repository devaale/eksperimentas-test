using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class ImageProcessingSettings : IImageProcessingSettings
	{

		public virtual string OriginalFolder { get; set; }

		public bool CreateNormal { get; set; }
		public virtual string NormalFolder { get; set; }
		public virtual int MaxNormalHeigh { get; set; }
		public virtual int MaxNormalWidth { get; set; }

		public bool CreateThumb { get; set; }
		public virtual string ThumbFolder { get; set; }
		public virtual int MaxThumbHeigh { get; set; }
		public virtual int MaxThumbWidth { get; set; }
	}
}
