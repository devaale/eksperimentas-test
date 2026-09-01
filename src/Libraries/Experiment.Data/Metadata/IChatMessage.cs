using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IChatMessage
	{
		int Id { get; set; }
		DateTime Date { get; set; }
		string Author { get; set; }
		string Body { get; set; }
		DateTime? Read { get; set; }
		string Action { get; set; }
		bool IsMyMessage { get; set; }
	}
}
