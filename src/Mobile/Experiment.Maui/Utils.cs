using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

using Microsoft.Maui.Controls;

using DXC = DevExpress.Maui.Charts;

using Experiment.Core;
using Experiment.Core.Enums;
using Experiment.Data.Enums;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.ViewModels.Devices;
using Experiment.Maui.Models;

namespace Experiment.Maui{
    internal class Utils
	{
		const string TYPE_NAME = nameof(Utils);
		const bool DEBUG = false;

		/// <summary>
		/// Convert devices data in grouped for ListView
		/// 
		/// @TODO improve or simplify algorithm
		/// </summary>
		/// <typeparam name="T">Type for grouping</typeparam>
		/// <param name="groupedDevices">Destination, where to update the devices</param>
		/// <param name="devices">Devices normal IEnumerable array</param>
		/// <param name="getGroupingKey">getGroupingKey delegate function, which returns grouping value</param>
		/// <returns></returns>
		internal static void GroupDevices<T>(
			ObservableCollection<Grouping<T, VisualDevice>> groupedDevices, 
			IEnumerable<VisualDevice> devices, 
			Func<VisualDevice, T> getGroupingKey)
		{
			//return Group<T, DeviceViewModel>(groupedDevices, devices, getGroupingKey);
			// Grouping items
			Dictionary<T, List<VisualDevice>> groupedData = new Dictionary<T, List<VisualDevice>>();
			foreach(var device in devices)
			{
				T key = getGroupingKey(device);
				if(!groupedData.ContainsKey(key))
				{
					groupedData.Add(key, new List<VisualDevice>());
				}
				groupedData[key].Add(device);
			}

			// Delivering them in required format
			groupedDevices.Clear();
			foreach(T key in groupedData.Keys)
			{
				groupedDevices.Add(new Grouping<T, VisualDevice>(key, groupedData[key]));
			}
		}

		internal static void Group<T,O>(
			ObservableCollection<Grouping<T, O>> groupedItems,
			IEnumerable<O> items,
			Func<O, T> getGroupingKey)
		{
			var vLoc = string.Format("{0}::{1}(..", TYPE_NAME, nameof(Group));
			Debug.WriteLineIf(DEBUG, vLoc);

			// Grouping items
			Dictionary<T, List<O>> groupedData = new Dictionary<T, List<O>>();
			foreach (var item in items)
			{
				T key = getGroupingKey(item);
				if (!groupedData.ContainsKey(key))
				{
					groupedData.Add(key, new List<O>());
				}
				groupedData[key].Add(item);
			}

			// Delivering them in required format
			groupedItems.Clear();
			foreach (T key in groupedData.Keys)
			{
				groupedItems.Add(new Grouping<T, O>(key, groupedData[key]));
			}
		}

		/// <summary>
		/// Converts our DatePartOrInterval enum value to DXC.DateTimeMeasureUnit
		/// </summary>
		/// <param name="dp"></param>
		/// <returns></returns>
		internal static DXC.DateTimeMeasureUnit ToDxcDateTimeMeasureUnit(DatePartOrInterval dp)
		{
			switch(dp)
			{
				case DatePartOrInterval.Millisecond:
					return DXC.DateTimeMeasureUnit.Millisecond;

				case DatePartOrInterval.Second:
					return DXC.DateTimeMeasureUnit.Second;

				case DatePartOrInterval.Minute:
					return DXC.DateTimeMeasureUnit.Minute;

				case DatePartOrInterval.Hour:
					return DXC.DateTimeMeasureUnit.Hour;

				case DatePartOrInterval.Day:
					return DXC.DateTimeMeasureUnit.Day;

				case DatePartOrInterval.Week:
					return DXC.DateTimeMeasureUnit.Week;

				case DatePartOrInterval.Month:
					return DXC.DateTimeMeasureUnit.Month;

				case DatePartOrInterval.Quarter:
					return DXC.DateTimeMeasureUnit.Quarter;

				case DatePartOrInterval.Year:
					return DXC.DateTimeMeasureUnit.Year;

				default:
				case DatePartOrInterval.None:
					return DXC.DateTimeMeasureUnit.Default;
			}
		}

		/// <summary>
		/// Converts DXC.DateTimeMeasureUnit enum value to our DatePartOrInterval 
		/// </summary>
		/// <param name="dp"></param>
		/// <returns></returns>
		internal static DatePartOrInterval ToOurDatePartOrInterval(DXC.DateTimeMeasureUnit mu)
		{
			switch (mu)
			{
				case DXC.DateTimeMeasureUnit.Millisecond:
					return DatePartOrInterval.Millisecond;

				case DXC.DateTimeMeasureUnit.Second:
					return DatePartOrInterval.Second;

				case DXC.DateTimeMeasureUnit.Minute:
					return DatePartOrInterval.Minute;

				case DXC.DateTimeMeasureUnit.Hour:
					return DatePartOrInterval.Hour;

				case DXC.DateTimeMeasureUnit.Day:
					return DatePartOrInterval.Day;

				case DXC.DateTimeMeasureUnit.Week:
					return DatePartOrInterval.Week;

				case DXC.DateTimeMeasureUnit.Month:
					return DatePartOrInterval.Month;

				case DXC.DateTimeMeasureUnit.Quarter:
					return DatePartOrInterval.Quarter;

				case DXC.DateTimeMeasureUnit.Year:
					return DatePartOrInterval.Year;

				default:
				case DXC.DateTimeMeasureUnit.Default:
					return DatePartOrInterval.None;
			}
		}

		/// <summary>
		/// Converts our ChartAggregationType enum value to DXC.AggregationType
		/// </summary>
		/// <param name="dp"></param>
		/// <returns></returns>
		internal static DXC.AggregationType ToDxcAggregationType(ChartAggregationType at)
		{
			switch (at)
			{
				default:
				case ChartAggregationType.AverageValue:
					return DXC.AggregationType.Average;

				case ChartAggregationType.SumValue:
					return DXC.AggregationType.Sum;

				case ChartAggregationType.MinimalValue:
					return DXC.AggregationType.Min;

				case ChartAggregationType.MaximumValue:
					return DXC.AggregationType.Max;
			}
		}

		/// <summary>
		/// Converts DXC.DateTimeMeasureUnit enum value to our DatePartOrInterval 
		/// </summary>
		/// <param name="dp"></param>
		/// <returns></returns>
		internal static ChartAggregationType ToOurAggregationType(DXC.AggregationType at)
		{
			switch (at)
			{
				default:
				case DXC.AggregationType.Average:
					return ChartAggregationType.AverageValue;

				case DXC.AggregationType.Sum:
					return ChartAggregationType.SumValue;

				case DXC.AggregationType.Min:
					return ChartAggregationType.MinimalValue;

				case DXC.AggregationType.Max:
					return ChartAggregationType.MaximumValue;
			}
		}

		public static UriImageSource CreateImageUrl(Guid imageId, ImageType imageType)
		{
			var url = string.Format("{0}?id={1}&type={2}",
				ApiServices.PostImageApiUrlRaw,
				imageId.ToString(), (int)imageType);

			var retVal = new UriImageSource()
			{
				CachingEnabled = Defaults.IMAGE_CACHING,
				Uri = new Uri(url),
			};
			return retVal;
		}

	}
}


