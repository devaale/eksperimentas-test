#define DISABLE_NEVER_USED_PROPERTIES
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Core.Data{
	/// <summary>
	/// Universal class for data caching with specified or default caching delay in seconds.
	/// 
	/// Initializing, need to give as constructor's parameter callback which will get data for specific object.
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CacheProvider<T>
	{
		#region Const
		public const int CACHE_UPDATE_PERIOD_SECS = 10;

		#endregion

		#region Attributes
#if !DISABLE_NEVER_USED_PROPERTIES
		bool _DataInitialized;	// Currently disabled, to prevent VS warnings about assigned and unused property, but later it might be needed somewhere
#endif
		T _Data;
		DateTime _LastTimeUpdated;
		int _UpdatePeriodSec;
		Func<T> _CallBack;


#endregion

#region Properties
		public T Data
		{
			get
			{
				return GetData();
			}
		}

#endregion

#region Delegates
		

#endregion

#region Ctor

		protected CacheProvider()
		{
#if !DISABLE_NEVER_USED_PROPERTIES
			_DataInitialized = false;
#endif
			_UpdatePeriodSec = CACHE_UPDATE_PERIOD_SECS;
		}

		public CacheProvider(Func<T> callback, int updatePeriodSec)
			: this()
		{
			_CallBack = callback;
			_UpdatePeriodSec = updatePeriodSec;

			Update();
		}

		public CacheProvider(Func<T> callback)
			: this(callback, CACHE_UPDATE_PERIOD_SECS)
		{

		}

#endregion

#region Helpers

		void Update()
		{
			Debug.WriteLine("CacheProvider::Update");
			_Data = _CallBack();
			_LastTimeUpdated = DateTime.Now;
		}

		public T GetData ()
		{
			if(DateTime.Now > _LastTimeUpdated.AddSeconds(_UpdatePeriodSec))
			{
				Update();
			}
			return _Data;
		}

#endregion

}
}
