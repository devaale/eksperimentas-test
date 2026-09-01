using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
    public class Group : IGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ObjectId { get; set; }

        public DateTime? Deleted { get; set; }

        public ICollection<DatapointSelection> Datapoints { get; set; }
        /*
		public bool CanBeEdited { get; set; }

		public bool CanDelete { get; set; }
        */
		public bool Editable { get; set; }
    }
}