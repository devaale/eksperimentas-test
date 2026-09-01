using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Enums;

namespace Experiment.Data.Metadata{
    public interface IDevice
    {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        //DeviceType Type { get; set; } // Removed 2023-10-10 @AG
        int ObjectId { get; set; }
        string Url { get; set; }
        int UnitId { get; set; }
        int Interval { get; set; }
        DeviceProtocol Protocol { get; set; }
        string ClientId { get; set; }
        string Topic { get; set; }
        decimal DeprGL { get; set; }
        decimal DeprA { get; set; }
        decimal DeprLIR { get; set; }
        decimal DeprRL { get; set; }
        decimal DeprC { get; set; }
        decimal DeprSD { get; set; }
        string ClientUsername { get; set; }
        string ClientPassword { get; set; }
    }
}
