using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
	public interface IDatapointFormulaChain
	{
		int Id { get; set; }
		int DatapointId { get; set; }
		int Order { get; set; }
		int? RelatedDatapointId { get; set; }
		decimal? Value { get; set; }
	}
}
