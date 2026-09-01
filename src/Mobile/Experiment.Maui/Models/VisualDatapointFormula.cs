using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Maui.Charts;
using Experiment.Data.Models;

namespace Experiment.Maui.Models{
    public class VisualDatapointFormula : DatapointFormula
    {
        public bool HasDynamicChains { get => NumDatapoints == 0; }
    }
}

