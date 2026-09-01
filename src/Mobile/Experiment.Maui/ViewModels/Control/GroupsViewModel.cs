using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Maui.Controls;

// MVVM
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views.Control;
using VM = Experiment.Maui.ViewModels.Control;
using D = Experiment.Maui.Data;

using Experiment.Core.Base;
using Experiment.Data.Metadata;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.Views;
using System.Net.Http;

namespace Experiment.Maui.ViewModels.Control{
    public class GroupsViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(GroupsViewModel);

        #endregion

        #region Attributes
        readonly ApiServices _ApiServices = new ApiServices();
        ObservableCollection<M.Group> _Items = new ObservableCollection<M.Group>();
        M.Group _SelectedItem;
        bool _IsSelectionMode = false;

        int ObjectId = 0;

        #endregion

        #region Properties
        public string LabelAdd { get => E.T("new"); }
        public string LabelEdit { get => E.T("edit"); }
        public string LabelContinue { get => E.T("continue"); }

        public ObservableCollection<M.Group> Items
        {
            get => _Items;
            set => SetProperty(ref _Items, value);
        }

        public M.Group SelectedItem
        {
            get => Items.FirstOrDefault(i => i.Id.Equals(D.Settings.Group));
            set
            {
                if (value == null)
                {
                    D.Settings.Group = 0;
                    D.Settings.GroupName = string.Empty;
                }
                else
                {
                    D.Settings.Group = value.Id;
                    D.Settings.GroupName = value.Name;
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsGroupSelected));
                OnPropertyChanged(nameof(SelectedItemDesc));
            }
        }

        public string SelectedItemDesc
        {
            get => E.T("groupSelected") + " " + (string.IsNullOrEmpty(D.Settings.GroupName) ? E.T("none") : D.Settings.GroupName);
        }

        public bool IsSelectionMode
        {
            get => _IsSelectionMode;
            set => SetProperty(ref _IsSelectionMode, value);
        }

        public bool IsGroupSelected
        {
            get
            {
                return SelectedItem != null;
            }
        }

        #endregion

        #region CTOR
        public GroupsViewModel()
        {
            ObjectId = D.Settings.ObjectId;

            Title = E.T("groups");
            //Items = new ObservableCollection<M.Group>();
        }

        #endregion

        #region Helpers
        async Task ShowSelectedItem(M.Group item)
        {
            if (SelectedItem is IGroup)
            {
                await Application.Current.MainPage.Navigation.PushAsync(
                    new V.GroupPage()
                    {
                        BindingContext = new GroupViewModel()
                        {
                            Item = item,
                        },
                    });
            }
        }

        #endregion

        #region Methods
        public async Task LoadAsync()
        {
            var vLoc = TYPE_NAME + "::LoadAsync()";
            try
            {
                IsBusy = true;

                Items.Clear();

                var grp = await _ApiServices.GroupListAsync(D.Settings.ObjectId);
                foreach (var i in grp)
                {
                    Items.Add(i);
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
                SelectedItem = SelectedItem;
            }
        }

        #endregion

        #region Commands

        public ICommand NewRecordCommand
        {
            get
            {
                return new Command(async () =>
                {

                    var grp = await _ApiServices.GroupNewAsync(ObjectId);

                    await Application.Current.MainPage.Navigation.PushAsync(
                        new V.GroupPage()
                        {
                            BindingContext = new GroupViewModel()
                            {
                                Item = grp,
                            },
                        });
                });
            }
        }

        public ICommand EditRecordCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await ShowSelectedItem(SelectedItem);
                });
            }
        }

        public ICommand ContinueCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (SelectedItem == null)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            E.T("warning"),
                            E.T("goupSelect"),
                            E.T("ok"));
                    }
                    else
                    {
                        await Workflow.Startup();
                        //await Application.Current.MainPage.Navigation.PushAsync(new MainMenuPage());
                    }
                });
            }
        }

        #endregion
    }
}

