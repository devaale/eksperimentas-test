using Experiment.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IAlgorithm
    {
		/// <summary>
		/// PK
		/// </summary>
		int Id { get; set; }
		/// <summary>
		/// 256
		/// </summary>
		string Name { get; set; }

        string Description { get; set; }

        AlgorithmType Type { get; set; }

        int ObjectId { get; set; }

        int AlarmId { get; set; }

        int GroupId { get; set; }

        int DatapointId { get; set; }

        decimal ValueFrom { get; set; }

        decimal ValueTo { get; set; }

		DateTime? DateStart { get; set; }

		DateTime? DateEnd { get; set; }

		TimeSpan? TimeStart { get; set; }

		TimeSpan? TimeEnd { get; set; }

        bool OnMonday { get; set; }

        bool OnTuesday { get; set; }

        bool OnWednesday { get; set; }

        bool OnThursday { get; set; }

        bool OnFriday { get; set; }

        bool OnSaturday { get; set; }

        bool OnSunday { get; set; }

        decimal ValueOff { get; set; }

        decimal ValueOn { get; set; }

		decimal Status { get; set; }

		DateTime? Read { get; set; }

		DateTime? EventTime { get; set; }

		int ReminderAfterHours { get; set; }

		DateTime? SnoozeNotificationTill { get; set; }

		/// <summary>
		/// NULL
		/// </summary>
		DateTime? Deleted { get; set; }
	}
}
