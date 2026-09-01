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

using Experiment.Data.Enums;
using Experiment.Core.Base;
using Experiment.Core.Ui;

using Experiment.Data.Metadata;
using Experiment.Data.Models;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.Enums;
using Experiment.Maui.Views;

namespace Experiment.Maui.ViewModels.Settings{
    public class FriendsOrBlockedViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(FriendsOrBlockedViewModel);

        #endregion

        #region Attributes
        readonly ApiServices _ApiServices = new ApiServices();
        UserRelationType _CurrentActivityType = UserRelationType.Friend;
        PickerHandler<NamedDbItem<UserRelationType>> _ActivityTypes;

        IRelatedPerson _SelectedItem;

        #endregion

        #region Properties

        public UserRelationType CurrentActivityType
        {
            get => _CurrentActivityType;
            set
            {
                var changed = !Equals(_CurrentActivityType, value);
                SetProperty(ref _CurrentActivityType, value);

                // If value really changed
                if (changed)
                {
                    LoadAsync();
                }
            }
        }
        public PickerHandler<NamedDbItem<UserRelationType>> ActivityTypes
        {
            get
            {
                if (_ActivityTypes == null)
                {
                    _ActivityTypes = new PickerHandler<NamedDbItem<UserRelationType>>(
                        this, nameof(CurrentActivityType), nameof(NamedDbItem<UserRelationType>.Id));

                    _ActivityTypes.AddRange(new NamedDbItem<UserRelationType>[]
                    {
                        new NamedDbItem<UserRelationType>()
                        {
                            Id = UserRelationType.Friend,
                            Name = E.T("friends"),
                        },
                        new NamedDbItem<UserRelationType>()
                        {
                            Id = UserRelationType.Blocked,
                            Name = E.T("blocked"),
                        }
                    });
                }

                return _ActivityTypes;
            }
        }

        public ObservableCollection<IRelatedPerson> Items { get; protected set; } =
            new ObservableCollection<IRelatedPerson>();

        public IRelatedPerson SelectedItem
        {
            get => _SelectedItem;
            set
            {
                SetProperty(ref _SelectedItem, value);
                OnPropertyChanged(nameof(IsAnythingSelected));
            }
        }

        public bool IsAnythingSelected
        {
            get => _SelectedItem != null && _SelectedItem is IRelatedPerson;
        }

        public string LabelAdd { get => E.T("new"); }
        public string LabelDelete { get => E.T("delete"); }
        public string LabelType { get => E.T("type"); }
        public string LabelProfile { get => E.T("profile"); }

        #endregion

        #region CTOR

        public FriendsOrBlockedViewModel()
        {
            Title = E.T("friendsOrFamily");
            Items = new ObservableCollection<IRelatedPerson>();
        }

        #endregion

        #region Helpers
        async Task WarningNothingSelected()
        {
            await Application.Current.MainPage.DisplayAlert(
                E.T("warning"),
                E.T("nothing-selected"),
                E.T("ok"));
        }

        #endregion

        #region Methods
        public async Task LoadAsync()
        {
            var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(LoadAsync));
            try
            {
                IsBusy = true;
                Items.Clear();

                IEnumerable<IRelatedPerson> items = null;
                if (CurrentActivityType == UserRelationType.Friend)
                {
                    items = await Dictionaries.Instance.GetFriends(true);
                }
                else if (CurrentActivityType == UserRelationType.Blocked)
                {
                    items = await Dictionaries.Instance.GetBlocked(true);
                }

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        Items.Add(item);
                    }
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

        public ICommand NewCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PushAsync(
                        new V.Settings.FriendOrBlockedNewPage()
                        {
                            BindingContext = new FriendOrBlockedNewViewModel()
                            {
                                CurrentActivityType = CurrentActivityType,
                            }
                        });
                });
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (IsAnythingSelected)
                    {
                        var answer = await Application.Current.MainPage.DisplayAlert(
                            E.T("question"),
                            string.Format("{0}\r\n\r\n{1}", E.T("sure-delete"), SelectedItem.Name),
                            E.T("yes"),
                            E.T("no"));

                        if (answer)
                        {
                            if (CurrentActivityType == UserRelationType.Friend)
                            {
                                await _ApiServices.FriendDeleteAsync(SelectedItem);
                            }
                            else
                            {
                                await _ApiServices.BlockedDeleteAsync(SelectedItem);
                            }

                            await LoadAsync();
                        }
                    }
                    else
                    {
#warning @TODO: Is this till available? Investigate it.
                        OnPropertyChanged("IsSearchTextAvailable");
                        await WarningNothingSelected();
                    }
                });
            }
        }
        public ICommand UserProfileCommand
        {
            get
            {
                //return new Command(async () =>
                return new Command(async (e) =>
                {
                    if (e is IRelatedPerson)
                    {
                        var related = e as IRelatedPerson;
                        await Application.Current.MainPage.Navigation.PushAsync(
                            new V.Ecosystem.UserProfilePage()
                            {
                                BindingContext = new VM.Ecosystem.UserProfileViewModel()
                                {
                                    UserId = related.RelatedUserId,
                                },
                            });
                    }
                });
            }
        }
        #endregion

    }
}

