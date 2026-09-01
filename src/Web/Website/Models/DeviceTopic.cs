using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

using Experiment.Data.Metadata;
using Experiment.Data.Enums;

namespace Website.Models
{
	[Table("tblDeviceTopic")]
	public class DeviceTopic : IDeviceTopic
	{
		[Key]
		public int Id { get; set; }

		public int DeviceId { get; set; }

		[StringLength(64)]
		public string Topic { get; set; }
	}
}