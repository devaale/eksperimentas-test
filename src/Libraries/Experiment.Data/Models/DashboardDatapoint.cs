using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class DashboardDatapoint : IDashboardDatapoint
	{
		#region Attributes
		Datapoint _Datapoint;

		#endregion

		#region Properties
		public int Id { get; set; }
		public string Name { get; set; }
		public string UserId { get; set; }
		public byte GraphId { get; set; }
		public int DatapointId { get; set; }

		#endregion

		#region Ctor
		public Datapoint AsDatapoint()
		{
			if(_Datapoint == null)
			{
				_Datapoint = new Datapoint()
				{
					Id = DatapointId,
					Name = Name,
				};
			}
			return _Datapoint;
		}

		#endregion
	}
}
