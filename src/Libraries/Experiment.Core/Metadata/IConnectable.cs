using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Core.Metadata{
	public interface IConnectable
	{
		bool IsConnected { get;  }
		bool Connect(string host, int port, int unitId);
		void Disconnect();
	}
}
