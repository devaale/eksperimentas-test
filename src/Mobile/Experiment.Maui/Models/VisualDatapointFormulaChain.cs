using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Experiment.Data.Models;
using Experiment.Maui.Services;
using Newtonsoft.Json;

namespace Experiment.Maui.Models{
    public class VisualDatapointFormulaChain : DatapointFormulaChain
    {
		const string TYPE_NAME = nameof(VisualDatapointFormulaChain);

		ApiServices _ApiServices = new ApiServices();
		Datapoint _RelatedDatapoint;
		string _relatedPickHint = string.Empty;

		public override decimal? Value
		{
			get => base.Value;
			set
			{
				var vLoc = string.Format("{0}::{1}-SET: {2}", TYPE_NAME, nameof(Value), value);
				//Debug.WriteLine(vLoc); 

				base.Value = value;
				// Each other resets each other, as only Value or RelatedDatapointId can be set
				if (base.Value != null && RelatedDatapointId != null)
				{
					Debug.WriteLine(string.Format("{0}, Resetting {1}", vLoc, nameof(RelatedDatapointId)));
					RelatedDatapoint = null;
				}

				OnPropertyChanged(nameof(IsValid));
			}
		}

		public override int? RelatedDatapointId
		{
			get => base.RelatedDatapointId;
			set
			{
				var vLoc = string.Format("{0}::{1}-SET: {2}", TYPE_NAME, nameof(RelatedDatapointId), value);
				//Debug.WriteLine(vLoc);

				base.RelatedDatapointId = value;
				// Each other resets each other, as only Value or RelatedDatapointId can be set
				if (base.RelatedDatapointId != null && Value != null)
				{
					Debug.WriteLine(string.Format("{0}, Resetting {1}", vLoc, nameof(Value)));
					Value = null;
				}

				OnPropertyChanged(nameof(IsValid));
			}
		}

		/// <summary>Shown on the formula-chain row when no related datapoint is chosen (same idea as Picker title).</summary>
		public void SetRelatedPickHint(string hint)
		{
			_relatedPickHint = hint ?? string.Empty;
			OnPropertyChanged(nameof(RelatedDatapointPickDisplayText));
		}

		/// <summary>Text for the datapoint picker button (name or hint).</summary>
		[JsonIgnore]
		public string RelatedDatapointPickDisplayText => RelatedDatapoint?.Name ?? _relatedPickHint;

		/// <summary>
		/// Picker selected item
		/// </summary>
		[JsonIgnore]
		public Datapoint RelatedDatapoint
		{
			get => _RelatedDatapoint;
			set
			{
				var vLoc = string.Format("{0}::{1}-SET: {2}", TYPE_NAME, nameof(RelatedDatapoint), value);
				//Debug.WriteLine(vLoc);

				SetProperty(ref _RelatedDatapoint, value);

				if (_RelatedDatapoint != null)
				{
					RelatedDatapointId = _RelatedDatapoint.Id;
				} else
				{
					RelatedDatapointId = null;
				}

				OnPropertyChanged(nameof(RelatedDatapointPickDisplayText));
			}
		}

		[JsonIgnore]
		public bool IsValid
		{
			get
			{
				// Only one should be entered, not both
				return (!Value.HasValue && RelatedDatapointId.HasValue) ||
					(Value.HasValue && !RelatedDatapointId.HasValue);
			}
		}
	}
}
