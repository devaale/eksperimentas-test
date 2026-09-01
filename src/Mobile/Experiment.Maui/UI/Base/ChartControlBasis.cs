using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Text;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using DevExpress.Maui.Charts;

using Experiment.Core.Base;
using Experiment.Core.Enums;

using Experiment.Data.Enums;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Metadata;
using Experiment.Maui.Services;

namespace Experiment.Maui.UI.Base{
	abstract public class ChartControlBasis<T> : Grid, IChartControl<T>
	{
		#region Const

		#endregion

		#region Attributes

		#endregion

		#region Properties

		/// <summary>
		/// Bindable ChartTitleProperty
		/// </summary>
		public static readonly BindableProperty ChartTitleProperty =
			BindableProperty.Create(nameof(ChartTitle), typeof(string), typeof(ChartControlBasis<T>), string.Empty, propertyChanged: OnChartTitleChanged);
		/// <summary>
		/// ChartTitle
		/// </summary>
		public virtual string ChartTitle
		{
			get => (string)GetValue(ChartTitleProperty);
			set => SetValue(ChartTitleProperty, value);
		}
		protected static void OnChartTitleChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var me = (ChartControlBasis<T>)bindable;
			me.OnChartTitleChange((string)oldValue, (string)newValue);
		}
		protected virtual void OnChartTitleChange(string oldValue, string newValue)
		{

		}

