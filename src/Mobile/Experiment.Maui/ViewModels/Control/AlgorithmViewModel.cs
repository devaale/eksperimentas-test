using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Runtime.Serialization;

using Microsoft.Maui.Controls;

// MVVM
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;
using D = Experiment.Maui.Data;

using Experiment.Core.Base;
using Experiment.Core.Ui;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;
using Experiment.Data.Models;

using Experiment.Maui.Enums;
using Experiment.Maui.Data;
using Experiment.Maui.Models;
using Experiment.Maui.Services;

namespace Experiment.Maui.ViewModels.Control{
    public class AlgorithmViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(AlgorithmViewModel);

        #endregion

        #region Attributes
        ApiServices _ApiServices = new ApiServices();
        VisualAlgorithm _Item;

        PickerHandler<NamedDbItem<AlgorithmType>> _AlgorithmTypes;

        PickerHandler<IAlgorithm> _Alarms;
        PickerHandler<IGroup> _Groups;
        PickerHandler<IDatapoint> _Datapoints;
        PickerHandler<IDatapoint> _DatapointsRead;
        PickerHandler<IDatapoint> _DatapointsWrite;

        bool _IsEnabledType = false;
        protected DateTime _DateStart;
        protected DateTime _DateEnd;
        protected TimeSpan _TimeStart;
        protected TimeSpan _TimeEnd;
		string _LabelDatapoint;
		string _LabelGroup;

		#endregion

		#region Properties

		public VisualAlgorithm Item { get => _Item; set => SetProperty(ref _Item, value); }

        public PickerHandler<NamedDbItem<AlgorithmType>> AlgorithmTypes
        {
            //get => _AlgorithmTypes;
            //set => SetProperty(ref _AlgorithmTypes, value);
            get
            {
                //if (null != Datapoints)
                if (null != Item)
                {
                    if (Item.Type == AlgorithmType.Alarm)
                    {
						Datapoints = DatapointsRead;
						LabelDatapoint = E.T("datapoint");
						LabelGroup = E.T("group");
					}

                    else
                    {
						Datapoints = DatapointsWrite;
						LabelDatapoint = E.T("datapoint-write");
						LabelGroup = E.T("group-write");
					}

                    // Set defauult time
					if (Item.Id == 0)
                    {
						TimeStart = new TimeSpan(0, 0, 0);
                        TimeEnd = new TimeSpan(23, 59, 59);
					}	
					else
                    {
						TimeStart = Item.TimeStart.Value;
						TimeEnd = Item.TimeEnd.Value;
					}
				}

                return _AlgorithmTypes;
            }

            set => SetProperty(ref _AlgorithmTypes, value);
        }

        public bool IsEnabledType
        {
            get => _IsEnabledType;
            set => SetProperty(ref _IsEnabledType, value);
        }

        public PickerHandler<IAlgorithm> Alarms
        {
            get => _Alarms;
            set => SetProperty(ref _Alarms, value);
        }

        public PickerHandler<IGroup> Groups
        {
            get => _Groups;
            set => SetProperty(ref _Groups, value);
        }

        public PickerHandler<IDatapoint> Datapoints
        {
            get => _Datapoints;
            set => SetProperty(ref _Datapoints, value);
        }

        public PickerHandler<IDatapoint> DatapointsRead
        {
            get => _DatapointsRead;
            set => SetProperty(ref _DatapointsRead, value);
        }

        public PickerHandler<IDatapoint> DatapointsWrite
        {
            get => _DatapointsWrite;
            set => SetProperty(ref _DatapointsWrite, value);
        }

        /// <summary>
        /// UI Filter date start (Nullable)
        /// </summary>
        public virtual DateTime DateStart
        {
            get => _DateStart;
            set => SetProperty(ref _DateStart, value);
        }

        /// <summary>
        /// UI Filter date end (Nullable)
        /// </summary>
        public virtual DateTime DateEnd
        {
            get => _DateEnd;
            set => SetProperty(ref _DateEnd, value);
        }

        /// <summary>
        /// UI Filter date start (Nullable)
        /// </summary>
        public virtual TimeSpan TimeStart
        {
            get => _TimeStart;
            set => SetProperty(ref _TimeStart, value);
        }

        /// <summary>
        /// UI Filter date end (Nullable)
        /// </summary>
        public virtual TimeSpan TimeEnd
        {
            get => _TimeEnd;
            set => SetProperty(ref _TimeEnd, value);
        }

        public override string Title
        {
            get
            {
                var retVal = E.T("undefined");

                if (Item != null)
                {
                    if (HasValidId)
                    {
                        retVal = Item.Name;
                    }
                    else
                    {
                        retVal = E.T("newAlgorithm");
                    }
                }

                return retVal;
            }
        }

