using System;
using System.Collections.Generic;
using System.Text;
using Experiment.Core.Base;
using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class ChatConversation : ViewModelBase
	{

		#region Attributes

		DateTime _Date;
		string _SenderUserId;
		string _Sender;
		string _ReceiverUserId;
		string _Receiver;
		string _Body;
		DateTime? _Read;
		bool _IsMyMessage;
		int _NumUnread;
		bool _HasUnread;

		#endregion

		#region Properties

		public virtual DateTime Date { get => _Date; set => SetProperty(ref _Date, value); }
		public virtual string SenderUserId { get => _SenderUserId; set => SetProperty(ref _SenderUserId, value); }
		public virtual string Sender { get => _Sender; set => SetProperty(ref _Sender, value); }
		public virtual string ReceiverUserId { get => _ReceiverUserId; set => SetProperty(ref _ReceiverUserId, value); }
		public virtual string Receiver { get => _Receiver; set => SetProperty(ref _Receiver, value); }
		public virtual string Body { get => _Body; set => SetProperty(ref _Body, value); }
		public virtual DateTime? Read { get => _Read; set => SetProperty(ref _Read, value); }
		public virtual bool IsMyMessage { get => _IsMyMessage; set => SetProperty(ref _IsMyMessage, value); }

		/// <summary>
		/// Number of unread messages
		/// </summary>
		public virtual int NumUnread { get => _NumUnread; set => SetProperty(ref _NumUnread, value); }

		/// <summary>
		/// Is this conversation has at least any unread messages
		/// </summary>
		public virtual bool HasUnread { get => _HasUnread; set => SetProperty(ref _HasUnread, value); }

		#endregion
	}
}
