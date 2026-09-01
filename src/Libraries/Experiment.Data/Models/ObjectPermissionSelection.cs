using Experiment.Data.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Models{
	public class ObjectPermissionSelection : ObjectPermission, ISelectable
	{
		public bool Selected { get; set; }
	}
}
