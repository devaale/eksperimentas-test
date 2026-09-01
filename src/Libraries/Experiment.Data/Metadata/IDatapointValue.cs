using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Metadata{
    public interface IDatapointValue
    {
        int Id { get; set; }
        int DatapointId { get; set; }
        DateTime Date { get; set; }
        decimal Value { get; set; }
    }
}
