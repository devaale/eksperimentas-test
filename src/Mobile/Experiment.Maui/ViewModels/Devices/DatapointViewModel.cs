using System;
using System.Data;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;

using Experiment.Core.Enums;
using Experiment.Data.Enums;
using Experiment.Data.Metadata;
using Experiment.Data.Models;
using Experiment.Core.Ui;

// MVVM
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Modbus;

using D = Experiment.Maui.Data;

using Experiment.Maui.Services;
using Experiment.Maui.Views.Devices;
using Experiment.Maui.Models;
using Experiment.Maui.Views.Control;
using Experiment.Maui.ViewModels.Control;

namespace Experiment.Maui.ViewModels.Devices{
	public class DatapointViewModel : M.Datapoint
	{
		#region Const
		const string TYPE_NAME = nameof(DatapointViewModel);
		const bool DEBUG = false;

		// We have enum DatePartOrInterval, but it has upper case names, which not fits for us, and numeric values as int type, what not fits too.
		// That's why i'm using custom approach here. As we need value or id as string, eg. day, hour.
		const string LABEL_MINUTE = "minute";
		const string LABEL_HOUR = "hour";
		const string LABEL_DAY = "day";
		const string LABEL_WEEK = "week";
		const string LABEL_MONTH = "month";
		const string LABEL_QUARTER = "quarter";
		const string LABEL_YEAR = "year";

		readonly static Style NegativeButton = (Style)Application.Current.Resources["negativeButton"];

		/// <summary>
		/// Minimum allowed interval value, in minutes of course, as they are smaller time unit.
		/// </summary>
		const int MINIMUM_MINS = 10;

		/// <summary>
		/// Default log level
		/// </summary>
		public const int DEFAULT_LOG_LEVEL = 5;

		#endregion

		#region Attributes
		ApiServices _ApiServices = new ApiServices();
		PickerHandler<NamedDbItem<DatePartOrInterval>> _IntervalDateparts;
		PickerHandler<NamedDbItem<DatePartOrInterval>> _AggregationDateparts;
		ObservableCollection<VisualDatapointFormula> _DatapointFormulas = new ObservableCollection<VisualDatapointFormula>();
		VisualDatapointFormula _SelectedFormula;
		ObservableCollection<VisualDatapointFormulaChain> _Chains = new ObservableCollection<VisualDatapointFormulaChain>();
		VisualDatapointFormulaChain _SelectedChain;
		ObservableCollection<Datapoint> _RelatedDatapoints = new ObservableCollection<Datapoint>();
		readonly List<Datapoint> _AllRelatedDatapoints = new List<Datapoint>();
		string _RelatedDatapointPickerFilterText = string.Empty;

		//static ExpSql _Db;
		//static ILogger _Logger;
		protected int _WriteValue;
		protected bool _IsWriteOptionVisible;
		protected bool _IsReadOptionVisible;

		PickerHandler<NamedDbItem<int>> _BACnetObjectTypes;
		PickerHandler<NamedDbItem<int>> _BACnetPropertyIds;
		PickerHandler<NamedDbItem<int>> _BACnetFunctionCodes;
		PickerHandler<NamedDbItem<int>> _BACnetDataTypes;

		#endregion

		#region Properties
		public bool Selected { get; set; }

		#region Experiment.Modbus Datapoint?

		public string RegisterTypeName
		{
			get
			{
				string retVal = E.T("uncategorized");
				if (Hardcoded.RegisterTypes.ContainsKey(RegisterType))
				{
					retVal = Hardcoded.RegisterTypes[RegisterType];
				}
				Debug.WriteLine("Returning name: " + retVal);
				return retVal;
			}
		}

		/// <summary>
		/// For legacy picker bindings that don't support normal bindings.
		/// </summary>
		public KeyValuePair<int, string> SelectedRegisterType
		{
			get
			{
				var retVal = Hardcoded.RegisterTypes.FirstOrDefault(i => i.Key.Equals(RegisterType));
				return retVal;
			}

			set
			{
				if (Hardcoded.RegisterTypes.ContainsKey(value.Key))
				{
					RegisterType = value.Key;
					Debug.WriteLine("Datapoint::SelectedRegisterType, assigned Type: " + RegisterType);
				}
				else
				{
					throw new KeyNotFoundException();
				}
			}
		}

		public string ReadWriteTypeName
		{
			get
			{
				string retVal = E.T("uncategorized");
				if (Hardcoded.ReadWriteTypes.ContainsKey(ReadWrite))
				{
					retVal = Hardcoded.ReadWriteTypes[ReadWrite];
				}
				Debug.WriteLine("Returning name: " + retVal);
				return retVal;
			}
		}

