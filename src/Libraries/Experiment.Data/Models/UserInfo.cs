using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	/// <summary>
	/// User information, includingd aggregated or aditional information about the user
	/// </summary>
	public class UserInfo : User, IUserInfo
	{
		public DashboardSetting DashboardSetting { get; set; }

		/// <summary>
		/// Licenses
		/// </summary>
		public IEnumerable<License> Licenses { get; }

		/// <summary>
		/// If user is friend, PK of tblFriend, which is used in specific controller
		/// 
		/// (Initialized not in all cases)
		/// </summary>
		public int? FriendId { get; set; }

		/// <summary>
		/// If user is blocked, PK of tblBlocked, which is used in specific controller
		/// 
		/// (Initialized not in all cases)
		/// </summary>
		public int? BlockedId { get; set; }

		/// <summary>
		/// (Initialized not in all cases)
		/// </summary>
		public bool IsMe { get; set; }
		/// <summary>
		/// (Initialized not in all cases)
		/// </summary>
		public bool IsFriend { get; set; }
		/// <summary>
		/// (Initialized not in all cases)
		/// </summary>
		public bool IsBlocked { get; set; }

		public UserInfo()
			: base()
		{
			Licenses = new ObservableCollection<License>();
		}
	}
}