        public string LabelAlgorithmType { get => E.T("algorithmType"); }
        public string LabelName { get => E.T("name"); }
        public string LabelDescription { get => E.T("description"); }
        public string LabelAlarm { get => E.T("alarm"); }
		//public string LabelGroup { get => E.T("group"); }
		public string LabelGroup
		{
			get => _LabelGroup;
			set => SetProperty(ref _LabelGroup, value);
		}
		// public string LabelDatapoint { get => E.T("datapoint"); }
		public string LabelDatapoint
		{
			get => _LabelDatapoint;
			set => SetProperty(ref _LabelDatapoint, value);
		}
		public string LabelValueRange { get => E.T("valueRange"); }
        public string LabelFrom { get => E.T("from"); }
        public string LabelTo { get => E.T("to"); }

        public string LabelDateRange { get => E.T("date-range"); }
        public string LabelMo { get => E.T("mo"); }
        public string LabelTu { get => E.T("tu"); }
        public string LabelWe { get => E.T("we"); }
        public string LabelTh { get => E.T("th"); }
        public string LabelFr { get => E.T("fr"); }
        public string LabelSa { get => E.T("sa"); }
        public string LabelSu { get => E.T("su"); }

        public string LabelSendValue { get => E.T("sendValue"); }
        public string LabelValueOn { get => E.T("on"); }
        public string LabelValueOff { get => E.T("off"); }

		public string LabelReminderAfterHours { get => E.T("reminderAfterHours"); }

		public string LabelSave { get => E.T("save"); }
        public string LabelDelete { get => E.T("delete"); }
        public string LabelCancel { get => E.T("cancel"); }

        public bool HasValidId { get => Item.Id != 0; }

        public bool CanBeEdited
        {
            get
            {
                var retVal = false;
                if (Item != null && Item is VisualAlgorithm)
                {
                    retVal = Item.CanBeEdited;
                }

                return retVal;
            }
        }

