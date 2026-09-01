using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

using M = Experiment.Data.Models;
using Experiment.Data.Metadata;
using Experiment.Data.Models;

namespace Website.Models
{
	[Table("tblGroupDatapoint")]
	public class GroupDatapoint : IGroupDatapoint
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int GroupId { get; set; }

		[ForeignKey(nameof(GroupId))]
		public Group Group { get; set; }

		[Required]
		public int DatapointId { get; set; }

		[ForeignKey(nameof(DatapointId))]
		public virtual Datapoint Datapoint { get; set; }

		public static IGroupDatapoint ToFrontendObject(IGroupDatapoint subject)
		{
			if (subject == null)
				throw new ArgumentNullException("subject");

			var result = new M.GroupDatapoint()
			{
				Id = subject.Id,
				GroupId = subject.GroupId,
				DatapointId = subject.DatapointId,
			};

			return result;
		}
	}
}
