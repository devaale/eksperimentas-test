using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Microsoft.Maui.Controls;

using M=Experiment.Data.Models;

using Experiment.Maui.Data;
using Newtonsoft.Json;

namespace Experiment.Maui.Models{
	public class VisualDevice : M.Device
	{
		public bool Selected { get; set; }

		[JsonIgnore]
		public string ProtocolName
		{
			get
			{
				string retVal = E.T("uncategorized");
				if (Hardcoded.ProtocolTypes.ContainsKey(Protocol))
				{
					retVal = Hardcoded.ProtocolTypes[Protocol];
				}
				Debug.WriteLine("Returning name: " + retVal);
				return retVal;
			}
		}

		[JsonIgnore]
		public bool IsNew { get => Id < 0; }

		public VisualDevice()
			: base()
		{
			ObjectId = Settings.ObjectId;
		}

	}
}

