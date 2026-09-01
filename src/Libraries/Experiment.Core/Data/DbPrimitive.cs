using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Experiment.Core.Metadata;

namespace Experiment.Core.Data{
	public class DbPrimitive : DataRowItem, IDbPrimitive
	{
		public string Id { get { return this[Defaults.DB_ID].ToString(); } }

		public DbPrimitive(DataRow row):
			base(row)
		{
		}

	}
}
