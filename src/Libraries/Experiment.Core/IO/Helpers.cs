using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Core.IO{
	public class Helpers
	{

		/// <summary>
		/// Write string to file without any aditional data added to output
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public static bool Write (string fileName, string data)
		{
			bool retVal = true;

			try
			{
				File.AppendAllText(fileName, data);
			}
			catch (Exception ex)
			{
				retVal = false;
				DebugOut(ex);
			}
			finally
			{

			}

			return retVal;
		}

		/// <summary>
		/// Binary write to file with default encoding
		/// 
		/// Samples:
		/// 
		/// System.Text;.Encoding.GetBytes(string)
		/// System.Text;.Encoding.Default.GetBytes(string)
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static bool Write (string fileName, byte[] bytes)
		{
			bool retVal = true;

			try
			{
				using (var stream = new FileStream(fileName, FileMode.Append))
				{
					stream.Write(bytes, 0, bytes.Length);
				}
			}
			catch (Exception ex)
			{
				retVal = false;
				DebugOut(ex);
			}
			finally
			{

			}

			return retVal;
		}

		/// <summary>
		/// It automatically will add the end of the line at the end of the text
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public static bool WriteLine (string fileName, string msg)
		{
			bool retVal = true;

			try
			{
				File.AppendAllText (fileName, msg + Environment.NewLine);
			}
			catch (Exception ex)
			{
				retVal = false;
				DebugOut(ex);
			}
			finally
			{

			}

			return retVal;
		}

		public static void DebugOut(Exception ex)
		{
			Debug.Print(ex.Message + Environment.NewLine + ex.StackTrace);
		}
	}
}
