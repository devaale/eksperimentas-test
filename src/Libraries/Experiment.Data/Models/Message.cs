using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class Message: IMessage
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public string SenderUserId { get; set; }
		public string ReceiverUserId { get; set; }
		public DateTime? Read { get; set; }
		public string Body { get; set; }
	}
}
