using Experiment.Data.Enums;
using Newtonsoft.Json;

namespace Experiment.DeviceProcessing.Service.Models{
	public class AiDecisionOnSpotResult
	{
		public const string JSONP_DECISION = "decision";

		[JsonProperty(JSONP_DECISION)]
		public TemperatureAction Decision { get; set; }
	}
}
