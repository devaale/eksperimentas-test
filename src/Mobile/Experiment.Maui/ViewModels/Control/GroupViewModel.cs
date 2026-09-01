using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Maui.Controls;

// MVVM
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Core.Base;
using Experiment.Data.Metadata;

using Experiment.Maui.Data;
using Experiment.Maui.Services;


namespace Experiment.Maui.ViewModels.Control{
    public class GroupViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(GroupViewModel);

        #endregion

        #region Attributes
        ApiServices _ApiServices = new ApiServices();

        #endregion

        #region Properties
        public M.Group Item { get; set; }

        public ObservableCollection<IDatapoint> Items { get; protected set; } =
            new ObservableCollection<IDatapoint>();

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
                        retVal = E.T("newGroup");
                    }
                }

                return retVal;
            }
        }

        public string LabelGroupInfo { get => E.T("groupInfo"); }
        public string LabelDescription { get => E.T("description"); }
        public string LabelDatapoints { get => E.T("datapoints"); }
        public string LabelSave { get => E.T("save"); }
        public string LabelDelete { get => E.T("delete"); }
        public string LabelCancel { get => E.T("cancel"); }

        public bool HasValidId { get => Item.Id != 0; }

        public bool CanBeEdited
        {
            get
            {
                var retVal = false;
                if (Item != null && Item is M.Group)
                {
                    retVal = Item.Editable;
                }
                retVal = true;
                return retVal;
            }
        }

        public bool CanDelete
        {
            get
            {
                var retVal = false;
                if (Item != null && Item is M.Group)
                {
                    retVal = HasValidId && Item.Editable;
                }
                retVal = true;
                return retVal;
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// Constructor
        /// </summary>
        public GroupViewModel()
        {
            //Items = new ObservableCollection<IDatapoint>();
        }

        #endregion

        #region Methods

        public async Task LoadAsync()
        {
            var vLoc = string.Format("{0}:{1}", TYPE_NAME, nameof(LoadAsync));
            try
            {
                IsBusy = true;
                Items.Clear();

                int deviceId = 29;
                var items = await _ApiServices.DatapointListByDeviceIdsAsync(deviceId.ToString());

                foreach (var item in items)
                {
                    Items.Add(item);
                }
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
        public ICommand SaveCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (HasValidId)
                    {
                        await _ApiServices.GroupPutAsync(Item);
                    }
                    else
                    {
                        await _ApiServices.GroupPostAsync(Item);
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
                        await _ApiServices.GroupDeleteAsync(Item);
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

