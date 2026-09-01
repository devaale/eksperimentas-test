//#define FORCE_GC_COLLECT

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Drawing{
	public class ImageProcessor
	{
		#region Const
		const string TYPE_NAME = nameof(ImageProcessor);

		#endregion

		#region Attributes
		IImageProcessingSettings _Settings;

		#endregion

		#region Ctor

		public ImageProcessor(IImageProcessingSettings settings)
		{
			_Settings = settings;
		}
		#endregion

		#region Helpers

		/// <summary>
		/// Normal image processing
		/// </summary>
		/// <param name="fileNameTemplate"></param>
		/// <param name="originalFilePath"></param>
		/// <returns></returns>
		protected bool ProcessNormal(string fileNameTemplate, string originalFilePath)
		{
			var vLoc = TYPE_NAME + "::" + nameof(ProcessNormal);
			//Debug.WriteLine(vLoc + ": " + fileNameTemplate);

			// Loading image for analysis
			var retVal = true;
			var srcBmp = Bitmap.FromFile(originalFilePath);
			var savePath = Path.Combine(_Settings.NormalFolder, fileNameTemplate);
			var srcSize = srcBmp.Size;
			var heightIsBigger = srcSize.Height > srcSize.Width;
			Size? destSize = null;

			if (heightIsBigger)
			{
				if (srcSize.Height > _Settings.MaxNormalHeigh)
				{
					double cof = (double)srcSize.Height / (double)_Settings.MaxNormalHeigh;
					destSize = new Size((int)(srcSize.Width / cof), (int)(srcSize.Height / cof));
				}
			}
			else
			{
				if (srcSize.Width > _Settings.MaxNormalWidth)
				{
					double cof = (double)srcSize.Width / (double)_Settings.MaxNormalWidth;
					destSize = new Size((int)(srcSize.Width / cof), (int)(srcSize.Height / cof));
				}
			}

			if (destSize.HasValue)
			{
				Debug.WriteLine(
					String.Format("{0}, {1}x{2} ==> {3}x{4}",
					fileNameTemplate,
					srcSize.Width, srcSize.Height, 
					destSize.Value.Width, destSize.Value.Height));

				var destBmp = ImageOperations.Resize(
					srcBmp, destSize.Value, srcBmp.RawFormat);
				if(destBmp != null)
				{
					destBmp.Save(savePath, srcBmp.RawFormat);
					destBmp.Dispose();
					destBmp = null;
				}
			}
			else
			{
				srcBmp.Save(savePath, srcBmp.RawFormat);
				srcBmp.Dispose();
				srcBmp = null;
			}

#if FORCE_GC_COLLECT
			GC.Collect();
#endif

			return retVal;
		}

		/// <summary>
		/// Thumbnail processing
		/// </summary>
		/// <param name="fileNameTemplate"></param>
		/// <param name="originalFilePath"></param>
		/// <returns></returns>
		protected bool ProcessThumb(string fileNameTemplate, string originalFilePath)
		{
			var vLoc = TYPE_NAME + "::" + nameof(ProcessThumb);
			//Debug.WriteLine(vLoc + ": " + fileNameTemplate);

			var retVal = false;
			var srcBmp = Bitmap.FromFile(originalFilePath);
			var savePath = Path.Combine(_Settings.ThumbFolder, fileNameTemplate);

			// Determining aspect ratios
			var dstWAr = (float)_Settings.MaxThumbWidth / (float)_Settings.MaxThumbHeigh;	// Destination AR from width
			var dstHAr = (float)_Settings.MaxThumbHeigh / (float)_Settings.MaxThumbWidth;	// Destination AR from height
			var srcWAr = (float)srcBmp.Width / (float)srcBmp.Height;	// Source AR from width

			Rectangle dstRect = new Rectangle(0, 0, _Settings.MaxThumbWidth, _Settings.MaxThumbHeigh);
			Rectangle srcRect;

			if (srcWAr > dstWAr)
			{
				// Width is bigger
				var size = new Size((int)(srcBmp.Height * dstWAr), srcBmp.Height);
				var loc = new Point((srcBmp.Width - size.Width) / 2, 0);
				srcRect = new Rectangle(loc, size);
			}
			else
			{
				// Source is higher
				var size = new Size(srcBmp.Width, (int)(srcBmp.Width * dstHAr));
				var loc = new Point(0, (srcBmp.Height - size.Height) / 2);
				srcRect = new Rectangle(loc, size);
			}

			Debug.WriteLine(
				String.Format("{0}({1}x{2}:{3}), {4}:{5},{6}x{7} ==> {8}:{9},{10}x{11}",
				fileNameTemplate,
				srcBmp.Width, srcBmp.Height, srcWAr,
				srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height,
				dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height));

			var dstBmp = ImageOperations.CropImage(srcBmp, dstRect, srcRect);
			dstBmp.Save(savePath, srcBmp.RawFormat);
			dstBmp.Dispose();
			dstBmp = null;

			return retVal;
		}

		#endregion

		#region Methods
		public bool Process(string fileNameTemplate, byte[] image)
		{
			// Write original file
			var fullPath = Path.Combine(_Settings.OriginalFolder, fileNameTemplate);
			File.WriteAllBytes(fullPath, image);

			return Process(fileNameTemplate, fullPath);
		}

		public bool Process(string fileNameTemplate, string originalFilePath)
		{
			var retVal = true;

			// File exists?
			if (!File.Exists(originalFilePath))
				return retVal;

			if(_Settings.CreateNormal)
				retVal &= ProcessNormal(fileNameTemplate, originalFilePath);

			if(_Settings.CreateThumb)
				retVal &= ProcessThumb(fileNameTemplate, originalFilePath);

			return retVal;
		}

		#endregion
	}
}
