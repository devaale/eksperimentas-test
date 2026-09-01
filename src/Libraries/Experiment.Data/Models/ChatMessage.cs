using System;
using System.Collections.Generic;
using System.Text;
using Experiment.Core.Base;
using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class ChatMessage : ViewModelBase, IChatMessage
	{

		#region Attributes

		int _Id;
		DateTime _Date;
		string _Author;
		string _Body;
		DateTime? _Read;
		string _Action;
		bool _IsMyMessage;

		#endregion

		#region Properties

		public virtual int Id { get => _Id; set => SetProperty(ref _Id, value); }
		public virtual DateTime Date { get => _Date; set => SetProperty(ref _Date, value); }
		public virtual string Author { get => _Author; set => SetProperty(ref _Author, value); }
		public virtual string Body { get => _Body; set => SetProperty(ref _Body, value); }
		public virtual DateTime? Read { get => _Read; set => SetProperty(ref _Read, value); }
		public virtual string Action { get => _Action; set => SetProperty(ref _Action, value); }
		public virtual bool IsMyMessage { get => _IsMyMessage; set => SetProperty(ref _IsMyMessage, value); }

		#endregion
	}
}
