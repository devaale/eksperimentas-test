using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

using M = Experiment.Data.Models;
using Experiment.Data.Metadata;
using Experiment.Data.Enums;
using Experiment.Data.Models;

namespace Website.Models
{
    /// <summary>
    /// Algorithm
    /// </summary>
    [Table("tblAlgorithm")]
	public class Algorithm : IAlgorithm
    {
		[Key]
		public int Id { get; set; }

        [StringLength(256)]
        public string Name { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        public AlgorithmType Type { get; set; }
        
        public int ObjectId { get; set; }

        public int AlarmId { get; set; }

        public int GroupId { get; set; }

        public int DatapointId { get; set; }

        public decimal ValueFrom { get; set; }

        public decimal ValueTo { get; set; }

		[DisplayFormat(NullDisplayText = "N/A")]
		public DateTime? DateStart { get; set; }

		[DisplayFormat(NullDisplayText = "N/A")]
		public DateTime? DateEnd { get; set; }

		[DisplayFormat(NullDisplayText = "N/A")]
		public TimeSpan? TimeStart { get; set; }

        [DisplayFormat(NullDisplayText = "N/A")]
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

		public int ReminderAfterHours { get; set; }
        
        public DateTime? SnoozeNotificationTill { get; set; }

		public DateTime? Deleted { get; set; }
    }
}