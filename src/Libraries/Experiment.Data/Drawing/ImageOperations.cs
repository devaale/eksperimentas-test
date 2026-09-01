using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace Experiment.Data.Drawing{
	public class ImageOperations
	{
		/// <summary>
		/// Creates image of specific size and type.
		/// 
		/// Portions of code from https://stackoverflow.com/a/24979829
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="enuType"></param>
		/// <returns></returns>
		public static Bitmap CreateImage(int width, int height, ImageFormat enuType)
		{
			Bitmap retVal;

			if (enuType == ImageFormat.Png)
				retVal = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			else if (enuType == ImageFormat.Gif)
				retVal = new Bitmap(width, height); //PixelFormat.Format8bppIndexed should be the right value for a GIF, but will throw an error with some GIF images so it's not safe to specify.
			else
				retVal = new Bitmap(width, height, PixelFormat.Format24bppRgb);

			//For some reason the resolution properties will be 96, even when the source image is different, so this matching does not appear to be reliable.
			//bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

			//If you want to override the default 96dpi resolution do it here
			//bmPhoto.SetResolution(72, 72);

			return retVal;
		}

		public static Bitmap CropImage(
			Image imgSource, 
			Rectangle dstRect, 
			Rectangle srcRect)
		{
			Bitmap bmPhoto = ImageOperations.CreateImage(
				dstRect.Width, dstRect.Height, imgSource.RawFormat);

			Graphics grPhoto = Graphics.FromImage(bmPhoto);
			grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
			grPhoto.DrawImage(imgSource, dstRect, srcRect, GraphicsUnit.Pixel);
			grPhoto.Dispose();

			return bmPhoto;
		}

		/// <summary>
		/// Creates from original image new resized image of specific image type
		/// 
		/// Portions code taken from: https://stackoverflow.com/a/24979829
		/// </summary>
		/// <param name="imgSource"></param>
		/// <param name="size"></param>
		/// <param name="imageType"></param>
		/// <returns></returns>
		public static Bitmap Resize(Image imgSource, Size size, ImageFormat imageType)
		{
			var srcRec = SizeToRectangle(imgSource);
			var destRec = SizeToRectangle(size);

			return CropImage(imgSource, destRec, srcRec);
		}


		#region Utils

		/// <summary>
		/// Converts Size to Rectangle with 0 X and Y coordinates
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static Rectangle SizeToRectangle(Size size)
		{
			return new Rectangle(new Point(0, 0), size);
		}

		/// <summary>
		/// Takes size from Image
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		public static Rectangle SizeToRectangle(Image image)
		{
			return new Rectangle(new Point(0, 0), image.Size);
		}

		#endregion

	}
}
