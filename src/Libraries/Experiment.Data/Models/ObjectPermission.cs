using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Core.Base;
using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class ObjectPermission : ViewModelBase, IObjectPermission
	{
		int _Id;
		int _ObjectId;
		string _FriendUserId;
		bool _PermWrite;
		bool _PermDevice;
		bool _PermAlgorithm;
		bool _PermGroup;
		bool _PermAlarm;

		public virtual int Id { get => _Id; set => SetProperty(ref _Id, value); }
		public virtual int ObjectId { get => _ObjectId; set => SetProperty(ref _ObjectId, value); }
		public virtual string FriendUserId { get => _FriendUserId; set => SetProperty(ref _FriendUserId, value); }
		public virtual bool PermWrite { get => _PermWrite; set => SetProperty(ref _PermWrite, value); }
		public virtual bool PermDevice { get => _PermDevice; set => SetProperty(ref _PermDevice, value); }
		public virtual bool PermAlgorithm { get => _PermAlgorithm; set => SetProperty(ref _PermAlgorithm, value); }
		public virtual bool PermGroup { get => _PermGroup; set => SetProperty(ref _PermGroup, value); }
		public virtual bool PermAlarm { get => _PermAlarm; set => SetProperty(ref _PermAlarm, value); }
	}
}