        public bool CanDelete
        {
            get
            {
                var retVal = false;
                if (Item != null && Item is VisualAlgorithm)
                {
                    retVal = HasValidId && Item.CanDelete;
                }

                return retVal;
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// Constructor
        /// </summary>
        public AlgorithmViewModel()
        {
        }

        #endregion

        #region Helpers
        void LoadAlgorithmTypes()
        {
            AlgorithmTypes = new PickerHandler<NamedDbItem<AlgorithmType>>(
                Item, nameof(IAlgorithm.Type), nameof(NamedDbItem<AlgorithmType>.Id));

            AlgorithmTypes.AddRange(new NamedDbItem<AlgorithmType>[]
            {
                new NamedDbItem<AlgorithmType>()
                {
                    Id = AlgorithmType.TimeTrigger,
                    Name = E.T("timeTrigger"),
                },
                new NamedDbItem<AlgorithmType>()
                {
                    Id = AlgorithmType.PeriodicTimeTrigger,
                    Name = E.T("periodicTimeTrigger"),
                },
                new NamedDbItem<AlgorithmType>()
                {
                    Id = AlgorithmType.Alarm,
                    Name = E.T("alarm"),
                },
                new NamedDbItem<AlgorithmType>()
                {
                    Id = AlgorithmType.AlarmTrigger,
                    Name = E.T("alarmTrigger"),
                },
            });
        }

        #endregion

        #region Methods
        public async Task LoadAsync()
        {
            var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(LoadAsync));
            var stage = "Init";

            try
            {
                Debug.Assert(Item != null, string.Format("{0}, Item is NULL!", vLoc));

                IsBusy = true;

                stage = "Load AlgorithmTypes";
                LoadAlgorithmTypes();

                stage = "Load Alarms";
                if (Alarms == null || Groups == null || Datapoints == null)
                {
                    Alarms = new PickerHandler<IAlgorithm>(Item, nameof(IAlgorithm.AlarmId), nameof(IAlgorithm.Id));
                    var alarms = await _ApiServices.AlarmsListAsync(D.Settings.ObjectId);
                    alarms.Insert(0, new VisualAlgorithm() { Name = E.T("nothing-selected") });
                    Alarms.AddRange(alarms);
                }

                stage = "Load Groups";
                if (Groups == null)
                {
                    Groups = new PickerHandler<IGroup>(Item, nameof(IAlgorithm.GroupId), nameof(IGroup.Id));
                    var groups = await _ApiServices.GroupListAsync(D.Settings.ObjectId);
                    groups.Insert(0, new Group() { Name = E.T("nothing-selected") });
                    Groups.AddRange(groups);
                }

                stage = "Load Datapoints";
                if (Datapoints == null)
                {
                    Datapoints = new PickerHandler<IDatapoint>(Item, nameof(IAlgorithm.DatapointId), nameof(IDatapoint.Id));
                    DatapointsRead = new PickerHandler<IDatapoint>(Item, nameof(IAlgorithm.DatapointId), nameof(IDatapoint.Id));
                    DatapointsWrite = new PickerHandler<IDatapoint>(Item, nameof(IAlgorithm.DatapointId), nameof(IDatapoint.Id));

                    var datapoints = await _ApiServices.DatapointListAsync();

                    datapoints.Insert(0, new Datapoint() { Name = E.T("nothing-selected") });

                    if (Item.Type == AlgorithmType.Alarm)
                    {
						Datapoints.AddRange(datapoints.Where(dtp => dtp.ReadWrite.Equals(0) || dtp.Id.Equals(0)));
						LabelDatapoint = E.T("datapoint");
                        LabelGroup = E.T("group");
					}
                    else
                    {
						Datapoints.AddRange(datapoints.Where(dtp => dtp.ReadWrite.Equals(1) || dtp.Id.Equals(0)));
						LabelDatapoint = E.T("datapoint-write");
						LabelGroup = E.T("group-write");
					}
					
					// Don't know why but filtering (PickerHandler<IDatapoint> Datapoints) doesn't work so I use this solution
					DatapointsRead.AddRange(datapoints.Where(dtp => dtp.ReadWrite.Equals(0) || dtp.Id.Equals(0)));
                    DatapointsWrite.AddRange(datapoints.Where(dtp => dtp.ReadWrite.Equals(1) || dtp.Id.Equals(0)));
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    string.Format("{0} [{1}]", vLoc, stage),
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
        public ICommand SaveCommand
        {
            get
            {
                return new Command(async () =>
                {
                    // Rewrite time
					Item.TimeStart = TimeStart;
					Item.TimeEnd = TimeEnd;

					// Unset unsupported parameters i.e. set to zero
					switch (Item.Type)
                    {
                        case AlgorithmType.TimeTrigger:
                            Item.AlarmId = 0;
                            Item.ValueFrom = 0;
                            Item.ValueTo = 1;
                            Item.OnMonday = false;
                            Item.OnTuesday = false;
                            Item.OnWednesday = false;
                            Item.OnThursday = false;
                            Item.OnFriday = false;
                            Item.OnSaturday = false;
                            Item.OnSunday = false;
                            break;

                        case AlgorithmType.PeriodicTimeTrigger:
                            Item.AlarmId = 0;
                            Item.ValueFrom = 0;
                            Item.ValueTo = 1;
                            Item.DateStart = DateTime.Today;
                            Item.DateEnd = DateTime.Today;
                            break;

                        case AlgorithmType.Alarm:
                            Item.AlarmId = 0;
							Item.OnMonday = false;
                            Item.OnTuesday = false;
                            Item.OnWednesday = false;
                            Item.OnThursday = false;
                            Item.OnFriday = false;
                            Item.OnSaturday = false;
                            Item.OnSunday = false;
                            break;

                        case AlgorithmType.AlarmTrigger:
                            Item.ValueFrom = 0;
                            Item.ValueTo = 1;
                            Item.DateStart = DateTime.Today;
                            Item.DateEnd = DateTime.Today;
                            Item.TimeStart = DateTime.Now.TimeOfDay;
                            Item.TimeEnd = DateTime.Now.TimeOfDay;
                            Item.OnMonday = false;
                            Item.OnTuesday = false;
                            Item.OnWednesday = false;
                            Item.OnThursday = false;
                            Item.OnFriday = false;
                            Item.OnSaturday = false;
                            Item.OnSunday = false;
                            break;

                        default:
                            break;
                    }

                    if (HasValidId)
                    {
                        await _ApiServices.AlgorithmPutAsync(Item);
                    }
                    else
                    {
                        await _ApiServices.AlgorithmPostAsync(Item);
                    }
                    await Application.Current.MainPage.Navigation.PopAsync();
                });
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var confirmationResult = await Application.Current.MainPage.DisplayAlert(
                        E.T("question"),
                        string.Format(E.T("sureDelete1"), Item.Name),
                        E.T("yes"),
                        E.T("no"));
                    if (confirmationResult)
                    {
                        await _ApiServices.AlgorithmDeleteAsync(Item);
                        await Application.Current.MainPage.Navigation.PopAsync();
                    }
                });
            }
        }

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

        #endregion
    }
}

