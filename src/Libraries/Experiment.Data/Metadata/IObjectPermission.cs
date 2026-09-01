using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IObjectPermission
	{
		int Id { get; set; }
		int ObjectId { get; set; }
		string FriendUserId { get; set; }
		bool PermWrite { get; set; }
		bool PermDevice { get; set; }
		bool PermAlgorithm { get; set; }
		bool PermGroup { get; set; }
		bool PermAlarm { get; set; }
	}
}