		/// <summary>
		/// For legacy picker bindings that don't support normal bindings.
		/// </summary>
		public KeyValuePair<int, string> SelectedReadWriteType
		{
			get
			{
				var retVal = Hardcoded.ReadWriteTypes.FirstOrDefault(i => i.Key.Equals(ReadWrite));
				return retVal;
			}

			set
			{
				if (Hardcoded.ReadWriteTypes.ContainsKey(value.Key))
				{
					ReadWrite = value.Key;
					Debug.WriteLine("Datapoint::SelectedRegisterType, assigned Type: " + ReadWrite);
				}
				else
				{
					throw new KeyNotFoundException();
				}
			}
		}

		public string FunctionCodeName
		{
			get
			{
				string retVal = E.T("uncategorized");
				if (Hardcoded.FunctionCodes.ContainsKey(FunctionCode))
				{
					retVal = Hardcoded.FunctionCodes[FunctionCode];
				}
				Debug.WriteLine("Returning name: " + retVal);
				return retVal;
			}
		}

		/// <summary>
		/// For legacy picker bindings that don't support normal bindings.
		/// </summary>
		public KeyValuePair<int, string> SelectedFunctionCode
		{
			get
			{
				var retVal = Hardcoded.FunctionCodes.FirstOrDefault(i => i.Key.Equals(FunctionCode));
				return retVal;
			}

			set
			{
				if (Hardcoded.FunctionCodes.ContainsKey(value.Key))
				{
					FunctionCode = value.Key;
					Debug.WriteLine("Datapoint::SelectedFunctionCode, assigned Code: " + FunctionCode);
				}
				else
				{
					throw new KeyNotFoundException();
				}
			}
		}

		public int WriteValue
		{
			get => _WriteValue;
			set => SetProperty(ref _WriteValue, value);
		}

		#endregion

		public override string Name
		{
			get => base.Name;
			set
			{
				base.Name = value;

				OnPropertyChanged(nameof(IsValid));
			}
		}

		public override int? DatapointFormulaId
		{
			get => base.DatapointFormulaId;
			set
			{
				base.DatapointFormulaId = value;
				OnPropertyChanged(nameof(IsValid));
			}
		}

		/// <summary>
		/// IntervalDateparts
		/// </summary>
		[JsonIgnore]
		public PickerHandler<NamedDbItem<DatePartOrInterval>> IntervalDateparts
		{
			get
			{
				if (_IntervalDateparts == null)
				{
					_IntervalDateparts = new PickerHandler<NamedDbItem<DatePartOrInterval>>(
						this,
						nameof(IntervalDatepart),
						nameof(NamedDbItem<DatePartOrInterval>.Id));

					_IntervalDateparts.AddRange(new NamedDbItem<DatePartOrInterval>[]
					{
						//new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.None, Name = E.T("none") },
						//new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Minute, Name = E.T("minute") },    // Minute was removed under Martynas demand
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Hour, Name = E.T("hour") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Day, Name = E.T("day") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Week, Name = E.T("week") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Month, Name = E.T("month") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Quarter, Name = E.T("quarter") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Year, Name = E.T("year") },
					});
				}

				return _IntervalDateparts;
			}
		}

		public override DatePartOrInterval IntervalDatepart
		{
			get => base.IntervalDatepart;
			set
			{
				base.IntervalDatepart = value;
				OnPropertyChanged(nameof(IsValid));
			}
		}

		/// <summary>
		/// IntervalDateparts
		/// </summary>
		[JsonIgnore]
		public PickerHandler<NamedDbItem<DatePartOrInterval>> AggregationDateparts
		{
			get
			{
				if (_AggregationDateparts == null)
				{
					_AggregationDateparts = new PickerHandler<NamedDbItem<DatePartOrInterval>>(
						this,
						nameof(AggregationDatepart),
						nameof(NamedDbItem<DatePartOrInterval>.Id));

					_AggregationDateparts.AddRange(new NamedDbItem<DatePartOrInterval>[]
					{
						//new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.None, Name = E.T("none") },
						//new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Minute, Name = E.T("minute") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Hour, Name = E.T("hour") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Day, Name = E.T("day") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Week, Name = E.T("week") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Month, Name = E.T("month") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Quarter, Name = E.T("quarter") },
						new NamedDbItem<DatePartOrInterval>() { Id = DatePartOrInterval.Year, Name = E.T("year") },
					});
				}

				return _AggregationDateparts;
			}
		}

		public override DatePartOrInterval AggregationDatepart
		{
			get => base.AggregationDatepart;
			set
			{
				base.AggregationDatepart = value;

				OnPropertyChanged(nameof(IsValid));
			}
		}

