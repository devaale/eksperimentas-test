using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Experiment.Core.Metadata;


namespace Experiment.Core{
	/// <summary>
	/// Various utils
	/// </summary>
	public class Utils
	{
		const int HUMAN_BEHAVIOR_MIN_PAUSE_MS = 626;
		const int HUMAN_BEHAVIOR_MAX_PAUSE_MS = 3146;
		/// <summary>
		/// Convert int array to byte array
		/// </summary>
		/// <param name="ints"></param>
		/// <returns>bytes array or NULL if int array is NULL</returns>
		public static byte[] Convert(Int16[] ints)
		{
			if (ints == null)
			{
				return null;
			}
			else
			{
				byte[] result = new byte[ints.Length * sizeof(Int16)];
				Buffer.BlockCopy(ints, 0, result, 0, result.Length);
				return result;
			}

		}

		/// <summary>
		/// To sleep for a second
		/// </summary>
		public static void SleepForSecond()
		{
			Thread.Sleep(1000);
		}

		/// <summary>
		/// Mimics human actions pause
		/// </summary>
		public static void HumanBehaviorPause(ILogger logger)
		{
			if (logger != null)
				logger.WriteLine(5, "*** HUMAN BEHAVIOR PAUSE ***");

			Thread.Sleep(new Random().Next(HUMAN_BEHAVIOR_MIN_PAUSE_MS, HUMAN_BEHAVIOR_MAX_PAUSE_MS));
		}

		/// <summary>
		/// Splits DataTable to several DataTable IEnumerable batches with max [recordsPerTable] records per table
		/// </summary>
		/// <param name="table">Source DataTable</param>
		/// <param name="recordsPerTable">Max records per table</param>
		/// <returns></returns>
		public static IEnumerable<DataTable> SplitDataTablePerRecords(DataTable table, int recordsPerTable)
		{
			// If table has not more than max records per Batch, returning just it
			if (table.Rows.Count <= recordsPerTable)
			{
				yield return table;
			}
			else
			{
				var subjectTable = table.Clone();

				foreach (DataRow row in table.Rows)
				{
					subjectTable.Rows.Add(row.ItemArray);

					if (subjectTable.Rows.Count >= recordsPerTable)
					{
						yield return subjectTable;

						subjectTable = table.Clone();
					}
				}

				yield return subjectTable;
			}
		}

		/// <summary>
		/// Converts to file name, what means removal of illegal characters,
		/// replacement of them to _
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string ConvertToFileName(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				text = "DefaultFileName";
			}

			var invalid = Path.GetInvalidFileNameChars();
			foreach (var c in invalid)
			{
				text = text.Replace(c.ToString(), "_");
			}

			return text;
		}

		/// <summary>
		/// Clones the object with the same properties and their types
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="o"></param>
		/// <returns></returns>
		public static T Clone<T>(object o) where T: new ()
		{
			T retVal = new T();

			var srcProps = o.GetType().GetProperties();
			var dstProps = retVal.GetType().GetProperties();


			if(srcProps != null && dstProps != null)
			{
				foreach (var dstProp in dstProps)
				{
					if(dstProp.CanWrite)
					{
						var srcProp = srcProps.First(
							sp => sp.Name.Equals(dstProp.Name) && 
							sp.PropertyType.Equals(dstProp.PropertyType));

						if(srcProp != null)
						{
							dstProp.SetValue(retVal, srcProp.GetValue(o));
						}
					}
				}
			}

			return retVal;
		}

		public static void Dump(Exception ex)
		{
			Dump(ex, 1);
		}

		public static void Dump(Exception ex, int num)
		{
			Debug.WriteLine(string.Format("[{0}] {1}" + Environment.NewLine, num, ex.Message));

			if (ex.InnerException != null)
			{
				num++;
				Dump(ex.InnerException, num);
			}
		}

		/// <summary>
		/// Shorten string to specific length
		/// 
		/// And if it had larger length, adds as well .. at the end
		/// If not, not adding anything, just returning given string
		/// </summary>
		/// <param name="str"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string ShortenTo(string str, int length)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			else
			{
				if(str.Length > length)
				{
					return $"{str.Substring(0, length)}..";
				}
				else
				{
					return str;
				}

			}
		}
	}
}
