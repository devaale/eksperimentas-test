using System;
using System.Collections.Generic;
using System.Text;
using Experiment.Core.Base;
using Experiment.Data.Metadata;
using Newtonsoft.Json;

namespace Experiment.Data.Models{
	public class DatapointFormulaChain : ViewModelBase, IDatapointFormulaChain
	{
		int _Id;
		int _DatapointId;
		int _Order;
		int? _RelatedDatapointId;
		decimal? _Value;
		string _ExpectedDataPointName;

		public int Id
		{
			get => _Id;
			set => SetProperty(ref _Id, value);
		}

		public int DatapointId
		{
			get => _DatapointId;
			set => SetProperty(ref _DatapointId, value);
		}

		public int Order
		{
			get => _Order;
			set => SetProperty(ref _Order, value);
		}

		public virtual int? RelatedDatapointId
		{
			get => _RelatedDatapointId;
			set => SetProperty(ref _RelatedDatapointId, value);
		}

		public virtual decimal? Value
		{
			get => _Value;
			set => SetProperty(ref _Value, value);
		}

		[JsonIgnore]
		public string ExpectedDataPointName
		{
			get => _ExpectedDataPointName;
			set => SetProperty(ref _ExpectedDataPointName, value);
		}
	}
}