		[JsonIgnore]
		public ObservableCollection<VisualDatapointFormula> DatapointFormulas
		{
			get => _DatapointFormulas;
			set => SetProperty(ref _DatapointFormulas, value);
		}

		[JsonIgnore]
		public VisualDatapointFormula SelectedFormula
		{
			get => _SelectedFormula;
			set
			{
				SetProperty(ref _SelectedFormula, value);

				if (value != null)
				{
					// Transfer formula id to db field, which stores it
					DatapointFormulaId = value.Id;

#if MARTYNAS_ENFORCE_MORE_THAN_ONE_CHAIN
                    // Disabled beause in case of single Chain was added one more empty,
                    // which caused that user was forced to deal with one more chain, which he doesn't need
                    if (!HasMoreThanOneChain)
                        ChainAdd();
#endif
					if (value.NumDatapoints > 0)
					{
						if (value.NumDatapoints > Chains.Count)
						{
							for (int i = Chains.Count; i < value.NumDatapoints; i++)
							{
								ChainAdd();
							}
						}
					}

					_ = LoadPresetChainsAsync(value.Id);
				}

				OnPropertyChanged(nameof(FormulaSelected));
				OnPropertyChanged(nameof(HasAggregationDatepart));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public new ObservableCollection<VisualDatapointFormulaChain> Chains
		{
			get => _Chains;
			set => SetProperty(ref _Chains, value);
		}

		public bool IsChainsValid
		{
			get
			{
				if (Chains == null)
					return false;
				return Chains.All(c => c.IsValid);
			}
		}

		public Style ChainsButtonStyle
		{
			get
			{
				if (IsChainsValid)
				{
					return null;
				}
				else
				{
					return NegativeButton;
				}
			}

		}

		public void ChainAdd()
		{
			var newOrder = Chains.Count;
			var chain = new VisualDatapointFormulaChain()
			{
				DatapointId = Id,
				Order = newOrder,
				ExpectedDataPointName = _PresetChains?
					.FirstOrDefault(p => p.Order == newOrder)?
					.ExpectedDataPointName,
			};
			Chains.Add(chain);

			chain.SetRelatedPickHint(LabelDatapoint);

			OnPropertyChanged(nameof(HasMoreThanOneChain));
			OnPropertyChanged(nameof(LabelFormulaParts));
		}

		public void ChainDelete()
		{
			var last = Chains.LastOrDefault();
			if (last != null && Chains.Count > 1)
			{
				Chains.Remove(last);
			}

			OnPropertyChanged(nameof(HasMoreThanOneChain));
			OnPropertyChanged(nameof(LabelFormulaParts));
		}

		List<DatapointFormulaPresetChain> _PresetChains;

		async Task LoadPresetChainsAsync(int formulaId)
		{
			try
			{
				_PresetChains = await _ApiServices.DatapointFormulaPresetChainsAsync(formulaId);
			}
			catch
			{
				_PresetChains = null;
			}

			AssignExpectedDataPointNames();
		}

		void AssignExpectedDataPointNames()
		{
			if (Chains == null)
				return;

			foreach (var chain in Chains)
			{
				chain.ExpectedDataPointName = _PresetChains?
					.FirstOrDefault(p => p.Order == chain.Order)?
					.ExpectedDataPointName;
			}
		}

		public bool HasMoreThanOneChain
		{
			get => Chains.Count > 1;
		}

		/// <summary>
		/// Unused today. Was the idea maybe to open in separate dialogue specific DatapointFormulaChain, but I redecided not to use it.
		/// It still assigned on XAML, but disabled selection mechanism itself.
		/// </summary>
		[JsonIgnore]
		public VisualDatapointFormulaChain SelectedChain
		{
			get => _SelectedChain;
			set
			{
				SetProperty(ref _SelectedChain, value);
				if (_SelectedChain != null)
				{
					// OpenChain(_SelectedChain);
				}
			}
		}


		[JsonIgnore]
		public ObservableCollection<Datapoint> RelatedDatapoints
		{
			get => _RelatedDatapoints;
			set => SetProperty(ref _RelatedDatapoints, value);
		}

		/// <summary>Filters the searchable list inside the related-datapoint popup by name, case-insensitive.</summary>
		public string RelatedDatapointPickerFilterText
		{
			get => _RelatedDatapointPickerFilterText;
			set
			{
				if (SetProperty(ref _RelatedDatapointPickerFilterText, value))
					ApplyRelatedDatapointPickerFilter();
			}
		}

		/// <summary>
		/// Record was saved from database, so it has already Id
		/// </summary>
		[JsonIgnore]
		public bool HasValidId { get => Id != 0; }

		[JsonIgnore]
		public bool HasAggregationDatepart
		{
			get
			{
				if (SelectedFormula != null)
				{
					return SelectedFormula.Aggregated;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsReadOptionVisible
		{
			get => _IsReadOptionVisible;
			set => SetProperty(ref _IsReadOptionVisible, value);
		}

		[JsonIgnore]
		public bool IsWriteOptionVisible
		{
			get => _IsWriteOptionVisible;
			set => SetProperty(ref _IsWriteOptionVisible, value);
		}


		[JsonIgnore]
		public bool IsValid
		{
			get
			{
				var retVal = !string.IsNullOrEmpty(Name);

				if (DatapointType == DatapointType.Virtual)
				{
					// Name not empty, formula selected, interval specified
					retVal = DatapointFormulaId.HasValue &&
						IntervalDatepart != DatePartOrInterval.None &&
						IsChainsValid &&
						SelectedFormula != null;

					// If success
					if (retVal)
					{
						// If it aggerggated
						if (SelectedFormula.Aggregated)
						{
							// Should have aggregation date part selected too
							retVal = AggregationDatepart != DatePartOrInterval.None;
						}
					}
				}

				return retVal;
			}
		}

		public bool IsBacnet { get => DeviceProtocol == Experiment.Data.Enums.DeviceProtocol.BACnet; }
		public bool IsMqtt { get => DeviceProtocol == Experiment.Data.Enums.DeviceProtocol.MQTT; }
		public bool IsOpenthread { get => DeviceProtocol == Experiment.Data.Enums.DeviceProtocol.OpenThread; }

		public PickerHandler<NamedDbItem<int>> BACnetObjectTypes
		{
			get
			{
				if (_BACnetObjectTypes == null)
				{
					_BACnetObjectTypes = new PickerHandler<NamedDbItem<int>>(
						this,
						nameof(BACnetObjectType),
						nameof(NamedDbItem<int>.Id));

					_BACnetObjectTypes.AddRange(new NamedDbItem<int>[]
					{
						new NamedDbItem<int>() { Id = 1, Name = E.T("analogInputAI") },
						new NamedDbItem<int>() { Id = 2, Name = E.T("analogOutputAO") },
						new NamedDbItem<int>() { Id = 3, Name = E.T("binaryInputBI") },
						new NamedDbItem<int>() { Id = 4, Name = E.T("binaryOutputBO") },
						new NamedDbItem<int>() { Id = 5, Name = E.T("multistateInputMI") },
						new NamedDbItem<int>() { Id = 6, Name = E.T("multistateOutputMO") },
						new NamedDbItem<int>() { Id = 7, Name = E.T("calendar") },
						new NamedDbItem<int>() { Id = 8, Name = E.T("trendLog") },
					});
				}

				return _BACnetObjectTypes;
			}
		}

		public PickerHandler<NamedDbItem<int>> BACnetPropertyIds
		{
			get
			{
				if (_BACnetPropertyIds == null)
				{
					_BACnetPropertyIds = new PickerHandler<NamedDbItem<int>>(
						this,
						nameof(BACnetPropertyId),
						nameof(NamedDbItem<int>.Id));

					_BACnetPropertyIds.AddRange(new NamedDbItem<int>[]
					{
						new NamedDbItem<int>() { Id = 1, Name = E.T("presentValue85") },
						new NamedDbItem<int>() { Id = 2, Name = E.T("statusFlags111") },
						new NamedDbItem<int>() { Id = 3, Name = E.T("objectName77") },
						new NamedDbItem<int>() { Id = 4, Name = E.T("highLimit56") },
						new NamedDbItem<int>() { Id = 5, Name = E.T("lowlimit54") },
						new NamedDbItem<int>() { Id = 6, Name = E.T("description28") },
						new NamedDbItem<int>() { Id = 7, Name = E.T("eventState23") },
						new NamedDbItem<int>() { Id = 8, Name = E.T("lifeSafetyAlarm121") },
						new NamedDbItem<int>() { Id = 9, Name = E.T("alarmValue101") },
						new NamedDbItem<int>() { Id = 10, Name = E.T("priorityArray87") },
						new NamedDbItem<int>() { Id = 11, Name = E.T("units19") },
						new NamedDbItem<int>() { Id = 12, Name = E.T("reliability65") },
						new NamedDbItem<int>() { Id = 13, Name = E.T("resolution118") },
					});
				}

				return _BACnetPropertyIds;
			}
		}

		public PickerHandler<NamedDbItem<int>> BACnetFunctionCodes
		{
			get
			{
				if (_BACnetFunctionCodes == null)
				{
					_BACnetFunctionCodes = new PickerHandler<NamedDbItem<int>>(
						this,
						nameof(BACnetFunctionCode),
						nameof(NamedDbItem<int>.Id));

					_BACnetFunctionCodes.AddRange(new NamedDbItem<int>[]
					{
						new NamedDbItem<int>() { Id = 1, Name = E.T("readProperty0x0C") },
						new NamedDbItem<int>() { Id = 2, Name = E.T("writeProperty0x0F") },
						new NamedDbItem<int>() { Id = 3, Name = E.T("whoIs0x01") },
						new NamedDbItem<int>() { Id = 4, Name = E.T("iAm0x02") },
						new NamedDbItem<int>() { Id = 5, Name = E.T("readPropertyMultiple0x10") },
						new NamedDbItem<int>() { Id = 6, Name = E.T("writePropertyMultiple0x12") },
					});
				}

				return _BACnetFunctionCodes;
			}
		}

		public PickerHandler<NamedDbItem<int>> BACnetDataTypes
		{
			get
			{
				if (_BACnetDataTypes == null)
				{
					_BACnetDataTypes = new PickerHandler<NamedDbItem<int>>(
						this,
						nameof(BACnetDataType),
						nameof(NamedDbItem<int>.Id));

					_BACnetDataTypes.AddRange(new NamedDbItem<int>[]
					{
						new NamedDbItem<int>() { Id = 1, Name = E.T("Boolean") },
						new NamedDbItem<int>() { Id = 2, Name = E.T("Unsigned Integer") },
						new NamedDbItem<int>() { Id = 3, Name = E.T("Signed Integer") },
						new NamedDbItem<int>() { Id = 4, Name = E.T("Real (Float)") },
						new NamedDbItem<int>() { Id = 5, Name = E.T("Double") },
						new NamedDbItem<int>() { Id = 6, Name = E.T("Character String") },
						new NamedDbItem<int>() { Id = 7, Name = E.T("Bit String") },
						new NamedDbItem<int>() { Id = 8, Name = E.T("Enumerated") },
						new NamedDbItem<int>() { Id = 9, Name = E.T("Date and Time") },
					});
				}

				return _BACnetDataTypes;
			}
		}

		[JsonIgnore]
		public bool FormulaSelected
		{
			get => _SelectedFormula != null;
		}

		[JsonIgnore]
		public string LabelName { get => E.T("name"); }

		[JsonIgnore]
		public string LabelDescription { get => E.T("description"); }

		[JsonIgnore]
		public string LabelDeviceId { get => E.T("deviceId"); }

		[JsonIgnore]
		public string LabelOrder { get => string.Format("{0}: ", E.T("order")); }

		[JsonIgnore]
		public string LabelMeasureUnit { get => E.T("measure-unit"); }

		[JsonIgnore]
		public string LabelAlias { get => E.T("alias"); }

		[JsonIgnore]
		public string LabelRegisterAddress { get => E.T("register-address"); }

		[JsonIgnore]
		public string LabelRegisterType { get => E.T("register-type"); }

		[JsonIgnore]
		public string LabelFunctionCode { get => E.T("function-code"); }

		[JsonIgnore]
		public string LabelMultiplier { get => E.T("multiplier"); }

		[JsonIgnore]
		public string LabelOffset { get => E.T("offset"); }

		[JsonIgnore]
		public string LabelReadValue { get => E.T("readValue"); }

		[JsonIgnore]
		public string LabelRead { get => E.T("read"); }

		[JsonIgnore]
		public string LabelReadWrite { get => E.T("read-write"); }

		[JsonIgnore]
		public string LabelSendValue { get => E.T("sendValue"); }

		[JsonIgnore]
		public string LabelValue { get => E.T("value"); }

		[JsonIgnore]
		public string LabelSend { get => E.T("send"); }

		[JsonIgnore]
		public string LabelSave { get => E.T("save"); }

		[JsonIgnore]
		public string LabelAdd { get => E.T("add"); }

		[JsonIgnore]
		public string LabelEdit { get => E.T("edit"); }

		[JsonIgnore]
		public string LabelDelete { get => E.T("delete"); }

		[JsonIgnore]
		public string LabelCancel { get => E.T("cancel"); }

		[JsonIgnore]
		public string LabelCreateAlarm { get => E.T("createAlarm"); }

		[JsonIgnore]
		public string LabelFunction { get => E.T("function"); }

		[JsonIgnore]
		public string LabelInterval { get => E.T("interval"); }

		[JsonIgnore]
		public string LabelAggregateBy { get => E.T("aggregateBy"); }

		[JsonIgnore]
		public string LabelFormulaPart { get => E.T("formulaPart"); }

		[JsonIgnore]
		public string LabelFormulaParts
		{
			get => string.Format("{0} ({1})", E.T("formulaParts"), GetChainsCount());
		}

		[JsonIgnore]
		public string LabelDatapoint { get => E.T("datapoint"); }

		// New fields since 2023-12-14
		[JsonIgnore]
		public string LabelInstance { get => E.T("instance"); }
		[JsonIgnore]
		public string LabelObjectType { get => E.T("objectType"); }
		[JsonIgnore]
		public string LabelPropertyId { get => E.T("propertyId"); }
		[JsonIgnore]
		public string LabelDataType { get => E.T("dataType"); }
		[JsonIgnore]
		public string LabelTopic { get => E.T("topic"); }
		[JsonIgnore]
		public string LabelResourceUri { get => E.T("resourceUri"); }
		[JsonIgnore]
		public string LabelPayload { get => E.T("payload"); }

		#endregion

		#region Ctor
		public DatapointViewModel()
		{
			RegisterType = Hardcoded.RegisterTypes.FirstOrDefault().Key;
			FunctionCode = Hardcoded.FunctionCodes.FirstOrDefault().Key;

			//_Logger = new FileLogger(DEFAULT_LOG_LEVEL, Defaults.DEFAULT_LOG_FOLDER, TYPE_NAME);
			//_Logger = new ConsoleLogger(DEFAULT_LOG_LEVEL, TYPE_NAME);

			WriteValue = 0;
			IsReadOptionVisible = false;
			IsWriteOptionVisible = false;
		}

		#endregion

		#region Helpers

		void ApplyRelatedDatapointPickerFilter()
		{
			if (_AllRelatedDatapoints.Count == 0)
				return;

			var q = _RelatedDatapointPickerFilterText?.Trim() ?? string.Empty;
			var inv = CultureInfo.CurrentCulture.CompareInfo;

			var neededIds = new HashSet<int>();
			if (Chains != null)
			{
				foreach (var c in Chains)
				{
					if (c?.RelatedDatapoint != null)
						neededIds.Add(c.RelatedDatapoint.Id);
					else if (c?.RelatedDatapointId is int id)
						neededIds.Add(id);
				}
			}

			_RelatedDatapoints.Clear();
			foreach (var dp in _AllRelatedDatapoints)
			{
				var nameMatch = dp.Name != null && q.Length > 0 &&
					inv.IndexOf(dp.Name, q, CompareOptions.IgnoreCase) >= 0;
				var emptyFilter = q.Length == 0;
				var keepForSelection = neededIds.Contains(dp.Id);
				if (emptyFilter || nameMatch || keepForSelection)
					_RelatedDatapoints.Add(dp);
			}
		}

		public async Task OpenRelatedDatapointPickerAsync(Page page, VisualDatapointFormulaChain chain)
		{
			if (page == null || chain == null || _AllRelatedDatapoints.Count == 0)
				return;

			_RelatedDatapointPickerFilterText = string.Empty;
			OnPropertyChanged(nameof(RelatedDatapointPickerFilterText));
			ApplyRelatedDatapointPickerFilter();

			var popup = new RelatedDatapointPickerPopup(chain, this);
			await page.ShowPopupAsync(popup);
		}

		protected async Task OpenChain(DatapointFormulaChain chain)
		{
			await Application.Current.MainPage.Navigation.PushAsync(new VirtualDatapointChainPage()
			{
				BindingContext = chain,
			});
		}

		int GetChainsCount()
		{
			if (Chains != null)
				return Chains.Count;

			return 0;
		}

		#endregion

		#region Methods
		public async Task LoadAsync(object sender)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(LoadAsync));

			try
			{
				IsBusy = true;

				if (DatapointType == DatapointType.Virtual)
				{
					// Until datapoint formulas not loaded, load datapoint formulas.
					if (DatapointFormulas.Count == 0)
					{
						var dfs = await _ApiServices.DatapointFormulaListAsync(D.Settings.Language);
						foreach (var df in dfs)
						{
							DatapointFormulas.Add(df);
						}

						// Restoring selected formula after formulas list load
						if (DatapointFormulaId.HasValue)
						{
							SelectedFormula = DatapointFormulas.FirstOrDefault(df => df.Id.Equals(DatapointFormulaId.Value));
						}
					}

					if (_AllRelatedDatapoints.Count == 0)
					{
						var list = await _ApiServices.DatapointListAsync();
						_AllRelatedDatapoints.Clear();
						foreach (var dp in list ?? Enumerable.Empty<Datapoint>())
						{
							Debug.WriteLine(string.Format("{0}, add, Id={1}, Name={2}", vLoc, dp.Id, dp.Name));
							_AllRelatedDatapoints.Add(dp);
						}
					}

					if (Chains != null && _AllRelatedDatapoints.Count > 0)
					{
						foreach (var chain in Chains)
						{
							if (chain.RelatedDatapointId.HasValue && chain.RelatedDatapoint == null)
							{
								chain.RelatedDatapoint = _AllRelatedDatapoints.FirstOrDefault(dp => dp.Id.Equals(chain.RelatedDatapointId.Value));
							}
						}
					}

					if (Chains != null)
					{
						foreach (var c in Chains)
							c.SetRelatedPickHint(LabelDatapoint);
					}

					if (DatapointFormulaId.HasValue && _PresetChains == null)
					{
						await LoadPresetChainsAsync(DatapointFormulaId.Value);
					}

					OnPropertyChanged(nameof(ChainsButtonStyle));
					OnPropertyChanged(nameof(IsChainsValid));
					OnPropertyChanged(nameof(IsValid));
				}

				IsReadOptionVisible = IsValid && ReadWrite == 0 && DatapointType == DatapointType.Normal;
				IsWriteOptionVisible = IsValid && ReadWrite == 1 && DatapointType == DatapointType.Normal;
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert(
					vLoc,
					E.T("err-list-load") + Environment.NewLine + Environment.NewLine + ex.Message,
					E.T("ok"));
			}
			finally
			{
				IsBusy = false;
			}
		}
		#endregion

		#region Commands

		public ICommand CancelCommand
		{
			get
			{
				return new Command(async () =>
				{
					await Application.Current.MainPage.Navigation.PopAsync();
				});
			}
		}

		public ICommand PostDatapointCommand
		{
			get
			{
				return new Command(async () =>
				{
					var vLoc = $"{TYPE_NAME}::{nameof(PostDatapointCommand)}";

					try
					{
						if (!IsValid)
							return;

						if (!IsChainsValid)
						{
							await Application.Current.MainPage.DisplayAlert(
								E.T("validation"),
								E.T("errFormulaParts"),
								E.T("cancel"));
							return;
						}

						IsBusy = true;

						HttpResponseMessage response;
						if (HasValidId)
						{
							response = await _ApiServices.DatapointPutAsync(this);
						}
						else
						{
							response = await _ApiServices.DatapointPostAsync(this);
						}
#if PROCESS_RESPONSES
						E.ProcessResponse(response);
#endif
						// Close Datapoint info ContentPage
						await Application.Current.MainPage.Navigation.PopAsync();
					}
					catch (Exception ex)
					{
						await Application.Current.MainPage.DisplayAlert(
							vLoc,
							E.T("err-op") + Environment.NewLine + Environment.NewLine + ex.Message,
							E.T("ok"));
					}
					finally
					{
						IsBusy = false;
					}
				});
			}
		}

		public ICommand DeleteDatapointCommand
		{
			get
			{
				return new Command(async () =>
				{
					var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(DeleteDatapointCommand));
					try
					{
						IsBusy = true;
						HttpResponseMessage response;

						if (Id > 0)
						{

							var confirmationResult = await Application.Current.MainPage.DisplayAlert(
								E.T("question"),
								E.T("sure-delete"),
								E.T("yes"),
								E.T("no"));
							if (confirmationResult)
							{
								response = await _ApiServices.DatapointDeleteAsync(this);
#if PROCESS_RESPONSES
								E.ProcessResponse(response);
#endif
								// Close Datapoint info ContentPage
								await Application.Current.MainPage.Navigation.PopAsync();
							}
						}
					}
					catch (Exception ex)
					{
						await Application.Current.MainPage.DisplayAlert(
							vLoc,
							E.T("err-op") + Environment.NewLine + Environment.NewLine + ex.Message,
							E.T("ok"));

					}
					finally
					{
						IsBusy = false;
					}

				});
			}
		}

		public ICommand ReadValueNowCommand
		{
			get
			{
				return new Command(async () =>
				{
					var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(ReadValueNowCommand));
					if (DatapointType != DatapointType.Normal)
					{
						Debug.WriteLine(vLoc + ", Wrong datapoint type!");
						return;
					}

					try
					{
						IsBusy = true;

						M.Datapoint datapoint = this;
						M.Device device = new M.Device();

						// Find device by ObjectId
						var devices = await _ApiServices.DeviceListAsync(
							D.Settings.ObjectId.ToString());

						foreach (var dev in devices)
						{
							if (dev.Id == datapoint.DeviceId)
							{
								device = dev;
								break;
							}
						}

						// Parse device info
						string Host = "";
						int Port = 0;
						int UnitID = 1;

						string[] tokens = device.Url.Split(':');

						if (tokens.Length >= 1)
						{
							Host = tokens[0];
						}

						if (tokens.Length >= 2)
						{
							int port = 0;

							if (int.TryParse(tokens[1], out port))
							{
								Port = port;
							}
						}

						UnitID = device.UnitId;

						// Write value
						decimal result = StandAloneOperations.ReadFromDeviceMobile(
							Host,
							Port,
							UnitID,
							datapoint.RegisterAddress,
							datapoint.FunctionCode,
							datapoint.RegisterType,
							datapoint.Multiplier,
							datapoint.Offset,
							datapoint.Id.ToString());

						await Application.Current.MainPage.DisplayAlert(
							E.T("readValue"),
							result.ToString(),
							E.T("ok"));
					}
					catch (Exception ex)
					{
						await Application.Current.MainPage.DisplayAlert(
							vLoc,
							E.T("err-op") + Environment.NewLine + Environment.NewLine + ex.Message,
							E.T("ok"));
					}
					finally
					{
						IsBusy = false;
					}
				});
			}
		}

		public ICommand WriteValueNowCommand
		{
			get
			{
				return new Command(async () =>
				{
					var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(WriteValueNowCommand));
					if (DatapointType != DatapointType.Normal)
					{
						Debug.WriteLine(vLoc + ", Wrong datapoint type!");
						return;
					}

					try
					{
						IsBusy = true;

						int Value = WriteValue;

						M.Datapoint datapoint = this;
						M.Device device = new M.Device();

						// Find device by ObjectId
						var devices = await _ApiServices.DeviceListAsync(
							D.Settings.ObjectId.ToString());

						foreach (var dev in devices)
						{
							if (dev.Id == datapoint.DeviceId)
							{
								device = dev;
								break;
							}
						}

						// Parse device info
						string Host = "";
						int Port = 0;
						int UnitID = 1;

						string[] tokens = device.Url.Split(':');

						if (tokens.Length >= 1)
						{
							Host = tokens[0];
						}

						if (tokens.Length >= 2)
						{
							int port = 0;

							if (int.TryParse(tokens[1], out port))
							{
								Port = port;
							}
						}

						UnitID = device.UnitId;

						// Write value
						decimal result = StandAloneOperations.WriteToDeviceMobile(
							Host,
							Port,
							UnitID,
							datapoint.RegisterAddress,
							datapoint.FunctionCode,
							datapoint.Id.ToString(), Value);

						await Application.Current.MainPage.DisplayAlert(
							E.T("sendValue"),
							E.T("done"),
							E.T("ok"));
					}
					catch (Exception ex)
					{
						await Application.Current.MainPage.DisplayAlert(
							vLoc,
							E.T("err-op") + Environment.NewLine + Environment.NewLine + ex.Message,
							E.T("ok"));
					}
					finally
					{
						IsBusy = false;
					}
				});
			}
		}