		/// <summary>
		/// Bindable ChartTypeProperty
		/// </summary>
		public static readonly BindableProperty ChartTypeProperty =
			BindableProperty.Create(nameof(ChartType), typeof(ChartType), typeof(ChartControlBasis<T>), ChartType.None, propertyChanged: OnChartTypeChanged);
		/// <summary>
		/// ChartType
		/// </summary>
		public ChartType ChartType
		{
			get => (ChartType)GetValue(ChartTypeProperty);
			set => SetValue(ChartTypeProperty, value);
		}
		protected static void OnChartTypeChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var me = (ChartControlBasis<T>)bindable;
			me.OnChartTypeChange((ChartType)oldValue, (ChartType)newValue);
		}
		protected virtual void OnChartTypeChange(ChartType oldValue, ChartType newValue)
		{

		}

		/// <summary>
		/// Bindable ChartSeriesProperty
		/// </summary>
		public static readonly BindableProperty ChartSeriesProperty =
			BindableProperty.Create(nameof(ChartSeries), typeof(T), typeof(ChartControlBasis<T>), null, propertyChanged: OnChartSeriesChanged);
		/// <summary>
		/// ChartSeries
		/// </summary>
		public T ChartSeries
		{
			get => (T)GetValue(ChartSeriesProperty);
			set => SetValue(ChartSeriesProperty, value);
		}
		protected static void OnChartSeriesChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var me = (ChartControlBasis<T>)bindable;
			me.OnChartSeriesChange((T)oldValue, (T)newValue);
		}
		protected virtual void OnChartSeriesChange(T oldValue, T newValue)
		{

		}

		/// <summary>
		/// Bindable ChartSeriesDisplayMemberProperty
		/// </summary>
		public static readonly BindableProperty ChartSeriesNameProperty =
			BindableProperty.Create(nameof(ChartSeriesName), typeof(string), typeof(ChartControlBasis<T>), string.Empty, propertyChanged: OnChartSeriesNameChanged);
		/// <summary>
		/// ChartTitle
		/// </summary>
		public virtual string ChartSeriesName
		{
			get => (string)GetValue(ChartSeriesNameProperty);
			set => SetValue(ChartSeriesNameProperty, value);
		}
		protected static void OnChartSeriesNameChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var me = (ChartControlBasis<T>)bindable;
			me.OnChartSeriesNameChange((string)oldValue, (string)newValue);
		}
		protected virtual void OnChartSeriesNameChange(string oldValue, string newValue)
		{
		}

		/// <summary>
		/// Bindable ChartSeriesValueMemberProperty
		/// </summary>
		public static readonly BindableProperty ChartSeriesDisplayMemberProperty =
			BindableProperty.Create(nameof(ChartSeriesDisplayMember), typeof(string), typeof(ChartControlBasis<T>), string.Empty, propertyChanged: OnChartSeriesDisplayMemberChanged);
		/// <summary>
		/// ChartTitle
		/// </summary>
		public virtual string ChartSeriesDisplayMember
		{
			get => (string)GetValue(ChartSeriesDisplayMemberProperty);
			set => SetValue(ChartSeriesDisplayMemberProperty, value);
		}
		protected static void OnChartSeriesDisplayMemberChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var me = (ChartControlBasis<T>)bindable;
			me.OnChartSeriesDisplayMemberChange((string)oldValue, (string)newValue);
		}
		protected virtual void OnChartSeriesDisplayMemberChange(string oldValue, string newValue)
		{
		}

		/// <summary>
		/// Bindable ChartSeriesValueMemberProperty
		/// </summary>
		public static readonly BindableProperty ChartSeriesValueMemberProperty =
			BindableProperty.Create(nameof(ChartSeriesValueMember), typeof(string), typeof(ChartControlBasis<T>), "Value", propertyChanged: OnChartSeriesValueMemberChanged);
		/// <summary>
		/// ChartTitle
		/// </summary>
		public virtual string ChartSeriesValueMember
		{
			get => (string)GetValue(ChartSeriesValueMemberProperty);
			set => SetValue(ChartSeriesValueMemberProperty, value);
		}
		protected static void OnChartSeriesValueMemberChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var me = (ChartControlBasis<T>)bindable;
			me.OnChartSeriesValueMemberChange((string)oldValue, (string)newValue);
		}
		protected virtual void OnChartSeriesValueMemberChange(string oldValue, string newValue)
		{
		}

		/// <summary>
		/// Bindable ChartMeasureUnitProperty
		/// </summary>
		public static readonly BindableProperty ChartMeasureUnitProperty =
			BindableProperty.Create(nameof(ChartMeasureUnit), typeof(DatePartOrInterval), typeof(ChartControlBasis<T>), DatePartOrInterval.Minute, propertyChanged: OnChartMeasureUnitChanged);
		/// <summary>
		/// ChartMeasureUnit
		/// </summary>
		public virtual DatePartOrInterval ChartMeasureUnit
		{
			get => (DatePartOrInterval)GetValue(ChartMeasureUnitProperty);
			set => SetValue(ChartMeasureUnitProperty, value);
		}
		protected static void OnChartMeasureUnitChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var me = (ChartControlBasis<T>)bindable;
			me.OnChartMeasureUnitChange((DatePartOrInterval)oldValue, (DatePartOrInterval)newValue);
		}
		protected virtual void OnChartMeasureUnitChange(DatePartOrInterval oldValue, DatePartOrInterval newValue)
		{
		}

		/// <summary>
		/// Bindable ChartAggregationType
		/// </summary>
		public static readonly BindableProperty ChartAggregationTypeProperty =
			BindableProperty.Create(nameof(ChartAggregationType), typeof(ChartAggregationType), typeof(ChartControlBasis<T>), ChartAggregationType.RealValue, propertyChanged: OnChartAggregationTypeChanged);
		/// <summary>
		/// ChartAggregationType
		/// </summary>
		public virtual ChartAggregationType ChartAggregationType
		{
			get => (ChartAggregationType)GetValue(ChartAggregationTypeProperty);
			set => SetValue(ChartAggregationTypeProperty, value);
		}
		protected static void OnChartAggregationTypeChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var me = (ChartControlBasis<T>)bindable;
			me.OnChartAggregationTypeChange((ChartAggregationType)oldValue, (ChartAggregationType)newValue);
		}
		protected virtual void OnChartAggregationTypeChange(ChartAggregationType oldValue, ChartAggregationType newValue)
		{
		}


		#endregion

		#region Events

		#endregion

		#region Helpers

		#endregion

		#region Methods

		#endregion

	}
}


