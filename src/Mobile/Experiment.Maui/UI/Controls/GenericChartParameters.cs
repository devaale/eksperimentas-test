using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Core.Base;
using Experiment.Core.Enums;
using Experiment.Data.Enums;
using Experiment.Data.Models;

using Experiment.Maui.Models;
using Experiment.Maui.ViewModels.Devices;

namespace Experiment.Maui.UI.Controls{
	public class GenericChartParameters : ViewModelBase
	{
		#region Attributes
		protected GenericChartControl _GenericChartControl;
		protected ChartType _GraphType;
		protected IEnumerable<Datapoint> _DataSource;
		protected DatePartOrInterval _Interval;
		protected ChartAggregationType _AggregationType;
		protected string _SeriesDataMember;
		protected string _ArgumentDataMember;
		protected string _ValueMember;
		protected bool _LegendVisible = true;

		#endregion

		#region Properties
		public bool HasChart { get => _GenericChartControl != null; }

		/// <summary>
		/// Chart title
		/// </summary>
		public override string Title
		{
			get => base.Title;
			set
			{
				var changed = base.Title != value;
				base.Title = value;

				if (HasChart && changed)
				{
					_GenericChartControl.UpdateTitle();
				}
			}
		}

		/// <summary>
		/// Chart Type (It is clear)
		/// </summary>
		public virtual ChartType GraphType
		{
			get => _GraphType;
			set
			{
				var changed = _GraphType != value;
				SetProperty(ref _GraphType, value);

				if(HasChart && changed)
				{
					_GenericChartControl.UpdateChartType();
				}
			}
		}

		/// <summary>
		/// Series data or DataSource
		/// </summary>
		public virtual IEnumerable<Datapoint> DataSource
		{
			get => _DataSource;
			set
			{
				var changed = _DataSource != value;
				SetProperty(ref _DataSource, value);

				if (HasChart && changed)
				{
					_GenericChartControl.UpdateSeries();
				}
			}
		}

		public virtual ChartAggregationType AggregationType
		{
			get => _AggregationType;
			set
			{
				var changed = _AggregationType != value;
				SetProperty(ref _AggregationType, value);

				if (HasChart && changed)
				{
					_GenericChartControl.UpdateAxisX();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual DatePartOrInterval Interval
		{
			get => _Interval;
			set
			{
				var changed = _Interval != value;
				SetProperty(ref _Interval, value);

				if(HasChart && changed)
				{
					_GenericChartControl.UpdateAxisX();
				}
			}
		}

		/// <summary>
		/// Series name (I think)
		/// 
		/// This should never happen as we do not change during runtime these variables.
		/// </summary>
		public virtual string SeriesDataMember
		{
			get => _SeriesDataMember;
			set
			{
				var changed = _SeriesDataMember != value;
				SetProperty(ref _SeriesDataMember, value);

				if (HasChart && changed)
				{
					_GenericChartControl.UpdateSeries();
				}
			}
		}

		/// <summary>
		/// Scale member name (eg. DatapointValue.Date)
		/// 
		/// This should never happen as we do not change during runtime these variables.
		/// </summary>
		public virtual string ArgumentDataMember
		{
			get => _ArgumentDataMember;
			set
			{
				var changed = _ArgumentDataMember != value;
				SetProperty(ref _ArgumentDataMember, value);

				if(HasChart && changed)
				{
					_GenericChartControl.UpdateArgumentDataMember();
				}
			}
		}

		/// <summary>
		/// Value member name (eg. DatapointValue.Value)
		/// 
		/// This should never happen as we do not change during runtime these variables.
		/// </summary>
		public virtual string ValueMember
		{
			get => _ValueMember;
			set
			{
				var changed = _ValueMember != value;
				SetProperty(ref _ValueMember, value);

				if(HasChart && changed)
				{
					_GenericChartControl.UpdateValueMember();
				}
			}
		}

		public virtual bool LegendVisible
		{
			get => _LegendVisible;
			set
			{
				var changed = _LegendVisible != value;
				SetProperty(ref _LegendVisible, value);

				if (HasChart && changed)
				{
					_GenericChartControl.UpdateLegendVisibility();
				}
			}
		}

		#endregion

		#region Ctor

		#endregion

		#region Helpers

		#endregion

		#region Methods

		/// <summary>
		/// Sets GenericChartControl for further processing
		/// </summary>
		/// <param name="genericChartControl"></param>
		/// <returns></returns>
		public bool Init(GenericChartControl genericChartControl)
		{
			_GenericChartControl = genericChartControl;
			if(HasChart)
			{
				if(GraphType != ChartType.None)
				{
					_GenericChartControl.UpdateChartType();
				}
				return _GenericChartControl.IsChart;
			}
			return false;
		}

		#endregion
	}
}
