using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Web;

using Experiment.Core.Base;
using Experiment.Core.Enums;
using Experiment.Data.Enums;
using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class Datapoint : ViewModelBase, IDatapoint
	{
		#region Const
		string TYPE_NAME = nameof(Datapoint);
		bool DEBUG = true;

		#endregion

		#region Attributes
		int _WriteValue;

		int _Id;
		int _DeviceId;
		int _Order;
		string _Name;
		string _Description;
		string _MeasureUnit;
		DatapointType _DatapointType;
		int _RegisterAddress;
		int _RegisterType;
		int _FunctionCode;
		string _Alias;
		decimal _Multiplier;
		decimal _Offset;
		int _ReadWrite;
		//
		int? _DatapointFormulaId = null;
		DatePartOrInterval _IntervalDatepart = DatePartOrInterval.Day;
		DatePartOrInterval _AggregationDatepart = DatePartOrInterval.None;
		DateTime _LastFormulaCalcTime;

		Device _Device;
		DeviceProtocol? _DeviceProtocol;

		string _Topic;
		string _Theme;
		string _ResourceUri;
		string _Payload;
		int _Instance;
		int _BACnetObjectType;
		int _BACnetPropertyId;
		int _BACnetFunctionCode;
		int _BACnetDataType;

		ICollection<DatapointValue> _Values;

		#endregion

		#region Properties

		public virtual int Id { get => _Id; set => SetProperty(ref _Id, value); }
		public virtual int DeviceId { get => _DeviceId; set => SetProperty(ref _DeviceId, value); }
		public virtual int Order { get => _Order; set => SetProperty(ref _Order, value); }
		public virtual string Name { get => _Name; set => SetProperty(ref _Name, value); }
		public virtual string Description { get => _Description; set => SetProperty(ref _Description, value); }
		public virtual string MeasureUnit { get => _MeasureUnit; set => SetProperty(ref _MeasureUnit, value); }
		public virtual DatapointType DatapointType { get => _DatapointType; set => SetProperty(ref _DatapointType, value); }
		public virtual int RegisterAddress { get => _RegisterAddress; set => SetProperty(ref _RegisterAddress, value); }
		public virtual int RegisterType { get => _RegisterType; set => SetProperty(ref _RegisterType, value); }
		public virtual int FunctionCode { get => _FunctionCode; set => SetProperty(ref _FunctionCode, value); }
		public virtual string Alias { get => _Alias; set => SetProperty(ref _Alias, value); }
		public virtual decimal Multiplier { get => _Multiplier; set => SetProperty(ref _Multiplier, value); }
		public virtual decimal Offset { get => _Offset; set => SetProperty(ref _Offset, value); }
		public virtual int ReadWrite { get => _ReadWrite; set => SetProperty(ref _ReadWrite, value); }
		public virtual int? DatapointFormulaId { get => _DatapointFormulaId; set => SetProperty(ref _DatapointFormulaId, value); }
		public virtual DatePartOrInterval IntervalDatepart { get => _IntervalDatepart; set => SetProperty(ref _IntervalDatepart, value); }
		public virtual DatePartOrInterval AggregationDatepart { get => _AggregationDatepart; set => SetProperty(ref _AggregationDatepart, value); }
		public virtual DateTime LastFormulaCalcTime { get => _LastFormulaCalcTime; set => SetProperty(ref _LastFormulaCalcTime, value); }
		public virtual Device Device { get => _Device; set => SetProperty(ref _Device, value); }
		public DeviceProtocol? DeviceProtocol { get => _DeviceProtocol; set => SetProperty(ref _DeviceProtocol, value); }

		// New fields since 2023-12-14
		public string Topic { get => _Topic; set => SetProperty(ref _Topic, value); }
		public string Theme { get => _Theme; set => SetProperty(ref _Theme, value); }
		public string ResourceUri { get => _ResourceUri; set => SetProperty(ref _ResourceUri, value); }
		public string Payload { get => _Payload; set => SetProperty(ref _Payload, value); }
		public int Instance { get => _Instance; set => SetProperty(ref _Instance, value); }
		public int BACnetObjectType { get => _BACnetObjectType; set => SetProperty(ref _BACnetObjectType, value); }
		public int BACnetPropertyId { get => _BACnetPropertyId; set => SetProperty(ref _BACnetPropertyId, value); }
		public int BACnetFunctionCode { get => _BACnetFunctionCode; set => SetProperty(ref _BACnetFunctionCode, value); }
		public int BACnetDataType { get => _BACnetDataType; set => SetProperty(ref _BACnetDataType, value); }

		public ICollection<DatapointValue> Values
		{
			get => _Values;
			set
			{
				if (DEBUG)
				{
					var vLoc = string.Format("{0}::{1}[SET] (Id={2}, Name={3})", TYPE_NAME, nameof(Datapoint.Values), Id, Name);
					if (value != null)
					{
						Debug.WriteLineIf(DEBUG, string.Format("{0}, value.Count={1}", vLoc, value.Count));
					}
					else
					{
						Debug.WriteLineIf(DEBUG, string.Format("{0}, value=NULL!", vLoc));
					}
				}
				SetProperty(ref _Values, value);
			}
		}
		public virtual ICollection<DatapointFormulaChain> Chains { get; set; }

		#endregion

		#region Ctor
		public Datapoint ()
		{
			Multiplier = 1;

			// New fields since 2023-12-14
			BACnetObjectType = 1;
			BACnetPropertyId = 1;
			BACnetFunctionCode = 1;
			BACnetDataType = 1;
		}

		#endregion
	}
}
