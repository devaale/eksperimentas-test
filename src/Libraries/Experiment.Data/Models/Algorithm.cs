using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using Newtonsoft.Json;

using Experiment.Core.Base;
using Experiment.Data.Enums;
using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
    public class Algorithm : ViewModelBase, IAlgorithm
    {
        AlgorithmType _Type;
        int _AlarmId;
		int _GroupId;
        int _DatapointId;

		public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public AlgorithmType Type
        {
            get => _Type; 
            set
            {
                SetProperty(ref _Type, value);

				OnPropertyChanged(nameof(IsVisibleGroupList));
				OnPropertyChanged(nameof(IsVisibleAlarmList));
				OnPropertyChanged(nameof(IsVisibleValueRange));
				OnPropertyChanged(nameof(IsVisibleDateTimePickerLabel));
				OnPropertyChanged(nameof(IsVisibleDatePicker));
				OnPropertyChanged(nameof(IsVisibleTimePicker));
				OnPropertyChanged(nameof(IsVisibleWeekDayPicker));
				OnPropertyChanged(nameof(IsVisibleSendValue));
				OnPropertyChanged(nameof(IsVisibleReminderAfterHours));
			}
        }

        public int ObjectId { get; set; }

        public int AlarmId
        {
            get => _AlarmId;
            set => SetProperty(ref _AlarmId, value);
        }
		
		public int GroupId
		{
			get => _GroupId;
			set => SetProperty(ref _GroupId, value);
		}

		public int DatapointId
		{
			get => _DatapointId;
			set => SetProperty(ref _DatapointId, value);
		}

		public decimal ValueFrom { get; set; }

        public decimal ValueTo { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

		public TimeSpan? TimeStart { get; set; }

		public TimeSpan? TimeEnd { get; set; }

		public bool OnMonday { get; set; }

        public bool OnTuesday { get; set; }

        public bool OnWednesday { get; set; }

        public bool OnThursday { get; set; }

        public bool OnFriday { get; set; }

        public bool OnSaturday { get; set; }

        public bool OnSunday { get; set; }

        public decimal ValueOff { get; set; }

        public decimal ValueOn { get; set; }

		public decimal Status { get; set; }

		public DateTime? Read { get; set; }

		public DateTime? EventTime { get; set; }

		public DateTime? Deleted { get; set; }

        public bool CanBeEdited { get; set; }

		public int ReminderAfterHours { get; set; }

		public DateTime? SnoozeNotificationTill { get; set; }

		public bool CanDelete { get; set; }

		#region Visual properties
		[JsonIgnore]		
		public bool IsVisibleGroupList { get => Type == AlgorithmType.TimeTrigger || Type == AlgorithmType.PeriodicTimeTrigger || Type == AlgorithmType.AlarmTrigger; }

		[JsonIgnore]
		public bool IsVisibleAlarmList { get => Type == AlgorithmType.AlarmTrigger; }

		[JsonIgnore]
		public bool IsVisibleValueRange { get => Type == AlgorithmType.Alarm; }

		[JsonIgnore]
		public bool IsVisibleDateTimePickerLabel { get => Type == AlgorithmType.TimeTrigger || Type == AlgorithmType.PeriodicTimeTrigger || Type == AlgorithmType.Alarm; }

		[JsonIgnore]
		public bool IsVisibleDatePicker { get => Type == AlgorithmType.TimeTrigger || Type == AlgorithmType.Alarm; }

		[JsonIgnore]
		public bool IsVisibleTimePicker { get => Type == AlgorithmType.TimeTrigger || Type == AlgorithmType.PeriodicTimeTrigger || Type == AlgorithmType.Alarm; }

		[JsonIgnore]
		public bool IsVisibleWeekDayPicker { get => Type == AlgorithmType.PeriodicTimeTrigger; }

		[JsonIgnore]
		public bool IsVisibleSendValue { get => Type == AlgorithmType.TimeTrigger || Type == AlgorithmType.PeriodicTimeTrigger || Type == AlgorithmType.AlarmTrigger; }

		[JsonIgnore]
		public bool IsVisibleReminderAfterHours { get => Type == AlgorithmType.Alarm; }

		#endregion

		#region Static
		public static T From<T> (DataRow algorithm)
			where T : Algorithm, new()
		{
			var retVal = new T()
			{
				Id = (int)algorithm[nameof(IAlgorithm.Id)],
				DatapointId = (int)algorithm[nameof(IAlgorithm.DatapointId)],
				ObjectId = (int)algorithm[nameof(IAlgorithm.ObjectId)],

				Name = null,    // Nullable
								//Name = (string)algorithm[nameof(IAlgorithm.Name)],

				Description = null, // Nullable
									//Description = (string)algorithm[nameof(IAlgorithm.Description)],

				DateStart = (DateTime)algorithm[nameof(IAlgorithm.DateStart)],
				DateEnd = (DateTime)algorithm[nameof(IAlgorithm.DateEnd)],
				TimeStart = (TimeSpan)algorithm[nameof(IAlgorithm.TimeStart)],
				TimeEnd = (TimeSpan)algorithm[nameof(IAlgorithm.TimeEnd)],
				ValueFrom = (decimal)algorithm[nameof(IAlgorithm.ValueFrom)],
				ValueTo = (decimal)algorithm[nameof(IAlgorithm.ValueTo)],

				ReminderAfterHours = (int)algorithm[nameof(IAlgorithm.ReminderAfterHours)],
				SnoozeNotificationTill = null,

				Status = (decimal)(algorithm[nameof(IAlgorithm.Status)]),

			};

			// Name
			if (!DBNull.Value.Equals(algorithm[nameof(IAlgorithm.Name)]))
			{
				retVal.Name = (string)algorithm[nameof(IAlgorithm.Name)];
			}

			// Description
			if (!DBNull.Value.Equals(algorithm[nameof(IAlgorithm.Description)]))
			{
				retVal.Description = (string)algorithm[nameof(IAlgorithm.Description)];
			}

			// SnoozeNotificationTill
			if (!DBNull.Value.Equals(algorithm[nameof(IAlgorithm.SnoozeNotificationTill)]))
			{
				retVal.SnoozeNotificationTill = (DateTime)algorithm[nameof(IAlgorithm.SnoozeNotificationTill)];
			}


			return retVal;
		}

		#endregion
	}
}