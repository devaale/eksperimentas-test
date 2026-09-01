using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Core.Metadata{
	interface IDecoder : IEncoder
	{
		string Decode(string s);
	}
}
