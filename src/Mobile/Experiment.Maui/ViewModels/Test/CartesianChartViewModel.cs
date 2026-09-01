//#define TEST_ORIGINAL
using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Core.Base;
using Experiment.Data.Models;
using Experiment.Maui.Views.Devices;

namespace Experiment.Maui.ViewModels.Test{
	/// <summary>
	/// @see https://docs.devexpress.com/MobileControls/400411/xamarin-forms/charts/getting-started/cartesian-chart-lesson
	/// </summary>
	public class CartesianChartViewModel : ViewModelBase
	{
#if TEST_ORIGINAL
		public class CountryGdp
		{
			public string CountryName { get; }
			public IList<GdpValue> Values { get; }

			public CountryGdp(string country, params GdpValue[] values)
			{
				this.CountryName = country;
				this.Values = new List<GdpValue>(values);
			}
		}

		public class GdpValue
		{
			public DateTime Year { get; }
			public double Value { get; }

			public GdpValue(DateTime year, double value)
			{
				this.Year = year;
				this.Value = value;
			}
		}
		public CountryGdp GdpValueForUSA { get; }
		public CountryGdp GdpValueForChina { get; }
		public CountryGdp GdpValueForJapan { get; }
#else
		public Datapoint GdpValueForUSA { get; }
		public Datapoint GdpValueForChina { get; }
		public Datapoint GdpValueForJapan { get; }
#endif

		public CartesianChartViewModel()
		{
			Title = nameof(CartesianChartViewModel);

#if TEST_ORIGINAL
			GdpValueForUSA = new CountryGdp(
				"USA",
				new GdpValue(new DateTime(2017, 1, 1), 19.391),
				new GdpValue(new DateTime(2016, 1, 1), 18.624),
				new GdpValue(new DateTime(2015, 1, 1), 18.121),
				new GdpValue(new DateTime(2014, 1, 1), 17.428),
				new GdpValue(new DateTime(2013, 1, 1), 16.692),
				new GdpValue(new DateTime(2012, 1, 1), 16.155),
				new GdpValue(new DateTime(2011, 1, 1), 15.518),
				new GdpValue(new DateTime(2010, 1, 1), 14.964),
				new GdpValue(new DateTime(2009, 1, 1), 14.419),
				new GdpValue(new DateTime(2008, 1, 1), 14.719),
				new GdpValue(new DateTime(2007, 1, 1), 14.478)
			);
			GdpValueForChina = new CountryGdp(
				"China",
				new GdpValue(new DateTime(2017, 1, 1), 12.238),
				new GdpValue(new DateTime(2016, 1, 1), 11.191),
				new GdpValue(new DateTime(2015, 1, 1), 11.065),
				new GdpValue(new DateTime(2014, 1, 1), 10.482),
				new GdpValue(new DateTime(2013, 1, 1), 9.607),
				new GdpValue(new DateTime(2012, 1, 1), 8.561),
				new GdpValue(new DateTime(2011, 1, 1), 7.573),
				new GdpValue(new DateTime(2010, 1, 1), 6.101),
				new GdpValue(new DateTime(2009, 1, 1), 5.110),
				new GdpValue(new DateTime(2008, 1, 1), 4.598),
				new GdpValue(new DateTime(2007, 1, 1), 3.552)
			);
			GdpValueForJapan = new CountryGdp(
				"Japan",
				new GdpValue(new DateTime(2017, 1, 1), 4.872),
				new GdpValue(new DateTime(2016, 1, 1), 4.949),
				new GdpValue(new DateTime(2015, 1, 1), 4.395),
				new GdpValue(new DateTime(2014, 1, 1), 4.850),
				new GdpValue(new DateTime(2013, 1, 1), 5.156),
				new GdpValue(new DateTime(2012, 1, 1), 6.203),
				new GdpValue(new DateTime(2011, 1, 1), 6.156),
				new GdpValue(new DateTime(2010, 1, 1), 5.700),
				new GdpValue(new DateTime(2009, 1, 1), 5.231),
				new GdpValue(new DateTime(2008, 1, 1), 5.038),
				new GdpValue(new DateTime(2007, 1, 1), 4.515)
			);
#else
			GdpValueForUSA = new Datapoint()
			{
				Name = nameof(GdpValueForUSA),
				Values = new List<DatapointValue>()
				{
					new DatapointValue() { Date = DateTime.Parse("2023-01-01 00:00"), Value = 0 },
					new DatapointValue() { Date = DateTime.Parse("2023-01-02 00:00"), Value = 1 },
					new DatapointValue() { Date = DateTime.Parse("2023-01-03 00:00"), Value = -10 },
					new DatapointValue() { Date = DateTime.Parse("2023-01-04 00:00"), Value = 50 },
				},
			};

			GdpValueForChina = new Datapoint()
			{
				Name = nameof(GdpValueForChina),
				Values = new List<DatapointValue>()
				{
					new DatapointValue() { Date = DateTime.Parse("2023-01-01 00:00"), Value = 50 },
					new DatapointValue() { Date = DateTime.Parse("2023-01-02 00:00"), Value = -20 },
					new DatapointValue() { Date = DateTime.Parse("2023-01-03 00:00"), Value = 13 },
					new DatapointValue() { Date = DateTime.Parse("2023-01-04 00:00"), Value = 2 },
				},
			};

			GdpValueForJapan = new Datapoint()
			{
				Name = nameof(GdpValueForJapan),
				Values = new List<DatapointValue>()
				{
					new DatapointValue() { Date = DateTime.Parse("2023-01-01 00:00"), Value = 5 },
					new DatapointValue() { Date = DateTime.Parse("2023-01-02 00:00"), Value = 120 },
					new DatapointValue() { Date = DateTime.Parse("2023-01-03 00:00"), Value = -30 },
					new DatapointValue() { Date = DateTime.Parse("2023-01-04 00:00"), Value = -10 },
				},
			};
#endif
		}
	}
}
