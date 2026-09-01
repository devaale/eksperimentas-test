using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace Experiment.Core.Web{
	public class ExpErrorStatus
	{
		public  const string STATUS_OK = "OK";
		public const string STATUS_ERROR = "ERROR";

		#region Attributes
		string _Status;
		string _ErrorMsg;
		object _ErrorData;

		#endregion
		
		#region Properties

		[JsonProperty(PropertyName = "error-status")]
		public string Status
		{
			get { return _Status; }
			set { _Status = value; }
		}

		[JsonProperty(PropertyName = "error-msg")]
		public string ErrorMsg
		{
			get { return _ErrorMsg; }
			set
			{
				_ErrorMsg = value;
				if(String.IsNullOrEmpty(_ErrorMsg))
				{
					Status = STATUS_OK;
				} else
				{
					Status = STATUS_ERROR;
				}
			}
		}


		[JsonProperty(PropertyName = "error-data")]
		public object ErrorData
		{
			get { return _ErrorData;  }
			set { _ErrorData = value; }
		}

		#endregion

		#region Ctor

		public ExpErrorStatus()
		{
			ErrorMsg = string.Empty;
			ErrorData = string.Empty;
		}

		#endregion
	}
}