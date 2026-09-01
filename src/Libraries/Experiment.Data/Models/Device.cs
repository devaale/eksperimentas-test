using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using Experiment.Core.Base;
using Experiment.Data.Metadata;
using Experiment.Data.Enums;

namespace Experiment.Data.Models{
    public class Device : ViewModelBase, IDevice
	{
		#region Attributes
		int _Id;
		string _Name;
		string _Description;
		//DeviceType _Type;	// Removed 2023-10-10 @AG
		int _ObjectId;
		string _Url;
		int _UnitId;
		int _Interval = 100;
		DeviceProtocol _Protocol;
		string _ClientId;
		string _Topic;
		
		decimal _DeprGL;
		decimal _DeprA;
		decimal _DeprLIR;
		decimal _DeprRL;
		decimal _DeprC;
		decimal _DeprSD;

		string _ClientUsername;
		string _ClientPassword;

		DateTime? _LastScanTime;
		DateTime? _ProjectedScanTime;

		ICollection<Datapoint> _Datapoints;

		#endregion

		#region Properties

		public int Id
		{
			get => _Id;
			set => SetProperty(ref _Id, value);
		}

		public string Name
		{
			get => _Name;
			set => SetProperty(ref _Name, value);
		}

		public string Description
		{
			get => _Description;
			set => SetProperty(ref _Description, value);
		}

		//public DeviceType Type { get => _Type; set => SetProperty(ref _Type, value); }	// Removed 2023-10-10 @AG
		public int ObjectId
		{
			get => _ObjectId;
			set => SetProperty(ref _ObjectId, value);
		}

		public string Url
		{
			get => _Url;
			set => SetProperty(ref _Url, value);
		}

		public int UnitId
		{
			get => _UnitId;
			set => SetProperty(ref _UnitId, value);
		}

		public int Interval
		{
			get => _Interval;
			set => SetProperty(ref _Interval, value);
		}

		public DeviceProtocol Protocol
		{
			get => _Protocol;
			set => SetProperty(ref _Protocol, value);
		}

		public string ClientId
		{
			get => _ClientId;
			set => SetProperty(ref _ClientId, value);
		}

		public string Topic
		{
			get => _Topic;
			set => SetProperty(ref _Topic, value);
		}

		public decimal DeprGL { get => _DeprGL; set => SetProperty(ref _DeprGL, value); }
		public decimal DeprA { get => _DeprA; set => SetProperty(ref _DeprA, value); }
		public decimal DeprLIR { get => _DeprLIR; set => SetProperty(ref _DeprLIR, value); }
		public decimal DeprRL { get => _DeprRL; set => SetProperty(ref _DeprRL, value); }
		public decimal DeprC { get => _DeprC; set => SetProperty(ref _DeprC, value); }
		public decimal DeprSD { get => _DeprSD; set => SetProperty(ref _DeprSD, value); }

		public string ClientUsername
		{
			get => _ClientUsername;
			set => SetProperty(ref _ClientUsername, value);
		}
		public string ClientPassword
		{
			get => _ClientPassword;
			set => SetProperty(ref _ClientPassword, value);
		}

		public DateTime? LastScanTime
		{
			get => _LastScanTime;
			set => SetProperty(ref _LastScanTime, value);
		}

		public DateTime? ProjectedScanTime
		{
			get => _ProjectedScanTime;
			set => SetProperty(ref _ProjectedScanTime, value);
		}

		public ICollection<Datapoint> Datapoints
		{
			get => _Datapoints;
			set => SetProperty(ref _Datapoints, value);
		}

		#endregion

	}
}