		public ICommand CreateAlarmCommand
		{
			get
			{
				return new Command(async () =>
				{
					await Application.Current.MainPage.Navigation.PushAsync(
						new AlgorithmPage()
						{
							BindingContext = new AlgorithmViewModel()
							{
								Item = new VisualAlgorithm()
								{
									ObjectId = D.Settings.ObjectId,

									CanBeEdited = true,

									// Set default values
									Type = AlgorithmType.Alarm,
									Name = Name,

									DateStart = DateTime.Today,
									DateEnd = DateTime.Today,
									TimeStart = DateTime.Now.TimeOfDay,
									TimeEnd = DateTime.Now.TimeOfDay,

									ValueFrom = 0,
									ValueTo = 1,

									AlarmId = 0,
									GroupId = 0,
									DatapointId = Id,

									ValueOff = 0,
									ValueOn = 1,
								},
							},
						});
				});
			}
		}

		public ICommand EditAlarmCommand
		{
			get
			{
				return new Command(async () =>
				{
				});
			}
		}

		public ICommand DatapointChainsCommand
		{
			get
			{
				return new Command(async () =>
				{
					await Application.Current.MainPage.Navigation.PushAsync(new VirtualDatapointChainsPage()
					{
						BindingContext = this,
					});
				});
			}
		}

		public ICommand DeleteChainCommand
		{
			get
			{
				return new Command(async () =>
				{
					ChainDelete();
				});
			}
		}

		public ICommand AddChainCommand
		{
			get
			{
				return new Command(async () =>
				{
					ChainAdd();
				});
			}
		}

		#endregion
	}
}

