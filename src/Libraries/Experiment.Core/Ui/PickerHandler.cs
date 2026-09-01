using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using Experiment.Core.Base;
using System.ComponentModel;

namespace Experiment.Core.Ui{
	/// <summary>
	/// Simplifies work with picker UI control
	/// 
	/// @TODO: On change bindings to update values if property were changed
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PickerHandler<T> : ViewModelBase
		//where T: class, new()
		where T : class
	{
		#region Const
		readonly static string TYPE_NAME = nameof(PickerHandler<T>);

		#endregion

		#region Attributes
		object _Instance;
		PropertyInfo _InstancePropertyInfo;
		PropertyInfo _MemberValuePropertyInfo;
		ObservableCollection<T> _List;

		INotifyPropertyChanged _NotifyInstance;

		#endregion

		#region Properties
		public ObservableCollection<T> List
		{
			get
			{
				if(_List == null)
				{
					_List = new ObservableCollection<T>();
				}
				//Debug.WriteLine(TYPE_NAME + "::List[GET], Count=" + _List.Count);
				return _List;
			}
			protected set
			{
				//Debug.WriteLine(TYPE_NAME + "::List[SET]=" + value);
				SetProperty(ref _List, value);
			}

		}

		public T Selected
		{
			get
			{
				var currentValue = _InstancePropertyInfo.GetValue(_Instance);
				T retVal = null;

				foreach(T item in List)
				{
					var keyValue = _MemberValuePropertyInfo.GetValue(item);

					if (currentValue != null)
					{
						if (currentValue.Equals(keyValue))
						{
							retVal = item;
							break;
						}
					}
					else if (keyValue != null)
					{
						if (keyValue.Equals(currentValue))
						{
							retVal = item;
							break;
						}
					}
					else if (currentValue == keyValue)
					{
						retVal = item;
						break;
					}
				}

				//Debug.WriteLine(TYPE_NAME + "::Selected[GET], Return=" + retVal);
				return retVal;
			}
			set
			{
				Debug.WriteLine(string.Format("{0}::{1}[SET]={2}", TYPE_NAME, nameof(Selected), value));
				if(List.Any(i => Object.Equals(i, value)))
				{
					var newValue = _MemberValuePropertyInfo.GetValue(value);
					_InstancePropertyInfo.SetValue(_Instance, newValue);
				}
				/*
				var found = false;
				foreach(var item in List)
				{
					found = ((object)value == (object)item);
					if (found)
						break;
				}

				if(found)
				{
					var newValue = _MemberValuePropertyInfo.GetValue(value);
					_InstancePropertyInfo.SetValue(_Instance, newValue);
				}
				*/
			}
		}

		#endregion

		#region CTOR

		protected PickerHandler()
		{
		}

		public PickerHandler(
			object instance, 
			string propertyName, 
			string memberValue)

			: this()
		{
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));

			if (String.IsNullOrEmpty(propertyName))
				throw new ArgumentException(nameof(propertyName));

			if (String.IsNullOrEmpty(memberValue))
				throw new ArgumentException(nameof(memberValue));

			_Instance = instance;
			_InstancePropertyInfo = _Instance.GetType().GetProperty(propertyName);
			_MemberValuePropertyInfo = typeof(T).GetProperty(memberValue);

			if (_InstancePropertyInfo == null)
				throw new ArgumentException(String.Format("[{0}] or [propertyName={1}] is Invalid", nameof(instance), propertyName));

			if (_MemberValuePropertyInfo == null)
				throw new ArgumentException(String.Format("Generic<T> type has no [{0}={1}] property", nameof(memberValue), memberValue));

			// INotifyPropertyChanged
			if (_Instance is INotifyPropertyChanged)
			{
				_NotifyInstance = _Instance as INotifyPropertyChanged;
				_NotifyInstance.PropertyChanged += NotifyInstance_PropertyChanged;
			}
		}

		public PickerHandler(
			object instance, 
			string propertyName, 
			string memberValue, 
			IEnumerable<T> list)

			: this(instance, propertyName, memberValue)
		{
			AddRange(list);
		}

		/// <summary>
		/// Destructor (ever rare today and common decades ago.
		/// 
		/// Might throw an exception in case of deconstruction some day, then put everything in try catch and ignore the exception.
		/// </summary>
		~PickerHandler()
		{
			if(_NotifyInstance != null)
			{
				_NotifyInstance.PropertyChanged -= NotifyInstance_PropertyChanged;
			}
		}

		#endregion

		#region Helpers
		private void NotifyInstance_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName.Equals(_InstancePropertyInfo.Name))
			{
				OnPropertyChanged(nameof(Selected));
			}
		}

		#endregion

		#region Methods

		public void Clear()
		{
			List.Clear();
		}

		public void Add(T item)
		{
			List.Add(item);
		}

		public void AddRange(IEnumerable<T> items)
		{
			foreach(var item in items)
			{
				List.Add(item);
			}
			OnPropertyChanged(nameof(Selected));
		}

		#endregion
	}
}
