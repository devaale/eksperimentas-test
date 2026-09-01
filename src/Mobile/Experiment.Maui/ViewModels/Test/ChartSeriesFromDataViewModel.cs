//#define TEST_ORIGINAL
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using Experiment.Core.Base;
using Experiment.Data.Models;

namespace Experiment.Maui.ViewModels.Test{
	/// <summary>
	/// @see https://docs.devexpress.com/MobileControls/402973/xamarin-forms/charts/examples/generate-series-from-a-data-store
	/// </summary>
	internal class ChartSeriesFromDataViewModel : ViewModelBase
	{
#if TEST_ORIGINAL
		public class CountryYearlyStatisticsData
		{
			public List<CountryGdpStatistics> SeriesData { get; } = new List<CountryGdpStatistics> {
				new CountryGdpStatistics("UK", 45903.9, 2017),
				new CountryGdpStatistics("UK", 48651.6, 2018),
				new CountryGdpStatistics("UK", 49912.4, 2019),
				new CountryGdpStatistics("USA", 60091.6, 2017),
				new CountryGdpStatistics("USA", 63043.0, 2018),
				new CountryGdpStatistics("USA", 65240.4, 2019),
				new CountryGdpStatistics("Canada", 48479.8, 2017),
				new CountryGdpStatistics("Canada", 51106.9, 2018),
				new CountryGdpStatistics("Canada", 51822.2, 2019),
				new CountryGdpStatistics("Japan", 41119.4, 2017),
				new CountryGdpStatistics("Japan", 42840.2, 2018),
				new CountryGdpStatistics("Japan", 43642.7, 2019),
				new CountryGdpStatistics("France", 44622.8, 2017),
				new CountryGdpStatistics("France", 47922.2, 2018),
				new CountryGdpStatistics("France", 50693.5, 2019),
				new CountryGdpStatistics("Germany", 53122.0, 2017),
				new CountryGdpStatistics("Germany", 56689.0, 2018),
				new CountryGdpStatistics("Germany", 57557.9, 2019),
				new CountryGdpStatistics("Italy", 41713.9, 2017),
				new CountryGdpStatistics("Italy", 44444.7, 2018),
				new CountryGdpStatistics("Italy", 45691.0, 2019),
			};
		}

		public class CountryGdpStatistics
		{
			public string Country { get; private set; }
			public double Gdp { get; private set; }
			public int Year { get; private set; }

			public CountryGdpStatistics(string country, double gdp, int year)
			{
				Country = country;
				Gdp = gdp;
				Year = year;
			}
		}

		CountryYearlyStatisticsData data = new CountryYearlyStatisticsData();
		public List<CountryGdpStatistics> SeriesData => this.data.SeriesData;
#else
		public List<Datapoint> Datapoints { get; } = new List<Datapoint>()
		{
			new Datapoint() { Name = "A" },
			new Datapoint() { Name = "B" },
			new Datapoint() { Name = "C" },
		};

		public List<DatapointValue> SeriesData { get; set; }

		public ChartSeriesFromDataViewModel()
		{
			Title = nameof(ChartSeriesFromDataViewModel);

			SeriesData = new List<DatapointValue>()
			{
				// A
				new DatapointValue() { Datapoint = Datapoints[0], Date = DateTime.Parse("2023-01-01 00:00"), Value = 0 },
				new DatapointValue() { Datapoint = Datapoints[0], Date = DateTime.Parse("2023-01-02 00:00"), Value = 1 },
				new DatapointValue() { Datapoint = Datapoints[0], Date = DateTime.Parse("2023-01-03 00:00"), Value = -10 },
				new DatapointValue() { Datapoint = Datapoints[0], Date = DateTime.Parse("2023-01-04 00:00"), Value = 50 },

				// B
				new DatapointValue() { Datapoint = Datapoints[1], Date = DateTime.Parse("2023-01-01 00:00"), Value = 50},
				new DatapointValue() { Datapoint = Datapoints[1], Date = DateTime.Parse("2023-01-02 00:00"), Value = -20 },
				new DatapointValue() { Datapoint = Datapoints[1], Date = DateTime.Parse("2023-01-03 00:00"), Value = 13 },
				new DatapointValue() { Datapoint = Datapoints[1], Date = DateTime.Parse("2023-01-04 00:00"), Value = 2 },

				// C
				new DatapointValue() { Datapoint = Datapoints[2], Date = DateTime.Parse("2023-01-01 00:00"), Value = 5 },
				new DatapointValue() { Datapoint = Datapoints[2], Date = DateTime.Parse("2023-01-02 00:00"), Value = 120 },
				new DatapointValue() { Datapoint = Datapoints[2], Date = DateTime.Parse("2023-01-03 00:00"), Value = -30 },
				new DatapointValue() { Datapoint = Datapoints[2], Date = DateTime.Parse("2023-01-04 00:00"), Value = -10 },
			};
		}
#endif
	}
}
