using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Experiment.Core;
using Experiment.Core.Base;
using Experiment.Data.Metadata;

// MVVM
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Enums;
using Experiment.Maui.Models;
using Experiment.Maui.Services;
using Experiment.Maui.Views;
using System.Diagnostics;

namespace Experiment.Maui.Data{
    public class Dictionaries : ViewModelBase
    {
		#region Const
		const string TYPE_NAME = nameof(Dictionaries);

		#endregion

		#region Attributes
		readonly ApiServices _ApiServices = new ApiServices();

		VisualUser _CurrentUser;
		M.Object _CurrentObject;

		ObservableCollection<M.Object> _Objects;
		ObservableCollection<IRelatedPerson> _Friends;
		ObservableCollection<IRelatedPerson> _Blocked;
		ObservableCollection<M.Language> _Languages;

		#endregion

		#region Properties
		public VisualUser CurrentUser
		{
			get => _CurrentUser;
			set
			{
				SetProperty(ref _CurrentUser, value);

				// If user is loaded successful
				if (_CurrentUser != null)
				{
					// And user's language is different than saved in Settings
					if (!Settings.Language.Equals(_CurrentUser.Language))
					{
						// Updating user's language user's language from DB
						Settings.Language = _CurrentUser.Language;
					}
				}
			}
		}

		public M.Object CurrentObject
		{
			get => _CurrentObject;
			set => SetProperty(ref _CurrentObject, value);
		}
		public ObservableCollection<M.Object> Objects
        {
			get => _Objects;
            set => SetProperty(ref _Objects, value);
        }

		public ObservableCollection<IRelatedPerson> Friends
		{
			get => _Friends;
			set => SetProperty(ref _Friends, value);
		}
		public ObservableCollection<IRelatedPerson> Blocked
		{
			get => _Blocked;
			set => SetProperty(ref _Blocked, value);
		}

        public ObservableCollection<M.Language> Languages
		{
			get => _Languages;
			set => SetProperty(ref _Languages, value);
		}

		#region Singleton
		protected static Dictionaries _sInstance;
		public static Dictionaries Instance
		{
			get
			{
				if(_sInstance == null)
				{
					_sInstance = new Dictionaries();
				}
				return _sInstance;
			}
		}
		#endregion

		#endregion

		#region Methods

		public async Task<VisualUser> GetCurrentUser(bool reload)
		{
			var vLoc = string.Format("{0}::{1}(bool reload={2})", TYPE_NAME, nameof(GetCurrentUser), reload);
			Debug.WriteLine(vLoc);

			if (CurrentUser == null || reload)
			{
				CurrentUser = await _ApiServices.UserInfoAsync(UserInfoType.User, string.Empty);
			}

			//Debug.WriteLine(string.Format("{0}, Interval={1}", vLoc, CurrentUser.DashboardSetting.IntervalDatepart));

			return CurrentUser;
		}

		public async Task<M.Object> GetCurrentObject(bool reload)
		{
			var vLoc = string.Format("{0}::{1}(bool reload={2})", TYPE_NAME, nameof(GetCurrentObject), reload);
			Debug.WriteLine(vLoc);

			if (CurrentObject == null || reload)
			{
				var objects = await GetObjects(false);
				CurrentObject = objects.FirstOrDefault(o => o.Id.Equals(Settings.ObjectId));
			}
			return CurrentObject;
		}

		/// <summary>
		/// Returns currently loaded objects
		/// </summary>
		/// <param name="reload"></param>
		/// <returns></returns>
		public async Task<ObservableCollection<M.Object>> GetObjects(bool reload)
		{
			var needsCreation = Objects == null;
			var needsReload = reload || needsCreation;

			if(needsCreation)
			{
				Objects = new ObservableCollection<M.Object>();
			}

			if(needsReload)
			{
				Objects.Clear();

				var items = await _ApiServices.ObjectListAsync();
				foreach (var i in items)
				{
					Objects.Add(i);
				}
			}

			return Objects;
		}

		/// <summary>
		/// Returns currently loaded objects
		/// </summary>
		/// <param name="reload"></param>
		/// <returns></returns>
		public async Task<ObservableCollection<IRelatedPerson>> GetFriends(bool reload)
		{
			var needsCreation = Friends == null;
			var needsReload = reload || needsCreation;

			if (needsCreation)
			{
				Friends = new ObservableCollection<IRelatedPerson>();
			}

			if (needsReload)
			{
				Friends.Clear();

				var items = await _ApiServices.FriendListAsync();
				foreach (var i in items)
				{
					Friends.Add(i);
				}
			}

			return Friends;
		}

		/// <summary>
		/// Returns currently loaded objects
		/// </summary>
		/// <param name="reload"></param>
		/// <returns></returns>
		public async Task<ObservableCollection<IRelatedPerson>> GetBlocked(bool reload)
		{
			var needsCreation = Blocked == null;
			var needsReload = reload || needsCreation;

			if (needsCreation)
			{
				Blocked = new ObservableCollection<IRelatedPerson>();
			}

			if (needsReload)
			{
				Blocked.Clear();

				var items = await _ApiServices.BlockedListAsync();
				foreach (var i in items)
				{
					Blocked.Add(i);
				}
			}

			return Blocked;
		}

		public async Task<ObservableCollection<M.Language>> GetLanguages(bool reload)
		{
			var needsCreation = Languages == null;
			var needsReload = reload || needsCreation;

			if (needsCreation)
			{
				Languages = new ObservableCollection<M.Language>();
			}

			if (needsReload)
			{
				Languages.Clear();

				var items = await _ApiServices.LanguageListAsync();
				foreach (var i in items)
				{
					Languages.Add(i);
				}
			}

			return Languages;
		}

		/// <summary>
		/// Called when user logged of to purge required cached colletions
		/// </summary>
		public void Logout()
		{
			// All data related to specific user should be purged
			CurrentUser = null;
			CurrentObject = null;
			Friends = null;
			Objects = null;
		}

		#endregion
	}
}

