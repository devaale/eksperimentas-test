using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IMessage
	{
		int Id { get; set; }
		DateTime Date { get; set; }
		string SenderUserId { get; set; }
		string ReceiverUserId { get; set; }
		DateTime? Read { get; set; }
		string Body { get; set; }
	}
}
